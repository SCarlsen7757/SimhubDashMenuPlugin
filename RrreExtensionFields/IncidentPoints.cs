using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace RrreExtensionFields
{
    internal class IncidentPoints : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public IncidentPoints(string gameName) : base(gameName, "RRRE")
        {
            Data = new DataField()
            {
                Name = "ICP",
                Color = new ColorScheme("#f9ff42")
            };
        }

        public string Description => "Incident points.";

        protected override bool GameSupported(string gameName)
        {
            return gameName == "RRRE";
        }

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning) return;
            var r3eGameData = (R3E.Data.Shared)data.NewData.GetRawDataObject();
            int incidentPoints = r3eGameData.IncidentPoints;
            if (incidentPoints < 0)
            {
                Data.Value = "-";
            }
            else
            {
                Data.Value = r3eGameData.IncidentPoints.ToString();
            }
        }
    }
}
