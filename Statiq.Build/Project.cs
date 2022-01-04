namespace Statiq.Build
{
    public class Project
    {
        public static readonly Project[] All = new Project[]
        {
            new Project("Framework", true),
            new Project("Web", false, "Framework"),
            new Project("Docs", false, "Web", "Framework")
        };
        
        public Project(string name, bool nestedProjectFiles, params string[] references)
        {
            Name = name;
            NestedProjectFiles = nestedProjectFiles;
            References = references;
        }

        public string Name { get; }

        public bool NestedProjectFiles { get; }

        public string[] References { get; }
    }
}