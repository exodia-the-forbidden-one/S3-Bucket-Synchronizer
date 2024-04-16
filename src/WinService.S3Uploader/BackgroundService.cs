using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using S3Uploader.Helpers;

namespace S3Uploader
{
    internal class BackgroundService : ServiceBase
    {

        protected override void OnStart(string[] args)
        {
            Timer timer = new Timer(Test, null, TimeSpan.Zero, TimeSpan.FromSeconds(3)); // Örneğin her 5 dakikada bir çalışacak şekilde ayarlandı
            while (true)
            {
                LogHelper.Log("BackgroundService started");
            }
        }

        protected override void OnStop()
        {
            // Hizmet durduğunda buraya giriş noktası
        }

        public static void Test(object state)
        {
            LogHelper.Log("BackgroundService running");
        }


    }
}
