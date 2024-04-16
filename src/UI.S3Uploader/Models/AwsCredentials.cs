using System;
using System.Collections.Generic;

namespace Uploader_UI.Models;

public class AwsCredentials
{
    public Guid Id { get; set; }
    public string Tag { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Region { get; set; }
    public List<string> Buckets { get; set; }
}