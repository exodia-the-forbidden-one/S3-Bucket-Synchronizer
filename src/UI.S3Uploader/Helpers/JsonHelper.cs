using System;
using System.Text.Json;

namespace Uploader_UI.Helpers
{
    internal class JsonHelper
    {

        public static string Serialize<T>(T type)
        {
            try
            {
                string json = JsonSerializer.Serialize(type, new JsonSerializerOptions { WriteIndented = true });
                return json;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}