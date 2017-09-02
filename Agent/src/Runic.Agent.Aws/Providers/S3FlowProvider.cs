using System;
using Runic.Framework.Models;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Runic.Agent.Aws.Providers
{
    public class S3FlowProvider : IFlowProvider
    {
        private readonly IAmazonS3 _s3Client;
        public S3FlowProvider(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<Flow> GetFlow(string key)
        {
            var objectRequest = new GetObjectRequest()
            {
                BucketName = "dev-runic-agent-flows",
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(objectRequest);
            StreamReader reader = new StreamReader(response.ResponseStream);
            String content = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<Flow>(content);
        }
    }
}
