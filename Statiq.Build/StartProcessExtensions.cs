using Statiq.Build.Pipelines;
using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build
{
    public static class StartProcessExtensions
    {
        public static StartProcess WithVersions(this StartProcess startProcess)
        {
            foreach (Project project in Project.All)
            {
                startProcess = startProcess.WithArgument(Config.FromContext(context =>
                    $"-p:Statiq{project.Name}Version=\"{context.Outputs.FromPipeline(nameof(GetVersions))[0].GetString(project.Name)}\""));
            }
            return startProcess;
        }
    }
}