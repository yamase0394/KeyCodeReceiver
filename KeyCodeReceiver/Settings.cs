using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace KeyCodeReceiver
{
    public class Settings
    {
        private static string serializeFile = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\settings.config";

        private int port;
        public int Port
        {
            get
            {
                if (port == 0)
                {
                    return 8888;
                }
                else
                {
                    return port;
                }
            }
            set
            {
                port = value;
            }
        }
        public string Pw { get; set; }

        private Settings()
        {

        }

        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LoadFromXML();
                    Console.WriteLine("finished loading");
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public void Save()
        {
            Console.WriteLine("start saving");
            var serializer = new DataContractSerializer(typeof(Settings));
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.Encoding = new UTF8Encoding(false);
            using (var writer = XmlWriter.Create(serializeFile, xmlSettings))
            {
                serializer.WriteObject(writer, instance);
            }

            Console.WriteLine("finish saving");
        }

        private static Settings LoadFromXML()
        {
            if (!File.Exists(serializeFile))
            {
                Console.WriteLine("setting file dose not exist");
                Settings settings = new Settings();
                return settings;
            }
            Console.WriteLine("start loading");

            var serializer = new DataContractSerializer(typeof(Settings));
            using (var reader = XmlReader.Create(serializeFile))
            {
                return (Settings)serializer.ReadObject(reader);
            }
        }
    }
}
