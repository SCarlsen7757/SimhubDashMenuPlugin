using DashMenu.Data;
using GameReaderCommon;
using SimHub.Plugins;

namespace DashMenu
{
    internal class EmptyDataField : FieldExtensionBase<IDataField>, IDataFieldExtension
    {
        public EmptyDataField(string gameName) : base(gameName)
        {
            Data = new DataField();
        }
        public string Description { get; } = string.Empty;

        public void Update(PluginManager pluginManager, ref GameData data)
        {
            return;
        }

        private readonly static EmptyDataField @field = new EmptyDataField("");
        /// <summary>
        /// Empty field
        /// </summary>
        public static EmptyDataField Field => @field;
        private static string fullname = null;
        public static string FullName
        {
            get
            {
                if (fullname == null)
                {
                    fullname = Field.GetType().FullName;
                }
                return fullname;
            }
        }
    }
}
