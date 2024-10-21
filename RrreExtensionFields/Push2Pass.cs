using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace RrreExtensionFields
{
    public class Push2Pass : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public Push2Pass(string gameName) : base(gameName, "RRRE")
        {
            Data = new DataField()
            {
                Name = "P2P",
                Color = new ColorScheme("#ba42ff")
            };
        }

        public string Description => "Push to pass.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "RRRE";
        }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var r3eGameData = (R3E.Data.Shared)data.NewData.GetRawDataObject();
            Data.Value = r3eGameData.PushToPass.AmountLeft.ToString();
        }
    }
}
