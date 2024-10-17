using SimHub.Plugins.OutputPlugins.Dash.TemplatingCommon;
using System;

namespace DashMenu.Extensions
{
    internal static class SimhubHelper
    {
        internal static bool AddNCalcFunction(string name, string description, string syntax, Func<NCalcEngineBase, Delegate> func)
        {
            if (NCalcEngineMethodsRegistry.GenericMethodsProvider.ContainsKey(name.ToLower())) { return false; }

            NCalcEngineMethodsRegistry.AddMethod(name,
                syntax,
                description,
                func);
            NCalcEngineBase.AvailableFunctions.Add(new SimHub.Plugins.OutputPlugins.Dash.WPFUI.FormulaPropertyEntry(name, $"{name}({syntax})", description));
            return true;
        }
    }
}
