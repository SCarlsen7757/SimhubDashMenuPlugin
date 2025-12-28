using DashMenu.Settings;
using DashMenu.Settings.Interfaces;
using System;
using System.Collections.Generic;

namespace DashMenu.FieldManager.Interfaces
{
    internal interface IFieldManager<TSettingsField> where TSettingsField : IDataFieldSettings, new()
    {
        void AddExtensionField(Type type, FieldSettings<TSettingsField> settings);
        void DayNightModeChanged(IDictionary<string, TSettingsField> settings);
        void NextSelectedField(int index);
        void PrevSelectedField(int index);
        void UpdateSelectedFields(ICarFieldsSettings carFields);
        void RemoveField();
        void AddField();
    }
}
