using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace PushkinA.EnglishVocabulary.Services
{
    public interface IDataService<T>
    {
        T[] Get(string fileName="");

        void Set(T[] questions, string fileName="");

        string[] GetFiles();        
    }

    public class DataService<T> : IDataService<T> where T:new()
    {
        private readonly string dataFolderName = Path.Combine(Environment.CurrentDirectory,"DATA");

        public void Set(T[] entities, string fileName = "")
        {
            var xmlFileName = Path.Combine(dataFolderName, Path.ChangeExtension(fileName == string.Empty ? typeof(T).Name : fileName,".xml"));

            using (var writer = new XmlTextWriter(xmlFileName, Encoding.Unicode) { Formatting=Formatting.Indented})
            {
                var serializer = new XmlSerializer(typeof(T[]), string.Empty);                
                serializer.Serialize(writer, entities);
            }
        }

        public T[] Get(string fileName = "")
        {
            var xmlFileName = Path.Combine(dataFolderName, Path.ChangeExtension(fileName == string.Empty ? typeof(T).Name : fileName, ".xml"));
            if (!File.Exists(xmlFileName)) return new T[0];

            var xmlUri = new Uri(xmlFileName);

            using (var reader = new XmlTextReader(xmlUri.AbsolutePath))
            {
                var serializer = new XmlSerializer(typeof(T[]));
                try
                {
                    return (T[])serializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    return new T[0];
                }
            }
        }

        public string[] GetFiles()
        {            
            if (!Directory.Exists(dataFolderName)) return new string[0];

            var files = Directory.GetFiles(dataFolderName, "*.xml", SearchOption.TopDirectoryOnly);
            return files.Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
        }

        public void Rename(string oldFileName, string newFileName)
        {
            string oldFilePath = Path.Combine(dataFolderName, oldFileName, "*.xml");
            string newFilePath = Path.Combine(dataFolderName, oldFileName, "*.xml");

            if (!File.Exists(oldFilePath)) return;

            if (File.Exists(newFilePath))
                throw new Exception(String.Format("File '{0}' already exists", "newFileName"));

            File.Move(oldFileName, newFileName);
        }
    }
}
