using System.Collections.Generic;

namespace DashMenu.Settings
{
    internal class Settings
    {
        //TODO: Add INotifyPropertyChanged to settings class, to make the UI work properly.
        /// <summary>
        /// Max amount of fields that can be displayed.
        /// </summary>
        internal int MaxFields { get; set; } = 5;
        /// <summary>
        /// Fields displayed.
        /// </summary>
        internal string[] DisplayedFields { get; set; }
        /// <summary>
        /// All fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary>
        internal List<Fields> Fields { get; set; } = new List<Fields>();
    }
}
