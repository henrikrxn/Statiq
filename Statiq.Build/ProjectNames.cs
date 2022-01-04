using System.Collections.Generic;
using System.Linq;

namespace Statiq.Build
{
    public static class ProjectNames
    {
        public const string Framework = nameof(Framework);
        public const string Web = nameof(Web);
        public const string Docs = nameof(Docs);

        public static IEnumerable<string> GetAll() =>
            typeof(ProjectNames).GetFields().Select(x => x.Name);
    }
}