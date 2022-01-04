using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public abstract class PackBase : Pipeline
    {
        protected PackBase(string name, bool nestedProjectFiles)
        {
            Dependencies.Add($"Test{name}");

            ProcessModules = new ModuleList
            {
                new ThrowExceptionIf(Config.ContainsSettings("DAVIDGLICK_CERTPASS").IsFalse(), "DAVIDGLICK_CERTPASS setting missing"),
                new ReadFiles($"Statiq.{name}/src/{(nestedProjectFiles ? "**" : "*")}/*.csproj"),
                new StartProcess("dotnet")
                    .WithArgument("pack")
                    .WithArgument("--no-build")
                    .WithArgument("--no-restore")
                    .WithVersions()
                    .WithArgument("-o", Config.FromContext(ctx => ctx.FileSystem.GetOutputPath($"Statiq.{name}").FullPath), true)
                    .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                    .WithParallelExecution(false)
                    .LogOutput(),
                new ReadFiles(Config.FromContext(ctx => ctx.FileSystem.GetOutputPath($"Statiq.{name}/*.nupkg").FullPath)),
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
    }
}