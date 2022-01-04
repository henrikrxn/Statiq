using System;
using System.Threading.Tasks;
using Statiq.Common;

namespace Statiq.Build
{
    public static class ExecutionContextExtensions
    {
        public static async Task<string> GetVersionFromReleaseFileAsync(
            this IExecutionContext context,
            string name)
        {
            NormalizedPath directory = $"Statiq.{name}";
            IFile releaseFile = context.FileSystem.GetInputFile(directory.Combine("RELEASE.md"));
            string content = await releaseFile.ReadAllTextAsync(context.CancellationToken);
            string firstLine = content.Trim().Split('\r', '\n', StringSplitOptions.RemoveEmptyEntries)[0];
            return firstLine.TrimStart('#').Trim();
        }
    }
}