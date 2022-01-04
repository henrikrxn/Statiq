using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public class DeployDocs : Pipeline
    {
        public DeployDocs()
        {
            Deployment = true;

            Dependencies.Add(nameof(PublishDocs));
        }
    }
}