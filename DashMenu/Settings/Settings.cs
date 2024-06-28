using System.Collections.Generic;

namespace DashMenu.Settings
{
    public class Settings
    {
        //TODO: Add INotifyPropertyChanged to settings class, to make the UI work properly.
        /// <summary>
        /// Max amount of fields that can be displayed.
        /// </summary>
        public int MaxFields { get; set; } = 5;
        /// <summary>
        /// Fields displayed.
        /// </summary>
        public string[] DisplayedFields { get; set; }
        /// <summary>
        /// All fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary>
        public List<Fields> Fields { get; set; } = new List<Fields>();
    }
}
