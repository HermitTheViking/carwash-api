using Newtonsoft.Json;
using System;

namespace Domain.Events.Serialization
{
    public class JsonSerializer
    {
        public string Serialize(object item)
        {
            string assemblyQualifiedName = item.GetType().AssemblyQualifiedName;
            string serializedValue = JsonConvert.SerializeObject(item);

            return assemblyQualifiedName + "|" + serializedValue;
        }

        public object Deserialize(string serializedItem)
        {
            int index = serializedItem.IndexOf('|');

            string assemblyQualifiedName = serializedItem.Substring(0, index);
            string serializedValue = serializedItem.Substring(index + 1);

            Type type = Type.GetType(assemblyQualifiedName);
            return JsonConvert.DeserializeObject(serializedValue, type);
        }
    }
}