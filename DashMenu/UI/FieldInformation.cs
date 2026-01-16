using DashMenu.Settings.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DashMenu.UI
{
    internal sealed class FieldInformation
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public int Index { get; set; }

        internal static IEnumerable<FieldInformation> ItemsControlSource<TFieldSettings>(
            IList<string> defaultFields,
            Settings.FieldSettings<TFieldSettings> fieldSettings) where TFieldSettings : IBasicSettings, new()
        {
            return defaultFields
                .Select((fieldKey, index) => new { fieldKey, index })
                .Select(item => new
                {
                    item.index,
                    found = fieldSettings.Settings.TryGetValue(item.fieldKey, out TFieldSettings settings),
                    settings
                })
                .Where(item => item.found)
                .Select(item => new FieldInformation
                {
                    Index = item.index,
                    Namespace = item.settings.Namespace,
                    Name = item.settings.Name
                });
        }
    }
}
