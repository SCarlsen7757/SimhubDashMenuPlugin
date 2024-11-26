using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace AccExtensionFields
{
    public class TC2Level : AlertBase, IDataFieldExtension
    {
        public TC2Level(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "TC2",
                Color = new ColorScheme("#00a3d9")
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
            int tc2Level = accGameData.Graphics.TCCut;
            if (tc2Level < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = tc2Level.ToString();
        }
    }
}
