using DashMenu.UI;
using Newtonsoft.Json;


namespace DashMenu.Settings
{
    internal class Settings
    {
        public Settings()
        {
            GameName = SimHub.Plugins.PluginManager.GetInstance().GameName;
        }

        [JsonIgnore]
        internal string GameName { get; private set; }
        /// <summary>
        /// Fields displayed. Per game and car.
        /// </summary>
        public ObservableDictionary<string, GameSettings> GameSettings { get; set; } = new ObservableDictionary<string, GameSettings>();

        internal GameSettings GetCurrentGameSettings()
        {
            if (!GameSettings.ContainsKey(GameName))
            {
                GameSettings.Add(GameName, new GameSettings());
            }

            return GameSettings[GameName];
        }
    }
}
