using DashMenu.Data;
using GameReaderCommon;

namespace DashMenu
{
    internal class EmptyDataField : FieldExtensionBase, IFieldDataComponent
    {
        public EmptyDataField(string gameName) : base(gameName) { }
        public string Description { get; } = "";
        public IDataField Data { get; set; } = new DataField()
        {
            Name = "",
            Value = "",
            Color = new ColorScheme("#ffffff", "#ffffff")
        };

        public void Update(ref GameData data)
        {
            return;
        }

        private readonly static EmptyDataField field = new EmptyDataField("");
        /// <summary>
        /// Empty field
        /// </summary>
        public static EmptyDataField Field { get { return field; } }
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
