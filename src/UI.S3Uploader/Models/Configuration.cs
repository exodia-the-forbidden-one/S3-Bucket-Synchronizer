using System;

namespace Uploader_UI.Models;

public class Configuration
{
    public Guid Id { get; set; }
    public string Tag { get; set; }
    public string FolderPath { get; set; }
    public AwsSettings AwsSettings { get; set; }
}

public class AwsSettings
{
    public string Tag { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Region { get; set; }
    public string BucketName { get; set; }
}