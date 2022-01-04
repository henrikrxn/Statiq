using Statiq.Build.Pipelines;
using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build
{
    public static class StartProcessExtensions
    {
        public static StartProcess WithVersions(this StartProcess startProcess)
        {
            foreach (string name in ProjectNames.GetAll())
            {
                startProcess = startProcess.WithArgument(Config.FromContext(context =>
                    $"-p:Statiq{name}Version=\"{context.Outputs.FromPipeline(nameof(GetVersions))[0].GetString(name)}\""));
            }
            return startProcess;
        }
    }
}