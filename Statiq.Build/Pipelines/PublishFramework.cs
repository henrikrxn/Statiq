using System.Collections.Generic;
using System.IO;

namespace Statiq.Build.Pipelines
{
    public class PublishFramework : PublishBase
    {
        public PublishFramework()
            : base(ProjectNames.Framework)
        {
        }
    }
}