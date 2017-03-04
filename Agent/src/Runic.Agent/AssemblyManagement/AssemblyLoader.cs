using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using NLog;

namespace Runic.Agent.AssemblyManagement
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        private readonly string _folderPath;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AssemblyLoader(string folderPath)
        {
            _folderPath = folderPath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            _logger.Info($"Loading assembly {assemblyName.FullName}");
            var deps = DependencyContext.Default;
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();

            if (res.Count > 0)
                return Assembly.Load(new AssemblyName(res.First().Name));

            var apiApplicationFileInfo =
                new FileInfo($"{_folderPath}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");

            if (File.Exists(apiApplicationFileInfo.FullName))
            {
                var asl = new AssemblyLoader(apiApplicationFileInfo.DirectoryName);
                return asl.LoadFromAssemblyPath(apiApplicationFileInfo.FullName);
            }

            _logger.Error($"SAssembly not found {assemblyName.FullName}");
            throw new AssemblyNotFoundException();
        }
    }
}