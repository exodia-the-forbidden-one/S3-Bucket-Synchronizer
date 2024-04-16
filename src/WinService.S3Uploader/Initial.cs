using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon.S3;
using S3Uploader.Helpers;
using System.IO;
using System.Security.Cryptography;

namespace S3Uploader
{
    public class Initial
    {
        public string PathToWatch { get; set; }
        public QueueService QueueService { get; set; }
        public Awssettings AwsSettings { get; set; }


        public async Task<List<string>> GetS3Files()
        {
            try
            {
                List<string> objects = new List<string>();

                using (var client = new AmazonS3Client(AwsSettings.AccessKey, AwsSettings.SecretKey, Amazon.RegionEndpoint.USEast1))
                {
                    ListObjectsV2Request request = new ListObjectsV2Request
                    {
                        BucketName = AwsSettings.BucketName
                    };

                    ListObjectsV2Response response;
                    do
                    {
                        response = await client.ListObjectsV2Async(request);

                        foreach (var obj in response.S3Objects)
                        {
                            string key = Uri.UnescapeDataString(obj.Key.Replace("/","\\"));
                            string etag = obj.ETag.Replace("\"", "");
                            objects.Add(key + ">" + etag);
                        }

                        request.ContinuationToken = response.NextContinuationToken;
                    } while (response.IsTruncated);
                }

                return objects;
            }
            catch
            {
                return new List<string>();
            }
        }

        public List<string> GetLocalFiles(string folderPath)
        {
            List<string> filePaths = new List<string>();

            try
            {
                string[] filePathArray = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                foreach (var filePath in filePathArray)
                {
                    Uri currentUri = new Uri(PathToWatch);
                    Uri targetUri = new Uri(filePath);
                    Uri relativeUri = currentUri.MakeRelativeUri(targetUri);

                    string relativePath = Uri.UnescapeDataString(relativeUri.ToString().Replace("/","\\"));

                    filePaths.Add(relativePath + ">" + CalcEtag(filePath));
                }
            }
            catch (Exception ex)
            {
                ErrorHelper.HandleError($"Hata oluştu: {ex.Message}");
            }

            return filePaths;
        }

        public async Task GetDifferences(List<string> localFiles, List<string> s3Objects)
        {
            foreach (string file in localFiles)
            {
                if (!s3Objects.Contains(file))
                {
                    var parentPath = Directory.GetParent(PathToWatch);

                    if (parentPath != null)
                    {
                        string[] parts = file.Split('>');
                        string path = parts[0].Trim();

                        FileObject fileObject = new FileObject
                        {
                            FilePath = Path.Combine(parentPath.ToString(), path.Replace('/', '\\')),
                            BaseFolder = PathToWatch,
                            AwsSettings = AwsSettings
                        };
                        await QueueService.AddAsync(fileObject);
                    }
                }
            }
        }

        public string CalcEtag(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).ToLower().Replace("-", "");
                }
            }
        }
    }
}