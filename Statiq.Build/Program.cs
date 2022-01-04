using System;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;

namespace Statiq.Build
{
    public class Program
    {
        private static readonly NormalizedPath ArtifactsFolder = "artifacts";

        public static async Task<int> Main(string[] args) => await Bootstrapper
            .Factory
            .CreateDefault(args)
            .ConfigureFileSystem(x =>
            {
                x.RootPath = new NormalizedPath(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent;
                x.OutputPath = x.RootPath / ArtifactsFolder;
                x.InputPaths.Clear();
                x.InputPaths.Add(x.RootPath);
            })
            .ConfigureSettings(settings =>
            {
                settings.Add(Settings.IsBuildServer, settings.ContainsAnyKeys("GITHUB_ACTIONS", "TF_BUILD"));
                settings[Keys.CleanMode] = CleanMode.Full;
            })
            .AddPipelines()
            .RunAsync();
    }
}