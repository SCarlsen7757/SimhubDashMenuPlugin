using System.Reflection;

namespace DashMenu
{
    public class Version
    {
        public static string PluginVersion { get => Assembly.GetExecutingAssembly().GetName().Version.ToString(3); }
    }
}
