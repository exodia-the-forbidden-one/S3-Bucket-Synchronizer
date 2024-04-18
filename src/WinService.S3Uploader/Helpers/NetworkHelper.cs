using System.Net.NetworkInformation;

namespace S3Uploader.Helpers
{
    public class NetworkHelper
    {
        public static bool IsNetworkAvailable()
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send("www.google.com");
                return reply != null && (reply.Status == IPStatus.Success);
            }
            catch (PingException)
            {
                return false;
            }
        }
    }
}
