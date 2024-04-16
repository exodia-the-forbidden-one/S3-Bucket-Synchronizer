using S3Uploader.Helpers;

namespace S3Uploader
{
    public class FileObject
    {
        public string FilePath { get; set; }
        public string BaseFolder { get; set; }
        public Awssettings AwsSettings { get; set; }
    }
}
