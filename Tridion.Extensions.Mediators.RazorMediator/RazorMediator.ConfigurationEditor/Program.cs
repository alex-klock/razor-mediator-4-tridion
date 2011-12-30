using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RazorMediator.ConfigurationEditor
{
    class Program
    {
        static void Main(string[] args)
        {   
            TridionContentManagerConfigEditor editor = new TridionContentManagerConfigEditor();

            if (args.Length != 0 && args[0].Equals("u"))
            {
                editor.UnInstall();
            }
            else
            {
                editor.Install();
            }
        }

        static string GetTridionInstallPath()
        {
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Tridion");
            if (registryKey == null)
            {
                registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Tridion");
            }
            if (registryKey == null)
            {
                throw new Exception("Tridion Installation Path can not be found.");
            }

            return registryKey.GetValue("InstallDir").ToString();
        }
    }
}
