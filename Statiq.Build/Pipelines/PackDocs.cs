namespace Statiq.Build.Pipelines
{
    public class PackDocs : PackBase
    {
        public PackDocs()
            : base(ProjectNames.Docs, false)
        {
        }
    }
}