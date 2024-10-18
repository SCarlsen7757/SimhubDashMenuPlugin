using DashMenu.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DashMenu.FieldManager
{
    internal class AlertManager
    {
        public AlertManager()
        {
        }

        internal ObservableCollection<IAlert<IDataField>> SelectedAlerts { get; private set; } = new ObservableCollection<IAlert<IDataField>>();

        private readonly ObservableCollection<IFieldComponent<IDataFieldExtension, IDataField>> allAlerts = new ObservableCollection<IFieldComponent<IDataFieldExtension, IDataField>>();

        public IList<IFieldComponent<IDataFieldExtension, IDataField>> AllAlerts { get => allAlerts; }

        internal void AddAlerts(IList<IFieldComponent<IDataFieldExtension, IDataField>> allDataFields, IDictionary<string, Settings.Alert> settings)
        {
            foreach (var dataField in allDataFields)
            {
                Type type = dataField.FieldExtension.GetType();
                if (!type.GetInterfaces().Contains(typeof(IAlert<IDataField>))) continue;
                if (!settings.TryGetValue(dataField.FullName, out var alertSettings))
                {
                    alertSettings = new Settings.Alert()
                    {
                        Enabled = true
                    };
                    settings.Add(type.FullName, alertSettings);
                }

                alertSettings.Namespace = type.Namespace;
                alertSettings.Name = type.Name;
                alertSettings.FullName = type.FullName;

                allAlerts.Add(dataField);
            }
        }

        internal void UpdateSelectedAlerts(IDictionary<string, Settings.DataField> dataFieldsSettings, IDictionary<string, Settings.Alert> alertsSettings)
        {
            foreach (var key in alertsSettings.Keys)
            {
                var dataFieldSetting = dataFieldsSettings[key];
                var alertSetting = alertsSettings[key];

                if (dataFieldSetting.Enabled && alertSetting.Enabled)
                {
                    var alert = AllAlerts.First(x => x.FullName == alertSetting.FullName) ?? throw new ArgumentException($"Alert not found! {key}.");
                    SelectedAlerts.Add((IAlert<IDataField>)alert.FieldExtension);

                    UpdateShowTimeDuration(alertSetting);
                }
            }
        }

        internal void UpdateProperties(Settings.DataField dataFieldSettings, Settings.Alert alertSettings)
        {
            var field = AllAlerts.First(x => x.FullName == alertSettings.FullName);
            if (field == null) return;

            if (dataFieldSettings.Enabled && alertSettings.Enabled)
            {
                SelectedAlerts.Add((IAlert<IDataField>)AllAlerts.First(x => x.FullName == field.FullName).FieldExtension);
            }
            else
            {
                SelectedAlerts.Remove((IAlert<IDataField>)AllAlerts.First(x => x.FullName == field.FullName).FieldExtension);

            }
        }

        internal void UpdateShowTimeDuration(Settings.Alert settings)
        {
            var field = AllAlerts.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Alert not found! {settings.FullName}");
            var fieldExtionsion = (IAlert<IDataField>)field.FieldExtension;
            fieldExtionsion.ShowTimeDuration = settings.ShowTimeDuration;
        }
    }
}
