using System;
using System.Configuration;


namespace FileManager_CP
{
    class Settings
    {
        public string Path; // путь к просматриваемой папке / файлу
        public int Page; // количество строк для постраничного просмотра
        /// <summary>
        /// конструктор объекта
        /// </summary>
        public Settings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                Path = appSettings["path"];
            }
            catch (ConfigurationErrorsException) // если не удалось прочитать сохранённые параметры, устанавливаем их в значения по-умолчанию
            {
                Path = @"c:\";
            }
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (!int.TryParse(appSettings["page"], out Page))
                    Page = 20;
            }
            catch (ConfigurationErrorsException) // если не удалось прочитать сохранённые параметры, устанавливаем их в значения по-умолчанию
            {
                Page = 20;
            }

        }
        /// <summary>
        /// метод сохраняет данные пользователя в файле настроек
        /// </summary>
        /// <param name="path"></param>
        /// <param name="page"></param>
        public void PutSettings()
        {
            AddUpdateAppSettings("path", Path);
            AddUpdateAppSettings("page", Page.ToString());
        }

        /// <summary>
        /// метод сохраняет данные параметра в файл настроек
        /// </summary>
        /// <param name="key"> название параметра </param>
        /// <param name="value"> значение параметра </param>
        private void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] is null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
