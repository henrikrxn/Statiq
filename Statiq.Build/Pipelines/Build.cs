using System;
using Microsoft.Extensions.Logging;
using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public class Build : Pipeline, INamedPipeline
    {
        private const int RestoreRetries = 40;

        private const int RestoreDelaySeconds = 30;

        private readonly Project _project;

        public Build(Project project)
        {
            _project = project;

            Dependencies.Add(nameof(GetVersions));

            if (project.References is object)
            {
                foreach (string reference in project.References)
                {
                    Dependencies.Add($"{nameof(Build)}{reference}");
                }
            }

            ProcessModules = new ModuleList
            {
                new ReadFiles($"Statiq.{project.Name}/src/{(project.NestedProjectFiles ? "**" : "*")}/*.csproj"),
                new RetryModules(
                    new StartProcess("dotnet")
                        .WithArgument("restore")
                        .WithArgument("--no-cache")
                        .WithVersions()
                        .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                        .WithParallelExecution(false)
                        .LogErrors(false)
                        .LogOutput())
                    .WithRetries(RestoreRetries)
                    .WithSleepDuration(x => TimeSpan.FromSeconds(x * RestoreDelaySeconds))
                    .WithFailureMessage(
                        x => $"Error restoring packages, retry {x} of {RestoreRetries} in {RestoreDelaySeconds} seconds...",
                        LogLevel.Warning),
                new StartProcess("dotnet")
                    .WithArgument("build")
                    .WithVersions()
                    .WithArgument("-p:ContinuousIntegrationBuild=\"true\"")
                    .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                    .WithParallelExecution(false)
                    .LogOutput()
            };
        }

        public string PipelineName => $"{nameof(Build)}{_project.Name}";
    }
}