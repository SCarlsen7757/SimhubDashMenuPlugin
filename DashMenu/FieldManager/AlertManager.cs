using DashMenu.Data;
using DashMenu.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DashMenu.FieldManager
{
    internal class AlertManager
    {
        public AlertManager()
        {
            SimhubHelper.AddNCalcFunction($"dashalertshow",
                "Returns true of an alert is aktiv.",
                string.Empty,
                engine => (Func<bool>)AlertShow);

            SimhubHelper.AddNCalcFunction($"dashalertname",
                "Returns the name of the aktiv alert.",
                string.Empty,
                engine => (Func<string>)AlertName);

            SimhubHelper.AddNCalcFunction($"dashalertvalue",
                "Returns the value of the aktiv alert.",
                string.Empty,
                engine => (Func<string>)AlertValue);

            SimhubHelper.AddNCalcFunction($"dashalertunit",
                "Returns the unit of the aktiv alert.",
                string.Empty,
                engine => (Func<string>)AlertUnit);

            SimhubHelper.AddNCalcFunction($"dashalertcolorprimary",
                "Returns the primary color of the aktiv alert.",
                string.Empty,
                engine => (Func<string>)AlertColorPrimary);

            SimhubHelper.AddNCalcFunction($"dashalertcoloraccent",
                "Returns the accent color of the aktiv alert.",
                string.Empty,
                engine => (Func<string>)AlertColorAccent);
        }

        internal ObservableCollection<IAlert> SelectedAlerts { get; private set; } = new ObservableCollection<IAlert>();

        private readonly ObservableCollection<IFieldComponent<IDataFieldExtension, IDataField>> allAlerts = new ObservableCollection<IFieldComponent<IDataFieldExtension, IDataField>>();

        public IList<IFieldComponent<IDataFieldExtension, IDataField>> AllAlerts { get => allAlerts; }

        internal void AddAlerts(IList<IFieldComponent<IDataFieldExtension, IDataField>> allDataFields, IDictionary<string, Settings.Alert> settings)
        {
            foreach (var dataField in allDataFields)
            {
                Type type = dataField.FieldExtension.GetType();
                if (!type.ContainsInterface(typeof(IAlert))) continue;
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
                    SelectedAlerts.Add((IAlert)alert.FieldExtension);

                    UpdateShowTimeDuration(alertSetting);
                }
            }
        }

        internal void UpdateProperties(Settings.IDataField dataFieldSettings, Settings.IAlert alertSettings, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(alertSettings.Enabled):
                    UpdateProperties(dataFieldSettings, alertSettings);
                    return;
                case nameof(alertSettings.ShowTimeDuration):
                    UpdateShowTimeDuration(alertSettings);
                    return;
                default:
                    break;
            }
        }

        private void UpdateProperties(Settings.IDataField dataFieldSettings, Settings.IAlert alertSettings)
        {
            var field = AllAlerts.First(x => x.FullName == alertSettings.FullName);
            if (field == null) return;

            if (dataFieldSettings.Enabled && alertSettings.Enabled)
            {
                SelectedAlerts.Add((IAlert)AllAlerts.First(x => x.FullName == field.FullName).FieldExtension);
            }
            else
            {
                SelectedAlerts.Remove((IAlert)AllAlerts.First(x => x.FullName == field.FullName).FieldExtension);
            }
        }

        private void UpdateShowTimeDuration(Settings.IAlert settings)
        {
            var field = AllAlerts.First(x => x.FullName == settings.FullName) ?? throw new ArgumentException($"Alert not found! {settings.FullName}");
            var fieldExtionsion = (IAlert)field.FieldExtension;
            fieldExtionsion.ShowTimeDuration = settings.ShowTimeDuration;
        }

        private IAlert LatestAlert()
        {
            return SelectedAlerts
            .Where(x => x.Show)
            .OrderByDescending(x => x.EndTime)
            .FirstOrDefault();
        }

        public bool AlertShow() => LatestAlert()?.Show ?? false;

        public string AlertName() => LatestAlert()?.Data.Name ?? string.Empty;

        public string AlertValue() => LatestAlert()?.Data.Value ?? string.Empty;

        public string AlertUnit() => LatestAlert()?.Data.Unit ?? string.Empty;

        public string AlertColorPrimary() => LatestAlert()?.Data.Color.Primary ?? string.Empty;

        public string AlertColorAccent() => LatestAlert()?.Data?.Color.Accent ?? string.Empty;
    }
}
