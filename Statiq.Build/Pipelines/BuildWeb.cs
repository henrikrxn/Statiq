namespace Statiq.Build.Pipelines
{
    public class BuildWeb : BuildBase
    {
        public BuildWeb()
            : base(ProjectNames.Web, false, ProjectNames.Framework)
        {
        }
    }
}