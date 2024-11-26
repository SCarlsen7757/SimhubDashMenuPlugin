using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace AccExtensionFields
{
    public class WiperLevel : AlertBase, IDataFieldExtension
    {
        public WiperLevel(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "WIP",
                Color = new ColorScheme("#db973d")
            };
            Data.PropertyChanged += DataAlert_PropertyChanged;
        }

        public string Description => "TC 2 Level.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "AssettoCorsaCompetizione";
        }

        public override void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var accGameData = (ACSharedMemory.ACC.Reader.ACCRawData)data.NewData.GetRawDataObject();
            int wiperLevel = accGameData.Graphics.WiperLV;
            if (wiperLevel < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = wiperLevel.ToString();
        }
    }
}
