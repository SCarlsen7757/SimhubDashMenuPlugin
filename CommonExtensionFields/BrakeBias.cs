using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class BrakeBias : FieldExtensionBase, IGaugeFieldExtension
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
        }

        public string Description => "Brake bias.";

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

        IDataField IDataFieldExtension.Data
        {
            get => Data; // Return the same GaugeField instance
            set => Data = (IGaugeField)value; // Set the same instance
        }

        IGaugeField IGaugeFieldExtension.Data
        {
            get => Data;
            set => Data = value; //Make sure to set base Data
        }

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
    }
}
