using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSMedium_4._2
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurator = new Configurator();
            var fileWatcher = new ConfigurationFileWatcher("E:", "Config.xml", configurator);

            Console.WriteLine("Для выхода нажмите 'q', и любую другую клавишу для показа опций");
            while(true)
            {
                if(Console.ReadKey(true).Key == ConsoleKey.Q)
                    break;
                else
                    configurator.ShowOptions();
            }
        }
    }

    class Configurator
    {
        public Dictionary<string, string> Options { get; private set; }

        public Configurator()
        {
            Options = new Dictionary<string, string>();
        }

        public void SetOption(string key, string value)
        {
            if(Options.ContainsKey(key))
                Options[key] = value;
            else
                Options.Add(key, value);
        }

        public void ShowOptions()
        {
            foreach(var option in Options) {
                Console.WriteLine(option.Key + " => " + option.Value);
            }
            Console.WriteLine();
        }
    }

    class ConfigurationFileWatcher
    {
        private string _filePath;
        private FileSystemWatcher _systemWatcher;
        private Configurator _configurator;

        public ConfigurationFileWatcher(string filePath, string fileName, Configurator configurator)
        {
            _filePath = filePath + @"\" + fileName;

            _configurator = configurator;

            _systemWatcher = new FileSystemWatcher(filePath, fileName);
            _systemWatcher.Changed += RefreshOptions;
            _systemWatcher.EnableRaisingEvents = true;
        }

        public void RefreshOptions(object sender, FileSystemEventArgs e)
        {
            // Пробовал вынести в private XmlTextReader _xmlReader, но тогда возникала ошибка, мол, файл используется другим процессом.
            // Не знаю, почему так, но если каждый раз объявлять XmlTextReader заново, то всё хорошо.
            var reader = new XmlTextReader(@_filePath);

            string optionName = "";
            while(reader.Read())
            {
                switch(reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if(IsRootTag(reader.Name))
                            continue;
                        else
                            optionName = reader.Name;
                        break;
                    case XmlNodeType.Text:
                        _configurator.SetOption(optionName, reader.Value);
                        break;
                }
            }

            // Надпись почему-то выводится два раза. Всю голову сломал, но так и не понял причину.
            Console.WriteLine("Файл был изменён");
        }

        private bool IsRootTag(string tag)
        {
            return tag == "options";
        }
    }
}
