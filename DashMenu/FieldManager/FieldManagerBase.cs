using SimHub.Plugins;
using System;
using System.Collections.Generic;

namespace DashMenu.FieldManager
{
    internal abstract class FieldManagerBase
    {
        public FieldManagerBase()
        {
            gameName = SimHub.Plugins.PluginManager.GetInstance().GameName;
        }

        public FieldManagerBase(PluginManager pluginManager, Type pluginType) : this()
        {
            this.pluginManager = pluginManager;
            this.pluginType = pluginType;
        }

        public FieldManagerBase(PluginManager pluginManager, Type pluginType, string fieldTypeName) : this(pluginManager, pluginType)
        {
            amountOfFieldName = $"AmountOf{fieldTypeName}Fields";
        }

        protected readonly PluginManager pluginManager;
        protected readonly Type pluginType;
        protected readonly string gameName;
        private readonly string amountOfFieldName;
        protected string AmountOfFieldName { get => amountOfFieldName; }


        internal delegate void SelectedFieldsChangedEventHandler(IList<string> selectedFields);
    }
}