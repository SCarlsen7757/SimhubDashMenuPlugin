# Dash menu

This plugin allows you to create and display customizable data fields on the dashboard. These fields are easy to change, even during a race, eliminating the need for multiple dashboards for different car types.

![Demo that shows fields can be changed and amount of fields can be changes as well](./Image/Demo.gif)
*Demo of the testing dashboard found in the [repo](./Dash%20menu%20test.simhubdash).*

## Using

When using a car for the first time, a default number of empty fields will be created. These fields can then be adjusted to display the relevant data for the car. The number of fields can also be changed as needed.

When using a car that has already been set up, it will use the latest settings for that car.

## Install plugin

To install the plugin:

1. Download the DLL file.
2. Paste the DLL file into the root directory of Simhub: `C:\Program Files (x86)\SimHub`.

## Install Data fields

This plugin uses a flexible and expandable approach similar to Simhub for plugins. To add additional fields:

1. Create a folder named `DashMenuExtensionFields` in the root directory of Simhub.
2. Place any additional dash menu data field DLL files into this folder.

The DLL files placed in this folder will be loaded when starting Simhub.

## Configure Control Mapping

After installing the plugin and additional data fields, you need to configure the control mapping to manage actions. This is done in the "Controls and Events" menu.

1. Select the "Controls and Events" menu.
2. Navigate to the "Controls" tab.
3. Click on "New mapping".
4. Search for the action.
5. Assign a button.

Repeat step 3 to 5 for all required actions.

Available Actions:

|Name| Required | Description|
| --- | :---: | --- |
| `ToggleConfigMode` | :heavy_check_mark: | Toggle configuration mode |
| `ConfigNextField` | :heavy_check_mark: | Select next field when in configuration mode |
| `ConfigPrevField` | :heavy_check_mark: | Select previous field when in configuration mode |
| `ChangeFieldTypeNext` | :heavy_check_mark: | Change field type of the selected field when in configuration mode |
| `ChangeFieldTypePrev` | :heavy_check_mark: | Change field type of the selected field when in configuration mode |
| `IncreaseNumberOfFieldData` | :x: | Increase number of fields for the current car (maximum 20) |
| `DecreaseNumberOfFieldData` | :x: | Decrease number of fields for the current car (minimum 1) |

![Select "Controls and events" menu then "Controls" tab and click the "New mapping" button.](./Image/ControlsAndEvents.png)

## Using in Dashboard

In Dash Studio the field can be accessed with the NCalc/JavaScript function/method `dashfielddata(fieldnumber)`. Unfortunately it can't be found in the NCalc Functions list :unamused: but it's available :sunglasses:!
The easiest way is to create a widget and pass the field data as a variable.

![Write NCalc formula to get field data](./Image/PassFieldDataToWidget.png)

To get the number of fields for the current car, use the property `AmountOfFields`.

To display the values of the field data in a widget, use these JavaScript code snippets for the binding property:

* `return $prop("variable.data").Name`
* `return $prop("variable.data").Value`
* `return $prop("variable.data).Unit`
* `return $prop("variable.data").Color.Primary`
* `return $prop("variable.data").Color.Accent`

```mermaid
erDiagram
    FieldData{
      string Name "Name of the data"
      string Value "Value of the data"
      string Unit "Unit of the value"
      int Decimal "How many decimal if the value is a decimal number"
      ColorScheme Color "Color to be used for easy identification"
    }

    ColorScheme{
      string Primary "Primary color"
      string Accent "Accent color"
    }

    FieldData ||--|| ColorScheme : contains
```

### Config screen

It's possible to make a configuration screen by using the the following properties:

* `ConfigMode`
* `ActiveConfigField`

`ConfigMode` is true when it's possible to navigate and change the configuration of the displayed fields.

`ActiveConfigField` is the current index of the displayed field that can be changed.

Changes are automatically saved, and it is not possible to undo changes to the configuration except by manually reverting the changes yourself.

## Change amount of fields

In the General settings tab, you can adjust the default amount of fields to use when setting up a new car. You can choose any number between 1 and 20.

You can change the number of fields for the current car by assigning the `IncreaseNumberOfFieldData` and `DecreaseNumberOfFieldData` actions. When using a new car, the fields will be created with the default number of fields.

## Configuring Fields

You can change or configure various settings for the fields.

![Configuration of fields](./Image/FieldSettings.png)

### General settings

#### Enable

The field data extension can be enabled to make it selectable or disabled to reduce the number of selectable fields.

### Override

You can override some of the default behavior of the field.

#### Name

Override the displayed name of the field data.

