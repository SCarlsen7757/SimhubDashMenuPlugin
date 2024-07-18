using System.Collections.Generic;

namespace DashMenu.Settings.DisplayedFields
{
    internal class GameSettings
    {
        public Dictionary<string, CarSettings> CarSettings { get; set; }
        public GameSettings()
        {
            CarSettings = new Dictionary<string, CarSettings>();
        }
    }
}
