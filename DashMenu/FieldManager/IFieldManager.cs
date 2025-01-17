using DashMenu.Settings;
using System;
using System.Collections.Generic;

namespace DashMenu.FieldManager
{
    internal interface IFieldManager<TSettingsField> where TSettingsField : Settings.IDataField, new()
    {
        void AddExtensionField(Type type, FieldSettings<TSettingsField> settings);
        void DayNightModeChanged(IDictionary<string, TSettingsField> settings);
        void NextSelectedField(int index);
        void PrevSelectedField(int index);
        void UpdateSelectedFields(Settings.ICarFields carFields);
        void RemoveField();
        void AddField();
    }
}
