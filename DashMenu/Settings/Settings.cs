using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class Settings : INotifyPropertyChanged
    {
        //TODO: Add INotifyPropertyChanged to settings class, to make the UI work properly.
        private int maxFields = 5;
        /// <summary>
        /// Max amount of fields that can be displayed.
        /// </summary>
        public int MaxFields
        {
            get => maxFields; set
            {
                if (value == maxFields) return;
                maxFields = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Fields displayed.
        /// </summary>
        public string[] DisplayedFields { get; set; }
        /// <summary>
        /// All fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary> 
        [JsonConverter(typeof(IFieldConverter))]
        public List<IFields> Fields { get; set; } = new List<IFields>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public class IFieldConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(List<>);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JArray array = JArray.Load(reader);
                var list = new List<IFields>();

                foreach (JToken item in array)
                {
                    // Deserialize each item to Fields
                    var fieldsObject = item.ToObject<Fields>(serializer);

                    // Cast Fields to TInterface
                    var interfaceObject = (IFields)fieldsObject;

                    list.Add(interfaceObject);
                }

                return list;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}
