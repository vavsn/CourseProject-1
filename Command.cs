using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.IO;
using MyDirectory;
using System.Security;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace FileManager_CP
{
    /// <summary>
    /// класс для работы с командами
    /// </summary>
    public class Command
    {

		public string[] Args;
		private void SetArgs(MyDir myDir)
		{
			
		}
		private string res;
		public override string ToString()
		{ 
			return res; 
		}

		/// <summary>
		/// метод анализа строки аргументов. выявляет и нормализует аргументы, ограниченные знаком ". например длинные имена папок вида "c:\Documents and settings"
		/// проверяет наличие знака ". если количество знаков нечетное - возвращает ошибку "Неверный аргумент"
		/// если количество четное - возвращает массив строк, в каждом элементе которого нормированный аргумент
		/// </summary>
		/// <param name="args"></param>
		public bool analizeArgs(ref string[] args)
		{
			if (args.Length == 0)
			{
				res = "Не указаны аргументы";
				return false;
			}

			var s1 = string.Join(" ", args);
			int count = s1.Count(f => f == '\"');

			if (count != 0)
			{
				if (count % 2 == 1)
				{
					res = "Указан неверный аргумент";
					return false;
				}
				Regex r = new Regex("\"(.+?)\"");

				var res1 = r.Matches(s1).Cast<Match>().Select(m => m.Groups[0].Value).ToArray();

				int i = 0;
				foreach (var s in res1)
				{
					s1 = s1.Replace(s, i.ToString());
					i++;
				}

				string[] args1 = s1.Split(' ');

				i = 0;
				foreach (var s in s1.Split(' '))
				{
					if (Int32.TryParse(s, out int j))
						args1[i] = res1[j].Replace("\"", "");
					i++;
				}

				args = args1;
			}

			return true;
		}
		
		public Command(string[] args, ref MyDir myDir)
		{
			if (!analizeArgs(ref args))
				return;

			var command = args[0];

			switch (command)
			{
				case "exit":
					Exit(ref myDir);
					break;
				case "cd":
					CD(args, ref myDir);
					break;
				case "del":
					Del(args);
					myDir = myDir.Refresh();
					break;
				case "cp" when args.Length == 3:
					Copy(args);
					myDir = myDir.Refresh();
					break;
				default:
					res = "Указана неверная команда";
					break;
			}

		}

		private void DelDirectory(string SourceDirectory)
		{
			try
			{
				var source = new DirectoryInfo(SourceDirectory);
				// удаляем директорию
				source.Delete(true);
			}
			catch (UnauthorizedAccessException)
			{
				res = "Каталог содержит файл только для чтения";
			}
			catch (DirectoryNotFoundException)
			{
				res = "Папка не существует или не найдена";
			}
			catch (SecurityException)
			{
				res = "Отсутствует разрешение на удаление";
			}
			catch (IOException)
			{
				res = "Каталог доступен только для чтения либо содержит подпапки / файлы только для чтения.";
			}
			catch (Exception ex)
			{
				res = "Ошибка: " + ex.Message;
			}
		}

		private void DelFile(string SourceName)
		{
			try
			{
				FileInfo fi = new FileInfo(SourceName);
				//удаляем файл
				fi.Delete();
			}
			catch (UnauthorizedAccessException)
			{
				res = "Вместо имени файла указано имя папки";
			}
			catch (SecurityException)
			{
				res = "Отсутствует разрешение на удаление";
			}
			catch (IOException)
			{
				res = "Файл открыт либо используется в другом процессе";
			}
			catch (Exception ex)
			{
				res = "Ошибка: " + ex.Message;
			}
		}

		/// <summary>
		/// метод определяем, что удаляем папкиу или файл.
		/// соответственно запускается метод удаления папки или файла
		/// </summary>
		/// <param name="args"></param>
		public void Del(string[] args)
		{
			try
			{
				// проверим источник - это папка?
				// если да - удаляем папку

				FileAttributes attr = File.GetAttributes(args[1]);
				if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
				{
					DelDirectory(args[1]);
				}
				else
				{
					DelFile(args[1]);
				}
			}
			catch (UnauthorizedAccessException)
			{
				res = "Отсутствуют необходимые разрешения";
			}
			catch (NotSupportedException)
			{
				res = "Параметр задан в недопустимом формате";
			}
			catch (FileNotFoundException)
			{
				res = "Недопустимое имя файла либо файл не найден";
			}
			catch (DirectoryNotFoundException)
			{
				res = "Недопустимое имя папки либо папка не найдена";
			}
			catch (PathTooLongException)
			{
				res = "Параметр превышет максимальную длину, заданную в системе";
			}
			catch (ArgumentException)
			{
				res = "Параметр является пустой строкой или содержит недопустимые символы";
			}
			catch (IOException)
			{
				res = "Файл используется другим процессом";
			}
			catch (Exception ex)
			{
				res = "Ошибка: " + ex.Message;
			}
		}
		/// <summary>
		/// метод перехода между дисками / папками
		/// </summary>
		/// <param name="args"></param>
		/// <param name="myDir"></param>
		public void CD(string[] args, ref MyDir myDir)
		{
			string s1 = args.Length == 2 ? args[1] : string.Join(" ", args.Where((val, idx) => idx != 0).ToArray());

			res = "Директория " + s1 + " не существует";
			if (s1.Contains(@".\"))
			{
				string s2 = s1.Replace(@".\", "");
				s1 = string.Concat(string.IsNullOrEmpty(myDir.FullName) ? myDir.Root : myDir.FullName, s2);
			}
			if (s1.Contains(@".."))
				s1 = myDir.Root;
			try
			{
				if ((new DirectoryInfo(s1)).Exists)
				{
					try
					{
						myDir = new MyDir(s1);
						res = "Успешно";
					}
					catch (Exception ex)
					{
						res = "Ошибка: " + ex.Message;
					}
				}
			}
			catch (ArgumentNullException)
			{
				res = "Указан пустой путь к папке / файлу";
			}
			catch (SecurityException)
			{
				res = "Отсутствует доступ к папке / файлу";
			}
			catch (UnauthorizedAccessException)
			{
				res = "Отсутствует доступ к папке / файлу";
			}
			catch (ArgumentException)
			{
				res = "Недопустимые символы";
			}
			catch (PathTooLongException)
			{
				res = "Указанный путь, имя файла или оба значения превышают максимальную длину, заданную в системе.";
			}
			catch (Exception ex)
			{
				res = "Ошибка: " + ex.Message;
			}

		}
		/// <summary>
		/// метод завершает работу программы
		/// </summary>
		/// <param name="myDir"></param>
		public void Exit(ref MyDir myDir)
		{
			var Setting = new Settings();
			Setting.Path = myDir.FullName;
			Setting.PutSettings();
			Environment.Exit(0);
		}
		/// <summary>
		/// метод проверяет достаточно ли места на диске назначения
		/// </summary>
		/// <returns></returns>
		public bool IsEnougth(string SourceDir, string TargetDir)
		{
			var td = new DriveInfo(new DirectoryInfo(TargetDir).Root.ToString()).TotalFreeSpace;
			var sd = new MyDir(SourceDir).GetSize(SourceDir);

			return sd < td;
		}
		/// <summary>
		/// метод копирует директорию
		/// </summary>
		/// <param name="SourceDirectory"></param>
		/// <param name="TargetDirectory"></param>
		private void CopyDirectory(string SourceDirectory, string TargetDirectory)
		{
			try
			{
				DirectoryInfo source = new DirectoryInfo(SourceDirectory);
				DirectoryInfo target = new DirectoryInfo(TargetDirectory);
				// проверим достаточно ли места на устройстве-приёмнике
				if (!IsEnougth(SourceDirectory, TargetDirectory))
				{
					res = "Не достаточно места для копирования";
					return;
				}
				// проверим существует ли папка-источник
				if (!source.Exists)
				{
					res = "Папка " + SourceDirectory + " не существует";
					return;
				}
				// проверим существует ли папка-приёмник. если нет - создаём. если есть - показываем предупреждение
				if (!target.Exists)
					target.Create();
				else
				{
					res = "Папка " + TargetDirectory + " уже существует. Копирование невозможно.";
					return;
				}

				//Copy files.
				FileInfo[] sourceFiles = source.GetFiles();
				for (int i = 0; i < sourceFiles.Length; ++i)
					File.Copy(sourceFiles[i].FullName, target.FullName + "\\" + sourceFiles[i].Name, true);

				//Copy directories.
				DirectoryInfo[] sourceDirectories = source.GetDirectories();
				for (int j = 0; j < sourceDirectories.Length; ++j)
					CopyDirectory(sourceDirectories[j].FullName, target.FullName + "\\" + sourceDirectories[j].Name);
			}
			catch (ArgumentNullException)
			{
				res = "Указан пустой путь к папке / файлу";
			}
			catch (SecurityException)
			{
				res = "Отсутствует доступ к папке / файлу";
			}
			catch (UnauthorizedAccessException)
			{
				res = "Отсутствует доступ к папке / файлу";
			}
			catch (ArgumentException)
			{
				res = "Недопустимые символы";
			}
			catch (PathTooLongException)
			{
				res = "Указанный путь, имя файла или оба значения превышают максимальную длину, заданную в системе.";
			}
			catch (Exception ex)
			{
				res = "Ошибка: " + ex.Message;
			}
		}
		/// <summary>
		/// метод копирует файл
		/// </summary>
		/// <param name="SourceDirectory"></param>
		/// <param name="TargetDirectory"></param>
		private void CopyFile(string SourceName, string TargetName)
		{
			var targetPath = Path.GetDirectoryName(TargetName);
			var fileName = Path.GetFileName(TargetName) == string.Empty ? Path.GetFileName(SourceName) : Path.GetFileName(TargetName);
			var destFile = Path.Combine(targetPath, fileName);
			try
			{
				// проверим достаточно ли места на устройстве-приёмнике
				if (!IsEnougth(SourceName, TargetName))
				{
					res = "Не достаточно места для копирования";
					return;
				}
				File.Copy(SourceName, destFile, true);
			}
			catch(Exception ex)
			{
				res = "Ошибка: " + ex.Message;
			}
		}

		/// <summary>
		/// метод реакции на ввод команды копирования файла / директории
		/// </summary>
		/// <param name="args"></param>
		public void Copy(string[] args)
		{
			// добавим 
			// проверим источник - это папка?
			// если да - копируем папку
			FileAttributes attr = File.GetAttributes(args[1]);
			if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
			{
				CopyDirectory(args[1], args[2]);
			}
			else
			{
				CopyFile(args[1], args[2]);
			}
		}
	}

}
