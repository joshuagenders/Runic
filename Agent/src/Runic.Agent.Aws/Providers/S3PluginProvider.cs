using Amazon.S3;
using Amazon.S3.Model;
using Runic.Agent.Aws.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Runic.Agent.Aws.Providers
{
    public class S3PluginProvider : IPluginProvider
    {
        private readonly IAgentConfig _agentConfig;
        private readonly IAmazonS3 _s3Client;
        public S3PluginProvider(IAgentConfig agentConfig, IAmazonS3 s3Client)
        {
            _agentConfig = agentConfig;
            _s3Client = s3Client;
        }
        public string GetFilepath(string key)
        {
            return Path.Combine(GetFolderPath(), key, $"{key}.dll");
        }

        private string GetFolderPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), _agentConfig.AgentSettings.AgentPluginDirectory);
        }

        public void RetrieveSourceDll(string key)
        {
            if (!Directory.Exists(GetFolderPath()))
                Directory.CreateDirectory(GetFolderPath());
            
            var objectRequest = new GetObjectRequest()
            {
                BucketName = "dev-runic-agent-plugins",
                Key = $"{key}.zip"
            };
            var cts = new CancellationTokenSource();
            cts.CancelAfter(8000);
            var response = _s3Client.GetObjectAsync(objectRequest)
                                    .GetAwaiter()
                                    .GetResult();

            var extractPath = Path.Combine(GetFolderPath(), key);
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath);
            }
            Directory.CreateDirectory(extractPath);
            var zipPath = Path.Combine(GetFolderPath(), $"{key}.zip");
            response.WriteResponseStreamToFileAsync(zipPath, false, cts.Token);

            ZipArchive archive = ZipFile.OpenRead(zipPath);
            archive.ExtractToDirectory(extractPath);
        }
    }
}
