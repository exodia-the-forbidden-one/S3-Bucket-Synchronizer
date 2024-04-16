using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using S3Uploader.Helpers;
using System;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace S3Uploader
{
    public class QueueService
    {

        private static readonly Lazy<QueueService> instance = new Lazy<QueueService>(() => new QueueService());
        public readonly Channel<FileObject> _queue;
        private readonly LogHelper _logHelper;

        public static QueueService Instance => instance.Value;

        private QueueService()
        {
            BoundedChannelOptions options = new BoundedChannelOptions(Int32.MaxValue)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _logHelper = LogHelper.Instance;
            _queue = Channel.CreateBounded<FileObject>(options);
        }

        public async ValueTask AddAsync(FileObject fileObj)
        {
            try
            {
                await _queue.Writer.WriteAsync(fileObj);
                _logHelper.Information("Dosya kuyruğa eklendi. Dosya yolu: " + fileObj.FilePath + " - S3 Bucket Name: " + fileObj.AwsSettings.BucketName);
            }
            catch (Exception e)
            {
                ErrorHelper.HandleError(e.Message);
            }
        }

        public async ValueTask<FileObject> GetAsync()
        {
            var item = await _queue.Reader.ReadAsync();
            return item;
        }

        public async Task StartQueueAsync()
        {
            while (true)
            {
                // waits if queue is empty
                var fileObj = await GetAsync();

                if (!File.Exists(fileObj.FilePath))
                {
                    _logHelper.Warning($"file doesnt exist: {fileObj.FilePath}");
                    continue;
                }

                Awssettings awsSettings = fileObj.AwsSettings;
                FileInfo info = new FileInfo(fileObj.FilePath);
                _logHelper.Information($"Yükleme işlemi başladı: {fileObj.FilePath} ({info.Length} bytes)");

                var credentials = new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey);
                var config = new AmazonS3Config
                {
                    RegionEndpoint = AWSHelper.GetRegionEndpoint(awsSettings.Region)
                };

                Uri currentUri = new Uri(fileObj.BaseFolder);
                Uri targetUri = new Uri(fileObj.FilePath);
                Uri relativeUri = currentUri.MakeRelativeUri(targetUri);

                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    FilePath = fileObj.FilePath,
                    CannedACL = S3CannedACL.NoACL,
                    BucketName = awsSettings.BucketName,
                    Key = relativeUri.ToString().Replace("\\", "/") // Replace backslashes with forward slashes
                };

                //uploadRequest.UploadProgressEvent += (sender, e) =>
                //{
                //    LogHelper.Log($"Uploaded {e.TransferredBytes} bytes out of {e.TotalBytes}");
                //    LogHelper.Log($"Progress: {e.PercentDone}%");
                //};

                using (var client = new AmazonS3Client(credentials, config))
                {
                    var transferUtility = new TransferUtility(client);

                    try
                    {
                        await transferUtility.UploadAsync(uploadRequest);
                        _logHelper.Information("Yükleme başarıyla tamamlandı: " + fileObj.FilePath);
                    }
                    catch (AmazonS3Exception ex)
                    {
                        ErrorHelper.HandleError($"AWS sunucusu ile ilgili bir hata oluştur: '{ex.Message}'");
                    }
                    catch (Exception ex)
                    {
                        _logHelper.Warning($"Dosyaya erişim yok. Tekrar kuyruğa ekleniyor...");
                        await Task.Delay(3000);

                        if (File.Exists(fileObj.FilePath))
                        {
                            try
                            {
                                await AddAsync(fileObj);
                            }
                            catch (Exception exception)
                            {
                                ErrorHelper.HandleError("Kuyruğa ekleme sırasında bir hata oluştu: " + exception.Message);
                            }
                        }
                        else
                        {
                            ErrorHelper.HandleError("Dosya silinmiş olabilir. Kuyruktan çıkarıldı: " + fileObj.FilePath);
                        }
                    }

                    if (_queue.Reader.Count == 0)
                    {
                        Initial initial = new Initial
                        {
                            PathToWatch = fileObj.BaseFolder,
                            QueueService = Instance,
                            AwsSettings = awsSettings
                        };

                        var s3ObjList = await initial.GetS3Files();
                        var localObjList = initial.GetLocalFiles(fileObj.BaseFolder);

                        await initial.GetDifferences(localObjList, s3ObjList);
                    }
                }
            }
        }
    }
}
