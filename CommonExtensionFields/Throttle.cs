﻿using DashMenu.Data;
using GameReaderCommon;


namespace CommonExtensionFields
{
    public class Throttle : FieldExtensionBase, IGaugeFieldComponent
    {
        public Throttle(string gameName) : base(gameName)
        {
            Data = new GaugeField()
            {
                Name = "Throttle",
                IsDecimalNumber = true,
                Decimal = 0,
                Unit = "%",
                Color = new ColorScheme("#ffffff", "#000000"),
                IsRangeLocked = true,
                Maximum = 100.ToString(),
                Minimum = 0.ToString()
            };
        }

        public string Description { get => "Throttle position"; }

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
            if (data.NewData.Throttle < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = DecimalValue(data.NewData.Throttle);
        }
    }
}