using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public abstract class BuildBase : Pipeline
    {
        private const int RestoreRetries = 40;

        private const int RestoreDelaySeconds = 30;

        protected BuildBase(
            string name,
            bool nestedProjectFiles,
            params string[] references)
        {
            Dependencies.Add(nameof(GetVersions));

            if (references is object)
            {
                foreach (string reference in references)
                {
                    Dependencies.Add($"Build{reference}");
                }
            }

            ProcessModules = new ModuleList
            {
                new ReadFiles($"Statiq.{name}/src/{(nestedProjectFiles ? "**" : "*")}/*.csproj"),
                new RetryModules(
                    new StartProcess("dotnet")
                        .WithArgument("restore")
                        .WithArgument("--no-cache")
                        .WithVersions()
                        .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                        .WithParallelExecution(false)
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
    }
}