using System;
using System.Collections.Generic;

namespace DashMenu.FieldManager
{
    internal interface IFieldManager<TSettingsField>
    {
        void AddExtensionField(Type type, IDictionary<string, TSettingsField> settings);
        void DayNightModeChanged(IDictionary<string, TSettingsField> settings);
        void NextSelectedField(int index);
        void PrevSelectedField(int index);
        void UpdateSelectedFields(Settings.ICarFields carFields);
        void RemoveField();
        void AddField();
    }
}
