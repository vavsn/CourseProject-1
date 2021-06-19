using MyDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OutConsole
{
    public class OutCons
    {
        private int LengthHorizont; // количество символов в строке
        private int HeightVertical; // количество строк на экране
        public MyDir mDir; // объект с данными по директориям
        private string[] line = new string[3] { string.Empty, string.Empty, string.Empty }; // переменные для формирования вывода на экран секции со свойствами директорий / файлов
        public string commandLine = string.Empty; // переменная для формирования вывода на экран командной строки
        private int firstPos = 0; // первая позиция страницы для вывода в списке директорий
        private int posForOut = 0; // текущая позиция курсора в списке директорий / файлов для подсветки
        public int pageCount; // количество строк для постраничного просмотра
        /// <summary>
        /// форматирование строки для корректного вывода, слишком длинные строки обрезаются
        /// </summary>
        /// <param name="locVal"></param>
        /// <returns></returns>
        string formLine(string locVal)
        {
            return locVal.Length > LengthHorizont ?
                locVal.Substring(0, 8) + ".." + locVal.Substring(locVal.Length - LengthHorizont + 10, locVal.Length) :
                locVal;
        }
        /// <summary>
        /// курсор позиционируется в начальном элементе списка папок / директорий
        /// </summary>
        public void Home()
        {
            mDir.CurPos = 0;
            mDir.Change = true;
        }
        /// <summary>
        /// курсор позиционируется на 1 позицию вниз по списку папок / директорий
        /// </summary>
        public void Next()
        {
            mDir.CurPos++;
            mDir.Change = true;
        }
        /// <summary>
        /// курсор позиционируется на 1 страницу вниз по списку папок / директорий. количество строк в странице указывается в свойствах проекта
        /// </summary>
        public void PageDown()
        {
            mDir.CurPos += pageCount;
            mDir.Change = true;
        }
        /// <summary>
        /// курсор позиционируется в конечном элементе списка папок / директорий
        /// </summary>
        public void End()
        {
            mDir.CurPos = mDir.subDir.Length;
            mDir.Change = true;
        }
        /// <summary>
        /// курсор позиционируется на 1 позицию вверх по списку папок / директорий
        /// </summary>
        public void Prev()
        {
            mDir.CurPos--;
            mDir.Change = true;
        }
        /// <summary>
        /// курсор позиционируется на 1 страницу вверх по списку папок / директорий. количество строк в странице указывается в свойствах проекта
        /// </summary>
        public void PageUp()
        {
            mDir.CurPos -= pageCount;
            mDir.Change = true;
        }
        /// <summary>
        /// форматируем вывод верхней строки
        /// </summary>
        private string UpperLine
        {
            get
            {
                int maxLength = LengthHorizont - 2;
                string v = " File Manager ";
                int p = (maxLength - v.Length) / 2;
                return string.Format("{0}{1}{2}", (char)Frame.AngleUpLeft, new string((char)Frame.HorizontLine, maxLength), (char)Frame.AngleUpRight).Remove(p, v.Length).Insert(p, v);
            }
        }
        /// <summary>
        /// форматируем вывод нижней строки
        /// </summary>
        private string DownerLine
        {
            get
            {
                int maxLength = LengthHorizont - 2;
                return string.Format("{0}{1," + maxLength + "}{2}", (char)Frame.AngleDownLeft, new string((char)Frame.HorizontLine, maxLength), (char)Frame.AngleDownRight);
            }
        }
        private string mline;
        /// <summary>
        /// форматируем вывод строки между верхней строкой и разделительной
        /// </summary>
        private string MiddleLine
        {
            set
            {
                int maxLength = LengthHorizont - 2;
                mline = string.IsNullOrEmpty(value)
                    ? string.Format("{0}{1,-" + maxLength + "}{2}", (char)Frame.VerticalLine, new string((char)Frame.Space, maxLength), (char)Frame.VerticalLine)
                    : string.Format("{0}{1,-" + maxLength + "}{2}", (char)Frame.VerticalLine, formLine(value), (char)Frame.VerticalLine);
            }
            get
            {
                return mline;
            }
        }
        /// <summary>
        /// форматируем разделительнкю строку
        /// </summary>
        private string DividerLine
        {
            get
            {
                return string.Format("{0}{1}{2}", (char)Frame.DividerLeft, new string((char)Frame.HorizontLine, LengthHorizont - 2), (char)Frame.DividerRight);
            }
        }
        /// <summary>
        /// заготовка для формирования рамки
        /// символы взяты вот отсюда
        /// https://unicode-table.com/ru/#2524 
        /// </summary>
        public enum Frame
        {
            AngleUpLeft = '┌',
            AngleDownLeft = '└',
            AngleUpRight = '┐',
            AngleDownRight = '┘',
            HorizontLine = '─',
            VerticalLine = '│',
            DividerLeft = '├',
            DividerRight = '┤',
            Space = ' '
        }

        /// <summary>
        /// инициализатор объекта
        /// </summary>
        public OutCons()
        {
            LengthHorizont = Console.WindowWidth - 1; // запомним размер окна по горизонтали
            HeightVertical = Console.WindowHeight - 1; // запомним размер окна по вертикали
        }
        /// <summary>
        /// метод очищает консоль
        /// </summary>
        public void Clear()
        {
            Console.Clear();
        }

        /// <summary>
        /// формируем секцию для вывода свойств директории / файла
        /// </summary>
        private void writeSection2()
        {
            var md = mDir;
            if (md is null)
                return;

            if (md.Info != null) // проверка наличия информации по папке / файлу
            {
                if ((md.Info.Attributes & FileAttributes.Directory) == FileAttributes.Directory) // если элемент - директория - форматируем вывод специальным образом
                {
                    line[0] = string.Format("Директория {0}", md.Info.FullName);
                    try
                    {
                        // пытаемся получить и вывести информацию о содержимом папки
                        line[1] = string.Format("Размер {0}. Содержит {1} поддиректорий и {2} файлов", md.SpecSize, Directory.GetDirectories(md.Info.FullName).Length, Directory.GetFiles(md.Info.FullName).Length);
                    }
                    catch
                    {
                        // если доступ запрещен - форматируем вывод соответствующим образом
                        line[1] = "Доступ к директории запрещён";
                    }
                    line[2] = string.Format("Срытая [{0}] Системная [{1}]",
                        ((md.Info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) ? "X" : " ", // установим или снимем отметку, что директория является скрытой
                        ((md.Info.Attributes & FileAttributes.System) == FileAttributes.System) ? "X" : " ");// установим или снимем отметку, что директория является системной
                }
                else // иначе - элемент файл, форматируем вывод соответствующим образом
                {
                    line[0] = string.Format("Файл {0}", md.Info.Name);
                    line[1] = string.Format("Размер файла {0}", md.SpecSize);
                    line[2] = string.Format("Срытый [{0}] Системный [{1}]",
                        ((md.Info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) ? "X" : " ", // установим или снимем отметку, что файл является скрытым
                        ((md.Info.Attributes & FileAttributes.System) == FileAttributes.System) ? "X" : " ");// установим или снимем отметку, что файл является системным
                }
            }
            else
            {
                line[0] = string.Format("Информация отсутствует");
                line[1] = string.Empty;
                line[2] = string.Empty;
            }
        }
        /// <summary>
        /// подготовка к выводу на экран информации по списку директорий / файлов
        /// </summary>
        private void writeConsole()
        {
            if (mDir is null)
                return;
            Console.WriteLine(UpperLine);
            MiddleLine = "";
            posForOut = mDir.CurPos;
            int maxBorder = HeightVertical - 7;
            if (mDir.Minus) // проверка направления движения курсора
            {
                if (posForOut < firstPos) // корректировка текущей позиции
                    firstPos--;
                if (posForOut == 0) // если курсор вышел за рамку - устанавливаем его в минимальное значение
                    firstPos = 0;
            }
            else
                if (posForOut >= maxBorder) // если курсор вышел за рамку - устанавливаем его в максимальное значение
                    firstPos = posForOut - maxBorder + 1;
            for (int i = 0; i < maxBorder; i++)
            {
                MiddleLine = string.Empty;
                if (i + firstPos < mDir.Folders.Length)
                {
                    FileSystemInfo fi = mDir.subDir.Where(l => (string.Compare(l.FullName, mDir.Folders[i + firstPos]) == 0)).FirstOrDefault();

                    if (i + firstPos == mDir.CurPos) // выделим подсветкой текущую позицию курсора - черный символы на белом фоне
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }

                    MiddleLine = fi.Name;
                }
                Console.WriteLine(MiddleLine);
                Console.ForegroundColor = ConsoleColor.White; // выводим на экран информацию в "обычном" цвете белые символы на черном фоне
                Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.WriteLine(DividerLine);
            // подготовим информацию по файлу / директории для вывода на экран
            writeSection2();
            // выведем информацию по файлу/директории
            int j = 0;
            for (int i = HeightVertical - 6; i < (HeightVertical - 3); i++)
            {
                MiddleLine = line[j];
                Console.WriteLine(MiddleLine);
                j++;
            }
            Console.WriteLine(DividerLine);
            MiddleLine = commandLine;
            for (int i = HeightVertical - 4; i < (HeightVertical - 3); i++)
                Console.WriteLine(MiddleLine);
            Console.Write(DownerLine);
            Console.CursorTop = 0;
            Console.SetCursorPosition(1, HeightVertical - 1);
        }

        /// <summary>
        /// вывод на экран информации по директории
        /// </summary>
        public void Show()
        {
            Clear();
            LengthHorizont = LengthHorizont != Console.WindowWidth ? Console.WindowWidth - 1 : LengthHorizont;
            HeightVertical = HeightVertical != Console.WindowHeight ? Console.WindowHeight - 1 : HeightVertical;
            Console.ForegroundColor = ConsoleColor.White; // выводим на экран информацию в "обычном" цвете белые символы на черном фоне
            Console.BackgroundColor = ConsoleColor.Black;
            writeConsole();
        }
    }
}
