namespace Statiq.Build.Pipelines
{
    public class BuildDocs : BuildBase
    {
        public BuildDocs()
            : base(ProjectNames.Docs, false, ProjectNames.Web, ProjectNames.Framework)
        {
        }
    }
}