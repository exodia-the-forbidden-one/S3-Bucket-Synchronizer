using System;
using System.ServiceProcess;

namespace Uploader_UI.Helpers;

public class WindowsServiceHelper
{
    public static void StartService(string serviceName)
    {
        try
        {
            ServiceController service = new(serviceName);
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
        }
        catch(Exception ex)
        {
            throw new ArgumentException();
        }
    }

    public static void StopService(string serviceName)
    {
        ServiceController service = new(serviceName);
        if (service.Status == ServiceControllerStatus.Running)
        {
            service.Stop();
        }
    }

    public static void RestartService(string serviceName)
    {
        try
        {
            ServiceController service = new(serviceName);

            if (service.Status == ServiceControllerStatus.Running)
            {
                // Servis durdurma
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);
            }

            // Servis başlatma
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
        }
        catch
        {

        }
    }

    public static bool IsServiceRunning(string serviceName)
    {

        try
        {
            ServiceController service = new(serviceName);
            return service.Status == ServiceControllerStatus.Running;
        }
        catch
        {
            return false;
        }
        

    }
}