using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public class DeployFramework : Pipeline
    {
        public DeployFramework()
        {
            Deployment = true;

            Dependencies.Add(nameof(PublishFramework));
        }
    }
}