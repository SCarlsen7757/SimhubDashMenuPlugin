using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DashMenu.Extensions
{
    public static class ListExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IList<T> list)
        {
            return new ObservableCollection<T>(list);
        }
    }
}