#### Color

Simhub includes a built-in function for day/night settings, allowing you to configure a color scheme for both day and night. This ensures that the field colors change appropriately with the day/night mode.

##### Day Night mode

Simhub includes a built-in function for day/night settings, allowing you to configure a color scheme for both day and night. This ensures that the field colors change appropriately with the day/night mode.

### Decimal

If the field value is a decimal number, you can adjust the number of decimal places displayed. This setting is only visible if the field value can be a decimal number.

## Make your own extension fields

The following examples demonstrate how to create basic extension data fields for use within the `DashMenuPlugin` system. Each extension field includes a description, data fields with a name and color scheme, and an update method that is called on every game tick.

### Example 1: Basic Extension Field

This example creates a simple traction control (TC) level field.

```C#
//ref DashMenuPlugin.dll (this plugin)
using DashMenu.Data;
//ref GameReaderCommon.dll (found in Simhub root dir)
using GameReaderCommon;

namespace CommonDataFields
{
    /// <summary>
    /// Represents a traction control (TC) level field.
    /// </summary>
    public class TCLevel : ExtensionDataBase, IFieldDataComponent
    {
        public TCLevel(string gameName) : base(gameName) { }
        /// <summary>
        /// Gets the description of the field.
        /// </summary>
        public string Description { get => "TC Level."; }
        /// <summary>
        /// Gets or sets the data associated with the field.
        /// </summary>
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "TC",
            Color = new ColorScheme("#00a3d9", "#ffffff")
        };
        /// <summary>
        /// Updates the field value based on the current game data.
        /// </summary>
        /// <param name="data">The current game data.</param>
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.TCLevel < 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.TCLevel.ToString();
        }
    }
}

```

### Example 2: Decimal Number Field

This example creates a brake bias field, which can represent decimal numbers.

```c#
using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    /// <summary>
    /// Represents a brake bias field.
    /// </summary>
    public class BrakeBias : ExtensionDataBase, IFieldDataComponent
    {
        public BrakeBias(string gameName) : base(gameName) { }
        /// <summary>
        /// Gets the description of the field.
        /// </summary>
        public string Description { get => "Brake bias."; }
        /// <summary>
        /// Gets or sets the data associated with the field.
        /// </summary>
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "BB",
            //Add IsDecimalNumber and Decimal
            IsDecimalNumber = true,
            Decimal = 1,
            Color = new ColorScheme("#d90028", "#ffffff")
        };
        /// <summary>
        /// Updates the field value based on the current game data.
        /// </summary>
        /// <param name="data">The current game data.</param>
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.BrakeBias < 0)
            {
                Data.Value = "-";
                return;
            }
            //Use Data.Decimal in the ToString(formatter)
            Data.Value = data.NewData.BrakeBias.ToString($"N{Data.Decimal}");
        }
    }
}
```

### Example 3: Field with Unit

This example creates a water temperature field, which includes a unit derived from SimHub's settings.

```c#
using DashMenu.Data;
using GameReaderCommon;

namespace CommonDataFields
{
    /// <summary>
    /// Represents a water temperature field.
    /// </summary>
    public class WaterTemperature : ExtensionDataBase, IFieldDataComponent
    {
        public WaterTemperature(string gameName) : base(gameName) { }
        /// <summary>
        /// Gets the description of the field.
        /// </summary>
        public string Description { get => "Water temperature"; }
        /// <summary>
        /// Gets or sets the data associated with the field.
        /// </summary>
        public FieldData Data { get; set; } = new FieldData()
        {
            Name = "Water Temp",
            IsDecimalNumber = true,
            Decimal = 0,
            Color = new ColorScheme("#ffffff", "#ffffff")
        };
        /// <summary>
        /// Updates the field value based on the current game data.
        /// </summary>
        /// <param name="data">The current game data.</param>
        public void Update(ref GameData data)
        {
            if (!data.GameRunning) return;
            if (data.NewData.WaterTemperature <= 0)
            {
                Data.Value = "-";
                return;
            }
            Data.Value = data.NewData.WaterTemperature.ToString($"N{Data.Decimal}");
            //Unit is taken from the default unit configured in Simhub.
            Data.Unit = "°" + data.NewData.TemperatureUnit[0];
        }
    }
}
```

### FieldData class structure

```mermaid
classDiagram
    class DashMenuPlugin{
      +bool ConfigMode
      +int ActiveConfigField
      +int AmountOfFields
      +FieldData(string gameName)
    }
```

## Future features

* [ ] Sort the data fields to make cycling through them easier.
* [ ] Add extension for gauges.
