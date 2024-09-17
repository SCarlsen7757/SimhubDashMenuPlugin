using DashMenu.Data;
using GameReaderCommon;

namespace DashMenu
{
    internal class EmptyDataField : FieldExtensionBase, IDataFieldComponent
    {
        public EmptyDataField(string gameName) : base(gameName) { }
        public string Description { get; } = string.Empty;
        new public IDataField Data { get; set; } = new DataField();

        public void Update(ref GameData data)
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
