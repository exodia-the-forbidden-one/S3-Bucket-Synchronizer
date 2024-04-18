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
        public readonly Channel<FileObject> Queue;
        private readonly LogHelper _logHelper;

        public static QueueService Instance => instance.Value;

        private QueueService()
        {
            BoundedChannelOptions options = new BoundedChannelOptions(Int32.MaxValue)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _logHelper = LogHelper.Instance;
            Queue = Channel.CreateBounded<FileObject>(options);
        }

        public async ValueTask AddAsync(FileObject fileObj)
        {
            try
            {
                await Queue.Writer.WriteAsync(fileObj);
                _logHelper.Information("File added to queue. Path: " + fileObj.FilePath + " - S3 Bucket Name: " + fileObj.AwsSettings.BucketName);
            }
            catch (Exception e)
            {
                ErrorHelper.HandleError(e.Message);
            }
        }

        public async ValueTask<FileObject> GetAsync()
        {
            var item = await Queue.Reader.ReadAsync();
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
                    _logHelper.Warning($"File doesn't exist: {fileObj.FilePath}");
                    continue;
                }

                Awssettings awsSettings = fileObj.AwsSettings;
                FileInfo info = new FileInfo(fileObj.FilePath);
                _logHelper.Information($"Upload started: {fileObj.FilePath} ({info.Length} bytes)");

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
                    Key = relativeUri.ToString().Replace("\\", "/")
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
                        _logHelper.Information("File uploaded successfully. File path: " + fileObj.FilePath);
                    }
                    catch (AmazonS3Exception ex)
                    {
                        ErrorHelper.HandleError($"AWS Server error: '{ex.Message}'");
                    }
                    catch
                    {
                        if (!NetworkHelper.IsNetworkAvailable())
                        {
                            _logHelper.Error("No network connection. Reconnecting...");

                            while (!NetworkHelper.IsNetworkAvailable())
                            {
                                await Task.Delay(TimeSpan.FromSeconds(10));
                            }
                        }
                        else
                        {
                            _logHelper.Warning($"No access to file. Adding to the queue again...");
                            await Task.Delay(3000);
                        }


                        if (File.Exists(fileObj.FilePath))
                        {
                            try
                            {
                                await AddAsync(fileObj);
                            }
                            catch (Exception exception)
                            {
                                ErrorHelper.HandleError("An error occurred while adding to the queue: " + exception.Message);
                            }
                        }
                        else
                        {
                            ErrorHelper.HandleError("The file may have been deleted. Removed from the queue: " + fileObj.FilePath);
                        }
                    }

                    if (Queue.Reader.Count == 0)
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
