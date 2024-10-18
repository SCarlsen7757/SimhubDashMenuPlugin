using DashMenu.Data;
using System.ComponentModel;

namespace DashMenu
{
    internal interface IFieldComponent<TFieldExtension, TField> : INotifyPropertyChanged
        where TFieldExtension : class, IFieldExtensionBasic<TField>
        where TField : class, IDataField
    {
        bool Enabled { get; set; }
        TFieldExtension FieldExtension { get; set; }
        string FullName { get; }
    }
}