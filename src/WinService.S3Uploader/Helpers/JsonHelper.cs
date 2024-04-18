using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace S3Uploader.Helpers
{
    internal class JsonHelper
    {
        public static List<Configuration> Deserialize(string configPath)
        {
            try
            {
                string json = File.ReadAllText(configPath);
                var decrypted = EncryptionHelper.Decrypt(json);
                var cfg = JsonSerializer.Deserialize<List<Configuration>>(decrypted);

                return cfg ?? throw new ArgumentNullException(nameof(Configuration), @"Configuration file is empty");
            }
            catch (Exception e)
            {
                ErrorHelper.HandleError(e.Message);
                return new List<Configuration>();
            }
        }
    }

    public class Configuration
    {
        public string Id { get; set; }
        public string Tag { get; set; }
        public string FolderPath { get; set; }
        public Awssettings AwsSettings { get; set; }
    }

    public class Awssettings
    {
        public string Tag { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
    }


}
