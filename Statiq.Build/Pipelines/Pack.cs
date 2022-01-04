using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public class Pack : Pipeline, INamedPipeline
    {
        private readonly Project _project;

        public Pack(Project project)
        {
            _project = project;

            Dependencies.Add($"{nameof(Test)}{project.Name}");

            if (project.References is object)
            {
                foreach (string reference in project.References)
                {
                    Dependencies.Add($"{nameof(Pack)}{reference}");
                }
            }

            ProcessModules = new ModuleList
            {
                new ThrowExceptionIf(Config.ContainsSettings("DAVIDGLICK_CERTPASS").IsFalse(), "DAVIDGLICK_CERTPASS setting missing"),
                new ReadFiles($"Statiq.{project.Name}/src/{(project.NestedProjectFiles ? "**" : "*")}/*.csproj"),
                new StartProcess("dotnet")
                    .WithArgument("pack")
                    .WithArgument("--no-build")
                    .WithArgument("--no-restore")
                    .WithVersions()
                    .WithArgument("-o", Config.FromContext(ctx => ctx.FileSystem.GetOutputPath($"Statiq.{project.Name}").FullPath), true)
                    .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                    .WithParallelExecution(false)
                    .LogOutput(),
                new ReadFiles(Config.FromContext(ctx => ctx.FileSystem.GetOutputPath($"Statiq.{project.Name}/*.nupkg").FullPath)),
                new StartProcess("nuget")
                    .WithArgument("sign")
                    .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                    .WithArgument("-CertificatePath", Config.FromContext(ctx => ctx.FileSystem.GetRootFile("davidglick.pfx").Path.FullPath), true)
                    .WithArgument("-CertificatePassword", Config.FromSetting("DAVIDGLICK_CERTPASS"), true)
                    .WithArgument("-Timestamper", "http://timestamp.digicert.com", true)
                    .WithArgument("-NonInteractive")
                    .WithParallelExecution(false)
                    .HideArguments(true)
                    .LogOutput()
            };
        }

        public string PipelineName => $"{nameof(Pack)}{_project.Name}";
    }
}