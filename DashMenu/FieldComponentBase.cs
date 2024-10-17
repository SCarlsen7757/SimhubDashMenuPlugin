﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu
{
    internal abstract class FieldComponentBase<T>
    {
        protected bool enabled = true;
        private string fullName = null;
        public bool Enabled
        {
            get => enabled; set
            {
                if (enabled == value) return;
                enabled = value;
                OnPropertyChanged();
            }
        }
        public string FullName
        {
            get
            {
                if (fullName == null)
                {
                    fullName = FieldExtension.GetType().FullName;
                }
                return fullName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public T FieldExtension { get; set; }
    }
}