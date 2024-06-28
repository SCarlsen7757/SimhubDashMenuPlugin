using DashMenu.Data;

namespace DashMenu
{
    public class FieldComponent
    {
        public FieldComponent() { }
        public FieldComponent(IFieldData fieldData) { FieldData = fieldData; }
        public bool Enabled { get; set; } = true;
        public IFieldData FieldData { get; set; }

        public override string ToString()
        {
            return base.ToString() + FieldData.ToString();
        }
    }
}
