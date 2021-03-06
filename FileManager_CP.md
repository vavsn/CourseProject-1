# FileManager_CP
FileManager_CP - консольный файл менеджер, предназначенный для работы в среде Windows.

## Возможности

- просмотр содержимого диска / директории
- копирование директории (с поддиректориями / файлами), файла
- удаление директории (с поддиректориями / файлами), файла
- просмотр информации по директории: количество вложенных поддиреторий и файлов, размер директории, атрибутов "Системная" и "Скрытая"
- просмотр информации по файлу: размер файла, атрибуты "Системный" и "Скрытый"
- при выходе сохраняется состояние выводимой информации

## Описание

FileManager_CP написан на языке C# с использованием воможностей платформы .NET Framework. 

## Сборка

Для сборки проекта необходимо скопировать файлы проекта:

```sh
Program.cs
FileManager_CP.csproj
Settings.cs
Command.cs
app.manifest
..\Directory\Directory.cs
..\Directory\Directory.csproj
..\OutConsole\OutConsole.cs
..\OutConsole\OutConsole.csproj
```

## Зависимости

Для работы программы на компьютере должны быть установлены пакеты:

| Пакет | Установка |
| ------ | ------ |
| Microsoft ASP.NET Core Shared Framework 3.1.10 | [https://dotnet.microsoft.com/download/dotnet/3.1][MSNet] |

## Особенности работы программы

Для работы программы необходимы права администратора
При отсутствии таких прав у пользователя, программа попросит перезапуска с соответствующими правами

## Режимы работы программы
### Режим просмотра

Предназначен для просмотра информации по директориями / файлам в заданной пользователем директории
Режим выполняется автоматически при запуске программы, а также при выполнении команды, заданной пользователем

### Режим командной строки

Предназначен для выполнения определённых операций с директориями / файлами _копирование, удаление_, а также _выхода_ из программы
Для входа в командный режим необходимо нажать кнопку **Enter**
В нижней строке экрана выводится сообщение 
```sh
Введите команду: 
```

Выход из режима командной строки производится автоматически после выполнения команды

## Навигация...
### ... в режиме просмотра
Навигация по списку директорий / файлов происходит с помощью клавиш **Up, Down, PageUp, PageDown, Home, End** 
Для смены диска / директории необходимо войти в командный режим и ввести команду "cd ..."
### ... в режиме командной строки
В режиме командной строки навигация происходит по списку команд, ранее введённых пользователем в текущем сеансе работы программы
Нажатие на клавишу Enter запускает на выполнение команду, выбранную пользователем из истории команд

## Команды режима командной строки

#### изменение диска / директории 

```sh
cd ИМЯ_ПАПКИ
```
ИМЯ_ПАПКИ -  переход в директорию **ИМЯ_ПАПКИ** 
```sh
cd .\ИМЯ_ПАПКИ
```
.\ИМЯ_ПАПКИ - переход в поддиректорию **ИМЯ_ПАПКИ** текущего диска / директории
```sh
cd ..
```
переход  в корень **текущего диска** 

#### копирование директории / файла

```sh
cp ИМЯ_ИСТОЧНИК ИМЯ_ПРИЁМНИК
```
копирование директории / файла **ИМЯ_ИСТОЧНИК** в директорию / файл **ИМЯ_ПРИЁМНИК**

> ВНИМАНИЕ: **`ИМЯ_ИСТОЧНИК`** и **`ИМЯ_ПРИЁМНИК`** должны указываться как полный путь вида **c:\ИМЯ_ПАПКИ\ИМЯ_ФАЙЛА**
> ВНИМАНИЕ: если в имени директории / файла есть символы пробела имя необходимо писать в кавычках **`"c:\ИМЯ_ПАПКИ С ПРОБЕЛАМИ\ИМЯ_ФАЙЛА С ПРОБЕЛАМИ"`**

#### удаление директории / файла

```sh
del ИМЯ
```
удаление директории / файла ИМЯ

> ВНИМАНИЕ: **`ИМЯ`** должно указываться как полный путь вида **c:\ИМЯ_ПАПКИ\ИМЯ_ФАЙЛА**
> ВНИМАНИЕ: если в имени директории / файла есть символы **пробела** ИМЯ необходимо писать в кавычках **`"c:\ИМЯ_ПАПКИ С ПРОБЕЛАМИ\ИМЯ_ФАЙЛА С ПРОБЕЛАМИ"`**

## Результаты выполнения команды

В случае успешного выполнения команды в нижнюю строку экрана выводится сообщение
```sh
Успешно
```

В случае ошибки при выполнении команды в нижнюю строку экрана выводится сообщение об ошибке
```sh
Указана неверная команда
```

## Выход из программы

В режиме просмотра
```sh
Нажатие на клавишу ESC
```
В режиме командной строки
```sh
exit
```

[//]: # ()

   [MSNet]: <https://dotnet.microsoft.com/download/dotnet/3.1>
