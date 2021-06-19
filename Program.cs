using MyDirectory;
using OutConsole;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace FileManager_CP
{
    class Program
    {

        /// <summary>
        /// метод управления выводом на экран списка папок / файлов
        /// если пользователь нажимает кнопку ENTER, программа входит в режим ввода команд
        /// </summary>
        /// <param name="args"></param>
        static void Manager(string[] args)
        {
            var Setting = new Settings();
            var OC = new OutCons();
            OC.pageCount = Setting.Page;
            OC.mDir = new MyDir(Setting.Path);
            OC.Show();
            var k = new ConsoleKey();
            do
            {
                k = Console.ReadKey(true).Key;
                switch (k)
                {
                    case ConsoleKey.Enter: // запускается командный режим, пользователь вводит команды
                        OC.commandLine = string.Empty;
                        OC.Show();
                        Console.Write("Введите команду: ");
                        var com = new Command(Console.ReadLine().Split(' '), ref OC.mDir);
                        OC.commandLine = com.ToString();
                        break;
                    case ConsoleKey.Home:
                        OC.Home();
                        break;
                    case ConsoleKey.End:
                        OC.End();
                        break;
                    case ConsoleKey.PageDown:
                        OC.PageDown();
                        break;
                    case ConsoleKey.PageUp:
                        OC.PageUp();
                        break;
                    case ConsoleKey.UpArrow:
                        OC.Prev();
                        break;
                    case ConsoleKey.DownArrow:
                        OC.Next();
                        break;
                    default:
                        break;
                }
                OC.Show();
            }
            while (k != ConsoleKey.Escape);
        }

        /// <summary>
        /// метод проверяет права пользователя
        /// если пользователь не администратор, возвращается false
        /// </summary>
        /// <returns></returns>
        static bool IsUserAdmin()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch 
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        /// <summary>
        /// метод перезапускает процесс от имени администратора
        /// </summary>
        /// <param name="args"></param>
        static void RunAsAdmin(string[] args)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.Arguments = string.Join(" ", args);
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.FileName = Process.GetCurrentProcess().MainModule.FileName;
            proc.Verb = "runas";
            try
            {
                Process.Start(proc);
            }
            catch
            {

            }
        }

        static void Main(string[] args)
        {
            if (!IsUserAdmin())
            {
                RunAsAdmin(args);
            }

            Manager(args);
        }
    }
}
