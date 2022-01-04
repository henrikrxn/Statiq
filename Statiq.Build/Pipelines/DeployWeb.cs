using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public class DeployWeb : Pipeline
    {
        public DeployWeb()
        {
            Deployment = true;

            Dependencies.Add(nameof(PublishWeb));
        }
    }
}