using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnsysLaunch
{
    internal class Program
    {
        private static string AppPath = Path.GetDirectoryName(Application.ResourceAssembly.CodeBase.ToString()).Replace("file:\\", "");

        static void Main(string[] args)
        {
            var working = Properties.Settings.Default.WorkingPath;
            var lmgrd_cmd = Properties.Settings.Default.LicensenServerPath;
            var lmgrd_opts = Properties.Settings.Default.LicensenServerOptions;
            var prod_cmd = Properties.Settings.Default.ProductPath;
            var prod_opts = Properties.Settings.Default.ProductOptions;

            if (!Path.IsPathRooted(working)) working = Path.GetFullPath(working);
            if (!Path.IsPathRooted(lmgrd_cmd)) lmgrd_cmd = Path.GetFullPath(lmgrd_cmd);
            if (!Path.IsPathRooted(prod_cmd)) prod_cmd = Path.GetFullPath(prod_cmd);

            if (!string.IsNullOrEmpty(lmgrd_cmd) && File.Exists(lmgrd_cmd))
            {
                Process lmgrd = ProcessExists(lmgrd_cmd, lmgrd_opts);
                if (!(lmgrd is Process)) lmgrd = Run(lmgrd_cmd, lmgrd_opts, working: working);

                if (lmgrd is Process)
                {
                    if (!string.IsNullOrEmpty(prod_cmd) && File.Exists(prod_cmd))
                    {
                        Process prod = ProcessExists(prod_cmd, prod_opts);
                        if (!(prod is Process)) prod = Run(prod_cmd, prod_opts, working: working, waiting: true);
                        if (prod is Process && prod.HasExited && lmgrd is Process) lmgrd.Kill();
                    }
                }
            }
        }

        static bool IsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static Process ProcessExists(string cmd, string opts)
        {
            Process result = null;
            var pname = Path.GetFileNameWithoutExtension(cmd);

            //var plist = Process.GetProcessesByName(pname);
            //foreach (var p in plist)
            //{
            //    var pinfo = p.StartInfo;
            //    if (pinfo.FileName.Equals(cmd, StringComparison.CurrentCultureIgnoreCase) && pinfo.Arguments.Equals(opts, StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        result = p;
            //        break;
            //    }
            //}

#if DEBUG
            string wmiQueryString = $"SELECT * FROM Win32_Process WHERE Name LIKE '%{pname}%'";
#else
            string wmiQueryString = $"SELECT Name,ExecutablePath,CommandLine,ProcessID FROM Win32_Process WHERE Name LIKE '%{pname}%'";
#endif
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", wmiQueryString))
            {
                try
                {
                    foreach (var mo in searcher.Get().Cast<ManagementObject>())
                    {
                        if (mo is ManagementObject)
                        {
                            var mo_pid = (uint)mo["ProcessID"];
                            var mo_name = (string)mo["Name"];
                            var mo_path = (string)mo["ExecutablePath"];
                            var mo_args = (string)mo["CommandLine"];
                            if (mo_name.Equals(cmd, StringComparison.CurrentCultureIgnoreCase) && mo_args.Equals(opts, StringComparison.CurrentCultureIgnoreCase))
                            {
                                result = Process.GetProcessById((int)mo_pid);
                                break;
                            }
                        }
                    }
                }
                catch { }
            }            
            return (result);
        }

        public static Process Run(string cmd, string[] args = null, string working = null, bool hidden = false, bool waiting = false)
        {
            var opts = args is string[] && args.Length > 0 ? string.Join(" ", args) : string.Empty;
            return (Run(cmd, opts, working, hidden, waiting));
        }

        public static Process Run(string cmd, string opts = null, string working = null, bool hidden = false, bool waiting = false)
        {
            Process process = null;
            if (string.IsNullOrEmpty(cmd)) return (null);
            if (!Path.IsPathRooted(cmd)) cmd = Path.Combine(AppPath, cmd);
            if (File.Exists(cmd))
            {
                //Console.WriteLine(file);
                var process_info = new ProcessStartInfo();
                process_info.FileName = cmd;
                process_info.Arguments = opts;
                //process_info.CreateNoWindow = hidden ? true : false;
                //process_info.WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;
                process_info.WorkingDirectory = string.IsNullOrEmpty(working) && Directory.Exists(working) ? AppPath : working;
                process = new Process() { StartInfo = process_info };
                if (process.Start())
                {
                    if (waiting) process.WaitForExit();
                    //else process.ex
                }
            }
            return (process);
        }
    }
}
