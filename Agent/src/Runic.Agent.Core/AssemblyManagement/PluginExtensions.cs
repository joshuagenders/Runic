using System.Linq;
using System.Reflection;

namespace Runic.Agent.Core.AssemblyManagement
{
    public static class PluginExtensions
    {
        public static Assembly PopulateStaticPropertiesWithInstance<T>(this Assembly assembly, T instance)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                var staticFields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                staticFields.Where(f => f.GetType() == typeof(T))
                            .ToList()
                            .ForEach(f => f.SetValue(type, instance));
            }
            return assembly;
        }
    }
}
