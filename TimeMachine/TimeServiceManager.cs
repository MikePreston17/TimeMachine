﻿using DesignPatterns;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;

namespace TimeMachine
{
    //Adapted from: https://www.codeproject.com/Tips/703289/How-to-Control-a-Windows-Service-from-Code
    internal class WindowsTimeService : /*ServiceController,*/ IServiceController, ISingleton //todo: see if you can't just inherit from ServiceController
    {
        private const string serviceName = "Windows Time";
        private const int timeoutMilliseconds = 3600;
        private readonly static ServiceController _serviceMonitor = new ServiceController(serviceName);
        private TimeSpan _defaultStartupWaitTime = new TimeSpan(0, 0, 0, 0, 10);
        private const string serviceKeyName = @"SYSTEM\CurrentControlSet\Services\W32Time";

        public ServiceControllerStatus Status => _serviceMonitor.Status;
        public bool IsRunning => _serviceMonitor.Status == ServiceControllerStatus.Running;
        public bool IsStopped => _serviceMonitor.Status == ServiceControllerStatus.Stopped;
        public bool IsDisabled => CheckTimeServiceEnabled();

        private WindowsTimeService() { }

        public static WindowsTimeService Instance => Singleton<WindowsTimeService>.Instance;

        public bool RegistryValueExists(string hive_HKLM_or_HKCU, string registryRoot, string valueName)
        {
            RegistryKey root;
            switch (hive_HKLM_or_HKCU.ToUpper())
            {
                case "HKLM":
                    root = Registry.LocalMachine.OpenSubKey(registryRoot, false);
                    break;
                case "HKCU":
                    root = Registry.CurrentUser.OpenSubKey(registryRoot, false);
                    break;
                default:
                    throw new InvalidOperationException("parameter registryRoot must be either \"HKLM\" or \"HKCU\"");
            }

            return root.GetValue(valueName) != null;
        }

        /// <summary>
        /// * 0 = Boot
        ///* 1 = System
        ///* 2 = Automatic
        ///* 3 = Manual
        ///* 4 = Disabled
        /// </summary>
        public void Enable()
        {
            try
            {
                if (!IsDisabled)
                {
                    return;
                }

                using (var hkeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
                using (var key = Registry.LocalMachine.OpenSubKey(serviceKeyName, true))
                {
                    // key now points to the 64-bit key
                    if (key == null)
                    {
                        throw new Exception($"Could not find service {serviceName}");
                    }

                    key.SetValue("Start", 3);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not enable the service, error: " + e.Message);
            }
        }

        public void Disable()
        {
            try
            {
                if (IsRunning)
                {
                    Stop();
                }

                var key = Registry.LocalMachine.OpenSubKey(serviceKeyName, true);

                if (key == null)
                {
                    throw new Exception($"Could not find service {serviceName}");
                }

                key.SetValue("Start", 4);
            }
            catch (Exception e)
            {
                throw new Exception("Could not disable the service, error: " + e.Message);
            }
        }

        public void Start()
        {
            try
            {
                if (IsRunning)
                {
                    return;
                }

                if (IsDisabled)
                {
                    Enable();
                }

                if (_serviceMonitor.Status != ServiceControllerStatus.Running
                    || _serviceMonitor.Status != ServiceControllerStatus.StartPending)
                {
                    _serviceMonitor.Start();
                }

                _serviceMonitor.WaitForStatus(ServiceControllerStatus.Running, _defaultStartupWaitTime);
            }
            catch
            {
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                if (IsRunning)
                {
                    _serviceMonitor.Stop();
                    _serviceMonitor.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 1, 0));
                }
            }
            catch
            {
                throw;
            }
        }

        public void Restart()
        {
            try
            {
                Stop();
                Start();
            }
            catch
            {
                throw;
            }
        }

        private string FindWinTimeKeyName()
        {
            string keyName = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\";

            using (var hkeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
            using (var key = hkeyLocalMachine.OpenSubKey(keyName))
            {
                foreach (string name in key.GetSubKeyNames())
                {
                    Debug.WriteLine(name);
                }
            }

            return keyName;
        }

        private bool CheckTimeServiceEnabled()
        {
            try
            {
                string query = string.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", _serviceMonitor.ServiceName);
                var querySearch = new ManagementObjectSearcher(query);
                var services = querySearch.Get();
                // Since we have set the servicename in the constructor we asume the first result is always
                // the service we are looking for
                foreach (var service in services.Cast<ManagementObject>())
                {
                    return Convert.ToString(service.GetPropertyValue("StartMode")) == "Disabled";
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

    }
}
