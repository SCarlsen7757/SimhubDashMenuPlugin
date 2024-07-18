using DashMenu.Data;
using GameReaderCommon;

namespace DashMenu
{
    internal class EmptyField : IFieldDataComponent
    {
        public string Description { get; } = "";
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "",
            Value = "",
            Color = new ColorScheme("#ffffff", "#ffffff")
        };

        public bool GameSupported(string name)
        {
            return true;
        }

        public void Update(ref GameData data)
        {
            return;
        }

        private static EmptyField field = new EmptyField();
        /// <summary>
        /// Empty field
        /// </summary>
        public static EmptyField Field { get { return field; } }
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
