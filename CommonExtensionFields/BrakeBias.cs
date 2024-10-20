using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.ComponentModel;

namespace CommonExtensionFields
{
    public class BrakeBias : FieldExtensionBase<IGaugeField>, IDataFieldExtension, IGaugeFieldExtension, IAlert
    {
        public BrakeBias(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "BB",
                IsDecimalNumber = true,
                Decimal = 1,
                Color = new ColorScheme("#d90028"),
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
            Data.PropertyChanged += DataAlert_PropertyChanged;
        }

        private void DataAlert_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is IDataField)) return;
            switch (e.PropertyName)
            {
                case nameof(IDataField.Value):
                    EndTime = DateTime.Now + ShowTimeDuration;
                    break;
                default:
                    break;
            }
        }

        public string Description => "Brake bias.";

        IDataField IFieldExtensionBasic<IDataField>.Data { get => Data; set => Data = (IGaugeField)value; }

        IDataField IAlert.Data { get => Data; set => Data = (IGaugeField)value; }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.BrakeBias < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.BrakeBias.ToString($"N{Data.Decimal}");
        }

        public bool Show { get => DateTime.Now < EndTime; }

        public TimeSpan ShowTimeDuration { get; set; } = TimeSpan.Zero;

        public DateTime EndTime { get; protected set; } = DateTime.Now;
    }
}
