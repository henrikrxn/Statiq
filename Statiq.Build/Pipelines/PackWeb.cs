namespace Statiq.Build.Pipelines
{
    public class PackWeb : PackBase
    {
        public PackWeb()
            : base(ProjectNames.Web, false)
        {
        }
    }
}