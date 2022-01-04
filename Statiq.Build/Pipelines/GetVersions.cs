using Microsoft.Extensions.Logging;
using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public class GetVersions : Pipeline
    {
        public GetVersions()
        {
            ProcessModules = new ModuleList
            {
                new ExecuteConfig(Config.FromContext(async context =>
                {
                    MetadataItems metadata = new MetadataItems();
                    foreach (Project project in Project.All)
                    {
                        string version = await context.GetVersionFromReleaseFileAsync(project.Name);
                        context.LogInformation($"{project.Name} version {version}");
                        metadata.Add(project.Name, version);
                    }
                    return context.CreateDocument(metadata);
                }))
            };
        }
    }
}