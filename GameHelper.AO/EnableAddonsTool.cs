using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using GameHelper.Interfaces;

namespace GameHelper.AO
{
    public class EnableAddonsTool : ITool
    {
        private readonly string _rootFolder;

        public EnableAddonsTool(string rootFolder)
        {
            _rootFolder = rootFolder ?? throw new ArgumentNullException(nameof(rootFolder));
            if (!Directory.Exists(_rootFolder))
                throw new DirectoryNotFoundException(rootFolder);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var configFile = Path.Combine(_rootFolder, "Personal\\user.cfg");
            if (!File.Exists(configFile))
                throw new NotImplementedException();

            var version = await GetActualVersionAsync(cancellationToken);
            await Fix(configFile, version, cancellationToken);
        }

        private async Task<string> GetActualVersionAsync(CancellationToken cancellationToken)
        {
            var fileName = Path.Combine(_rootFolder, "-gup-\\last.xml");
            using var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var xml = await XElement.LoadAsync(file, LoadOptions.None, cancellationToken);

            var versionNum = xml.Attribute("VersionNum");
            return versionNum.Value;
        }

        private static async Task Fix(string configFile, string actualVersion, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await using var file1 = new FileStream(configFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(file1);
            var text = await reader.ReadToEndAsync();
            file1.Close();

            text = GetFixed(text, actualVersion);

            cancellationToken.ThrowIfCancellationRequested();

            await using var file2 = new FileStream(configFile, FileMode.Create, FileAccess.Write, FileShare.None);
            await using var writer = new StreamWriter(file2);
            await writer.WriteAsync(text);
            await writer.FlushAsync();
        }

        private static string GetFixed(string text, string actualVersion)
        {
            text = text.Replace("enabled=false", "enabled=true");

            var versionLine = $"version=L\"{actualVersion}\"";
            var sb = new StringBuilder(text.Length);
            foreach (var line in text.Split(Environment.NewLine))
            {
                if (line.Contains("version=L"))
                {
                    var i = line.IndexOf("version=L", StringComparison.InvariantCultureIgnoreCase);
                    sb.AppendLine(line.Substring(0, i) + versionLine);
                }
                else
                    sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}
