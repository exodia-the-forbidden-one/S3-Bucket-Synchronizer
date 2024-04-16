namespace S3Uploader.Helpers
{
    public static class ErrorHelper
    {
        public static void HandleError(string message)
        {
            try
            {
                LogHelper logHelper = LogHelper.Instance;

                logHelper.Error(message);

                //send email falan filan
            }
            catch
            {
                return;
            }
        }
    }
}
