﻿using DashMenu.Data;
using GameReaderCommon;

namespace CommonExtensionFields
{
    public class EngineMap : FieldExtensionBase, IDataFieldComponent
    {
        public EngineMap(string gameName) : base(gameName)
        {
            Data = new DataField()
            {
                Name = "MAP",
                Color = new ColorScheme("#d9c000", "#808080")
            };
        }

        public string Description { get => "Engine map"; }

        IDataField IDataFieldComponent.Data
        {
            get => Data;
            set => Data = value;
        }

        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.EngineMap < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.EngineMap.ToString();
        }
    }
}
