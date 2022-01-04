namespace Statiq.Build.Pipelines
{
    public class PackFramework : PackBase
    {
        public PackFramework()
            : base(ProjectNames.Framework, true)
        {
        }
    }
}