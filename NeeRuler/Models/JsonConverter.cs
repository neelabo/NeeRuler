using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace NeeRuler.Models
{
    public static class JsonConverter
    {
        private static DataContractJsonSerializerSettings _settings = new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true };

        public static void Serialize<T>(T data, string jsonFilePathOut)
        {
            using (var fs = new FileStream(jsonFilePathOut, FileMode.Create, FileAccess.Write))
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, Encoding.UTF8, true, true))
            {
                var serializer = new DataContractJsonSerializer(typeof(T), _settings);
                serializer.WriteObject(writer, data);
            }
        }

        public static T Deserialize<T>(string jsonFilePathIn)
        {
            using (var fs = new FileStream(jsonFilePathIn, FileMode.Open, FileAccess.Read))
            using (var reader = JsonReaderWriterFactory.CreateJsonReader(fs, XmlDictionaryReaderQuotas.Max))
            {
                var serializer = new DataContractJsonSerializer(typeof(T), _settings);
                return (T)serializer.ReadObject(reader);
            }
        }
    }
}