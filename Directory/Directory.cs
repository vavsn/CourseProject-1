using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace MyDirectory
{
    public class MyDir
    {
//        string dirName = string.Empty;
        public DirectoryInfo curentDir; // текущая папка
        public FileSystemInfo[] subDir;
        public string[] Folders; // полный список директорий и файлов   

        private Int64 GetDirectorySize(string folderPath)
        {
            Int64 currentSize;
            try
            {
                var files = Directory.EnumerateFiles(folderPath);
                // get the sizeof all files in the current directory
                currentSize = (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();
            }
            catch
            {
                currentSize = 0;
            }

            Int64 subDirSize;
            try
            {
                var directories = Directory.EnumerateDirectories(folderPath);

                // get the size of all files in all subdirectories
                subDirSize = (from directory in directories select GetDirectorySize(directory)).Sum();
            }
            catch
            {
                subDirSize = 0;
            }

            return currentSize + subDirSize;
        }

        public Int64 GetSize(string name)
        {
            try
            {
                if (string.Compare(new DirectoryInfo(name).Root.ToString(), name) == 0) // если параметром передан диск, привлекаем стандартный механизм определения размера
                {
                    return new DriveInfo(new DirectoryInfo(name).Root.ToString()).TotalSize;
                }
                else
                {
                    FileAttributes attr = File.GetAttributes(name);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        return GetDirectorySize(name);
                    }
                    else
                    {
                        return new FileInfo(name).Length;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public bool Change = false; // указатель на какие-то изменения с объектом

        private long size;

        public long Size
        {
            get { return size; }
        }

        /// <summary>
        ///  метод показывает размер папки / файла в Кб, Мб, Гб в зависимости от значения свойства size
        /// </summary>
        public string SpecSize
        {
            get 
            {
                string[] raz = new string[] { "байт", "Кб", "Мб", "Гб", "Тб"};
                int i = 0;
                long s = size;
                while (s / 1024 > 1000)
                {
                    s = s / 1024;
                    i++;
                }
                return s.ToString() + " "  + raz[i]; 
            }
        }

       // public FileSystemInfo DirInfo;
        public FileSystemInfo Info 
        { 
            get 
            {
                if (subDir is null)
                    return null;
                return subDir.Where(l => (string.Compare(l.FullName, Folders[curPos]) == 0)).FirstOrDefault();
            }
        }

        public bool Minus; // направление движения курсора

        public string FullName // полное имя 
        {
            get
            {
                if (curentDir is null)
                    return string.Empty;
                return curentDir.FullName;
            }
        }
        public string Root // корневой каталог
        {
            get
            {
                if (curentDir is null)
                    return string.Empty;
                return curentDir.Root.FullName;
            }
        }
        public string Parent // корневой каталог
        {
            get
            {
                if (curentDir is null)
                    return string.Empty;
                if (curentDir.Parent is null)
                    return string.Empty;
                return curentDir.Parent.FullName;
            }
        }
        public override string ToString() 
        { 
            return InfoN(curPos); 
        }

        private int curPos;
        public int CurPos
        {
            get
            {
                return curPos;
            }
            set
            {
                Minus = value < curPos ? true : false;
                if (subDir is null | value < 0)
                    curPos = 0;
                else
                    if (value > subDir.Length - 1)
                        curPos = subDir.Length - 1;
                    else curPos = value;
                size = GetSize(Folders[curPos]); // узнаем размер папки / файла
            }
        }

        /// <summary>
        /// формирование объекта, в котором хранится информация по директории / поддиректории
        /// </summary>
        /// <param name="dn"></param>
        public MyDir(string dn)
        {
            try
            {
                var drive = DriveInfo.GetDrives().Where(d => d.IsReady).FirstOrDefault().ToString();
                string s1 = !string.IsNullOrEmpty(dn) ? dn : drive;
                curentDir = new DirectoryInfo(s1.EndsWith(@"\") ? s1 : string.Concat(s1, @"\"));

                if (!curentDir.Exists)
                {
                    try
                    {
                        curentDir = new DirectoryInfo(drive);
                    }
                    catch 
                    {

                    }
                }
            }
            catch 
            {

            }

            subDir = curentDir.GetFileSystemInfos();
            string[] Dirs = Directory.GetDirectories(curentDir.FullName); // список поддиректорий
            Array.Sort(Dirs);
            string[] Files = Directory.GetFiles(curentDir.FullName); // список файлов в директории
            Array.Sort(Files);
            Folders = Dirs.Concat(Files).Distinct().ToArray();
            CurPos = 0;

        }
        /// <summary>
        /// метод обновляет информацию в объекте
        /// </summary>
        /// <returns></returns>
        public MyDir Refresh()
        {
            return new MyDir(curentDir.FullName);
        }

        /// <summary>
        /// возврат данных по текущей позиции в списке директорий / файлов
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        private string InfoN(int cp)
        {
            if (subDir is null)
                return string.Empty;
            string ret = string.Empty;
            FileSystemInfo fi = subDir.Where(l => (string.Compare(l.FullName, Folders[cp]) == 0)).FirstOrDefault();
            return fi.FullName.Replace(fi.Name, "");
        }
    }
}