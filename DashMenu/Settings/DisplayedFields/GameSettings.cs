using DashMenu.UI;

namespace DashMenu.Settings.DisplayedFields
{
    internal class GameSettings
    {
        public ObservableDictionary<string, CarFields> CarFields { get; set; }
        public GameSettings()
        {
            CarFields = new ObservableDictionary<string, CarFields>();
        }
    }
}
