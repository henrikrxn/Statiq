using System;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;

namespace Statiq.Build
{
    public class Program
    {
        private static readonly NormalizedPath ArtifactsFolder = "artifacts";

        public static async Task<int> Main(string[] args)
        {
            Bootstrapper bootstrapper = Bootstrapper
                .Factory
                .CreateDefaultWithout(args, DefaultFeatures.Pipelines)
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
                .AddPipeline<Pipelines.GetVersions>();

            foreach (Project project in Project.All)
            {
                bootstrapper.AddPipeline(new Pipelines.Build(project));
                bootstrapper.AddPipeline(new Pipelines.Test(project));
                bootstrapper.AddPipeline(new Pipelines.Pack(project));
                bootstrapper.AddPipeline(new Pipelines.Publish(project));
                bootstrapper.AddPipeline(new Pipelines.Deploy(project));
            }

            return await bootstrapper.RunAsync();
        }
    }
}