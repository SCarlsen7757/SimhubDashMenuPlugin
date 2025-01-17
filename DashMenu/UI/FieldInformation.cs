using System.Collections;
using System.Collections.Generic;

namespace DashMenu.UI
{
    internal class FieldInformation
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public int Index { get; set; }

        internal static IEnumerable ItemsControlSource<FieldType>(IList<string> defaultFields, Settings.FieldSettings<FieldType> fieldSettings) where FieldType : Settings.IBasicSettings, new()
        {
            var fields = new List<FieldInformation>();

            for (int i = 0; i < defaultFields.Count; i++)
            {
                var fieldDetails = fieldSettings.Settings[defaultFields[i]];
                var info = new FieldInformation()
                {
                    Index = i,
                    Namespace = fieldDetails.Namespace,
                    Name = fieldDetails.Name
                };
                fields.Add(info);
            }
            return fields;
        }
    }
}
