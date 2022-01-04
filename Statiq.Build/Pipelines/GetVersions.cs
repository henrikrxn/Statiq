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
                    foreach (string name in ProjectNames.GetAll())
                    {
                        string version = await context.GetVersionFromReleaseFileAsync(name);
                        context.LogInformation($"{name} version {version}");
                        metadata.Add(name, version);
                    }
                    return context.CreateDocument(metadata);
                }))
            };
        }
    }
}