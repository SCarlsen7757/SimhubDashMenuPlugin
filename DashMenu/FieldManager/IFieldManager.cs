using System;
using System.Collections.Generic;

namespace DashMenu.FieldManager
{
    internal interface IFieldManager<T>
    {
        void AddExtensionField(Type type, IDictionary<string, T> settings);
        void DayNightModeChanged(IDictionary<string, T> settings);
        void NextSelectedField(int index);
        void PrevSelectedField(int index);
        void UpdateSelectedFields(Settings.ICarFields carFields);
        void RemoveField();
        void AddField();
    }
}
