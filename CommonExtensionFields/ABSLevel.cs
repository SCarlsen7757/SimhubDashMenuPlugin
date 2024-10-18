using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace CommonExtensionFields
{
    public class ABSLevel : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public ABSLevel(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "ABS",
                Color = new ColorScheme("#00ff2a")
            };
        }

        public string Description => "ABS Level.";

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.ABSLevel < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.ABSLevel.ToString();
        }
    }
}
