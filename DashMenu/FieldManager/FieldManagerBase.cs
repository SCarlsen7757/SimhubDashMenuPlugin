using System.Collections.Generic;

namespace DashMenu.FieldManager
{
    internal abstract class FieldManagerBase
    {
        public FieldManagerBase()
        {
            gameName = SimHub.Plugins.PluginManager.GetInstance().GameName;
        }

        internal delegate void SelectedFieldsChangedEventHandler(IList<string> selectedFields);

        protected readonly string gameName;
    }
}