using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Octokit;
using Statiq.Common;
using Statiq.Core;

namespace Statiq.Build.Pipelines
{
    public class Publish : Pipeline, INamedPipeline
    {
        private readonly Project _project;

        public Publish(Project project)
        {
            _project = project;

            // Don't publish unless told to (I.e. during deployment as a dependency of the corresponding DeployXyz pipeline)
            ExecutionPolicy = ExecutionPolicy.Manual;

            Dependencies.Add($"{nameof(Pack)}{project.Name}");

            if (project.References is object)
            {
                foreach (string reference in project.References)
                {
                    Dependencies.Add($"{nameof(Publish)}{reference}");
                }
            }

            ProcessModules = new ModuleList
            {
                new ThrowExceptionIf(Config.ContainsSettings("STATIQ_NUGET_API_KEY").IsFalse(), "STATIQ_NUGET_API_KEY setting missing"),
                new ThrowExceptionIf(Config.ContainsSettings("STATIQ_GITHUB_TOKEN").IsFalse(), "STATIQ_GITHUB_TOKEN setting missing"),
                new ReadFiles(Config.FromContext(ctx => ctx.FileSystem.GetOutputPath($"Statiq.{project.Name}/*.nupkg").FullPath)),
                new StartProcess("nuget")
                    .WithArgument("push")
                    .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                    .WithArgument("-ApiKey", Config.FromSetting("STATIQ_NUGET_API_KEY"), true)
                    .WithArgument("-Source", "https://api.nuget.org/v3/index.json", true)
                    .WithArgument("-SkipDuplicate")
                    .WithParallelExecution(false)
                    .LogOutput(),
                new ExecuteConfig(Config.FromContext(async ctx =>
                {
                    // Get release notes
                    IFile releaseNotesFile = ctx.FileSystem.GetInputFile($"Statiq.{project.Name}/RELEASE.md");
                    string releaseNotes = await releaseNotesFile.ReadAllTextAsync();
                    string[] lines = releaseNotes.Trim().Split("\n#", StringSplitOptions.RemoveEmptyEntries)[0].Trim().Split("\n").Select(x => x.Trim()).ToArray();
                    string version = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];
                    string notes = string.Join(Environment.NewLine, lines.Skip(1).SkipWhile(x => string.IsNullOrWhiteSpace(x)));

                    ctx.LogInformation("Version " + version);
                    ctx.LogInformation("Notes " + Environment.NewLine + notes);

                    // Add release to GitHub
                    GitHubClient github = new GitHubClient(new ProductHeaderValue("Statiq"))
                    {
                        Credentials = new Credentials(ctx.GetString("STATIQ_GITHUB_TOKEN"))
                    };

                    try
                    {
                        Release release = await github.Repository.Release.Create("statiqdev", $"Statiq.{project.Name}", new NewRelease("v" + version)
                        {
                            Name = version,
                            Body = string.Join(Environment.NewLine, notes),
                            TargetCommitish = "main",
                            Prerelease = version.Contains('-')
                        });
                        ctx.LogInformation($"Added release {release.Name} to GitHub");
                    }
                    catch (Exception e)
                    {
                        ctx.LogWarning($"Could not add release to GitHub: {e.Message}");
                    }
                }))
            };
        }

        public string PipelineName => $"{nameof(Publish)}{_project.Name}";
    }
}