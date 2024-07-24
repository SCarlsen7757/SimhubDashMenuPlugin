using System.Collections.Generic;

namespace DashMenu.Settings.DisplayedFields
{
    internal class CarSettings
    {
        public string CarId;
        public string CarModel;
        public List<string> DisplayedFields { get; set; }
        public CarSettings() { }
        public CarSettings(string carId, string carModel, List<string> fields)
        {
            CarId = carId;
            CarModel = carModel;
            DisplayedFields = fields;
        }
    }
}
