using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    // This is just to trigger the publishing step on a deploy command
    public class Deploy : Pipeline, INamedPipeline
    {
        private readonly Project _project;

        public Deploy(Project project)
        {
            _project = project;

            Deployment = true;

            Dependencies.Add($"{nameof(Publish)}{project.Name}");
        }

        public string PipelineName => $"{nameof(Deploy)}{_project.Name}";
    }
}