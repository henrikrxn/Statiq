using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public class Test : Pipeline, INamedPipeline
    {
        private readonly Project _project;

        public Test(Project project)
        {
            _project = project;

            Dependencies.Add($"{nameof(Build)}{project.Name}");

            if (project.References is object)
            {
                foreach (string reference in project.References)
                {
                    Dependencies.Add($"{nameof(Test)}{reference}");
                }
            }

            ProcessModules = new ModuleList
            {
                new ReadFiles($"Statiq.{project.Name}/tests/{(project.NestedProjectFiles ? "**" : "*")}/*.csproj"),
                new StartProcess("dotnet")
                    .WithArgument("test")
                    .WithArgument("--no-build")
                    .WithArgument("--no-restore")
                    .WithVersions()
                    .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                    .WithParallelExecution(false)
                    .LogOutput()
            };
        }

        public string PipelineName => $"{nameof(Test)}{_project.Name}";
    }
}