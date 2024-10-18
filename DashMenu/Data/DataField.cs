using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Data
{
    public class DataField : IDataField
    {
        private string name = string.Empty;

        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                OnPropertyChanged();
            }
        }

        private string @value = "-";

        public string Value
        {
            get => @value;
            set
            {
                if (this.@value == value) return;
                this.@value = value;
                OnPropertyChanged();
            }
        }

        private string unit = string.Empty;

        public string Unit
        {
            get => unit;
            set
            {
                if (unit == value) return;
                unit = value;
                OnPropertyChanged();
            }
        }

        public bool IsDecimalNumber { get; set; } = false;

        public int @decimal = 0;

        public int Decimal
        {
            get => @decimal; set
            {
                if (this.@decimal == value) return;
                @decimal = value;
                OnPropertyChanged();
            }
        }
        public ColorScheme Color { get; set; } = new ColorScheme();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
