using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kodeks
{
    public static class TechexpertClientExtensions
    {
        public static async Task<string> GetbparserAsync(this TechexpertClient client, string text)
        {
            string url = $"{client.Endpoint.Address.Uri.AbsoluteUri}/bparser?parse={Uri.EscapeDataString(text)}";
            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task<string> KClientRemotePath(this TechexpertClient client)
        {
            string url = $"{client.Endpoint.Address.Uri.AbsoluteUri}";
            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(url))
            {
                var data = await response.Content.ReadAsStringAsync();
                string pattern = @"Клиент, который следует использовать с этим сервером - (?<path>.[^\s]*)\s";
                if (Regex.IsMatch(data, pattern))
                {
                    return Regex.Match(data, pattern).Groups["path"].Value;
                }
                else
                {
                    throw new Exception($"Невозможно определить путь к специальному клиентскому приложению \"Техэксперт-клиент\"\r\n{data}");
                }
            }
        }

        public static async Task RunKodeks(this TechexpertClient client, string link)
        {
            if (IsUriSupported(link))
            {
                Process.Start(link);
            }
            else
            {
                await client.StartKClientProcess(link);
            }
        }

        private static bool IsUriSupported(string link)
        {
            if (Uri.TryCreate(link, UriKind.Absolute, out Uri uri))
            {
                var registryValue = Registry.GetValue(@$"HKEY_CLASSES_ROOT\{uri.Scheme}", "", null);
                return registryValue != null;
            }
            return false;
        }

        private static async Task StartKClientProcess(this TechexpertClient client, string link)
        {
            var fileName = await client.KClientRemotePath();
            var workingDirectory = Path.GetDirectoryName(fileName);

            var uri = client.Endpoint.Address.Uri;
            var argument = link.Replace("kodeks://link/", $"{uri.Scheme}://{uri.Host}:{uri.Port}/");

            if (File.Exists(fileName))
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = argument,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                };
                Process.Start(startInfo);
            }
            else
            {
                throw new Exception("Необходимо установить специальное клиентское приложение \"Техэксперт-клиент\"");
            }
        }
    }
}
