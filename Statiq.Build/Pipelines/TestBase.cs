using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public abstract class TestBase : Pipeline
    {
        protected TestBase(
            string name,
            bool nestedProjectFiles)
        {
            Dependencies.Add($"Build{name}");

            ProcessModules = new ModuleList
            {
                new ReadFiles($"Statiq.{name}/tests/{(nestedProjectFiles ? "**" : "*")}/*.csproj"),
                new StartProcess("dotnet")
                    .WithArgument("test")
                    // See https://github.com/dotnet/sdk/issues/16122
                    .WithArgument("--logger \"console;verbosity=detailed\"")
                    .WithArgument("--no-build")
                    .WithArgument("--no-restore")
                    .WithVersions()
                    .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                    .WithParallelExecution(false)
                    .LogOutput()
            };
        }
    }
}