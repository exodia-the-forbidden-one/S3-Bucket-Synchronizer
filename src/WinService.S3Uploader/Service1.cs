using S3Uploader.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace S3Uploader
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {
            LogHelper logHelper = LogHelper.Instance;
            QueueService queue = QueueService.Instance;

            string cfgPath = Path.Combine(@"C:\S3FileUploader", "CFG");

            List<Configuration> configurations;

            checkFile:
            if (File.Exists(cfgPath))
            {
                configurations = JsonHelper.Deserialize(cfgPath);
            }
            else
            {
                ErrorHelper.HandleError($" Configuration file not found! ({cfgPath})");
                Task.Delay(TimeSpan.FromSeconds(60)).Wait();
                goto checkFile;
            }

            logHelper.Information("Service started!");


            foreach (var configuration in configurations)
            {
                Initial initial = new Initial
                {
                    PathToWatch = configuration.FolderPath,
                    QueueService = queue,
                    AwsSettings = configuration.AwsSettings
                };

                // detect file differences
                var s3ObjList = await initial.GetS3Files();
                var localObjList = initial.GetLocalFiles(configuration.FolderPath);

                await initial.GetDifferences(localObjList, s3ObjList);

                // Creating a FileSystemWatcher
                EnhancedFileSystemWatcher watcher = new EnhancedFileSystemWatcher
                {
                    Path = configuration.FolderPath, // Folder to be watched
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName, // Determines which types of file changes to watch 
                    IncludeSubdirectories = true // Determines if subdirectories within the specified path should be monitored
                };

                // Setting up event listeners
                watcher.Created += OnChanged;   // Triggered when a file is created
                watcher.Changed += OnChanged;   // Triggered when a change occurs on a file
                watcher.Renamed += OnChanged;   // Triggered when a file is renamed

                // Starting to listen for events
                watcher.EnableRaisingEvents = true;

                async void OnChanged(object sender, FileSystemEventArgs e)
                {
                    try
                    {
                        if (File.Exists(e.FullPath))
                        {
                            var fo = new FileObject()
                            {
                                FilePath = e.FullPath,
                                BaseFolder = configuration.FolderPath,
                                AwsSettings = configuration.AwsSettings
                            };
                            await queue.AddAsync(fo);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorHelper.HandleError("An error occured when adding to the queue: " + exception.Message);
                    }
                }
            }

            queue.StartQueueAsync();
        }

        protected override void OnStop()
        {
            ErrorHelper.HandleError("Service Stopped.");
        }
    }
}
