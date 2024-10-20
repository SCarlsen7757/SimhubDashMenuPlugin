using System;
using System.Linq;

namespace DashMenu.Extensions
{
    internal static class TypeExtensions
    {
        public static bool ContainsInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }
    }
}
