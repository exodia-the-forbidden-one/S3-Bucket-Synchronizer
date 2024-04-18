using System;
using System.IO;
using System.Text.Json;

namespace Uploader_UI.Helpers;

public class ConfigurationHelper
{
    public string ConfigPath { get; set; }

    public ConfigurationHelper(string configPath)
    {
        ConfigPath = configPath;
    }

    public bool SaveConfigurations(string content)
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                File.WriteAllText(ConfigPath, string.Empty);
            }
            else
            {
                File.Create(ConfigPath).Close();
            }

            var encrypted = EncryptionHelper.Encrypt(content);
            File.WriteAllText(ConfigPath, encrypted);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public T? GetConfigurations<T>() where T : new() 
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                string content = File.ReadAllText(ConfigPath);
                var decrypted = EncryptionHelper.Decrypt(content);
                return JsonSerializer.Deserialize<T>(decrypted);
            }
            return new T();
        }
        catch(Exception e)
        {
            return new T();
        }
    }
}