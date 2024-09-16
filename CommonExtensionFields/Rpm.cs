﻿using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class Rpm : FieldExtensionBase, IGaugeFieldComponent
    {
        public Rpm(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "RPM",
                IsDecimalNumber = false,
                Color = new ColorScheme("#ffffff", "#000000"),
                IsRangeLocked = true,
                Minimum = 0.ToString()
            };
        }

        public string Description { get => "RPM"; }

        private IGaugeField data;
        new IGaugeField Data
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

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            Data.Maximum = data.NewData.CarSettings.MaxRpm.ToString();
            Data.Value = DecimalValue(data.NewData.Rpms);
        }
    }
}