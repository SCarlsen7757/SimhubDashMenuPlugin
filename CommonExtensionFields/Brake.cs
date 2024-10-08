﻿using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class Brake : FieldExtensionBase, IGaugeFieldComponent
    {
        public Brake(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "BRK",
                IsDecimalNumber = true,
                Decimal = 0,
                Unit = "%",
                Color = new ColorScheme(),
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
        }
        public string Description => "Brake position.";

        private IGaugeField data;
        new public IGaugeField Data
        {
            get => data;
            set
            {
                data = value;
                base.Data = value;
            }
        }

        IDataField IDataFieldComponent.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }

        IGaugeField IGaugeFieldComponent.Data
        {
            get => Data;
            set => Data = value; //Make sure to set base Data
        }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Value = DecimalValue(data.NewData.Brake);
        }
    }
}
