﻿using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    public class ABSLevel : IFieldDataComponent
    {
        public string Description { get => "ABS Level."; }

        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "ABS",
            Color = new ColorScheme("#00ff2a", "#ffffff")
        };

        public bool GameSupported(string name) => true;
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.ABSLevel < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.ABSLevel.ToString();
        }
    }
}
