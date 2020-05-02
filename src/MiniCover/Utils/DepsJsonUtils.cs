using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MiniCover.Utils
{
    public class DepsJsonUtils
    {
        public readonly IFileSystem _fileSystem;

        public DepsJsonUtils(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void PatchDepsJson(IFileInfo depsJsonFile, string hitServicesVersion)
        {
            var content = _fileSystem.File.ReadAllText(depsJsonFile.FullName);
            var newContent = PatchDepsJsonContent(content, hitServicesVersion);
            _fileSystem.File.WriteAllText(depsJsonFile.FullName, newContent);
        }

        public void UnpatchDepsJson(IFileInfo depsJsonFile)
        {
            var content = _fileSystem.File.ReadAllText(depsJsonFile.FullName);
            var newContent = UnpatchDepsJsonContent(content);
            _fileSystem.File.WriteAllText(depsJsonFile.FullName, newContent);
        }

        public string PatchDepsJsonContent(string content, string hitServicesVersion)
        {
            var json = JsonConvert.DeserializeObject<JObject>(content);

            var targets = json["targets"] as JObject;

            if (targets != null)
            {
                foreach (var target in targets.PropertyValues().OfType<JObject>())
                {
                    target[$"MiniCover.HitServices/{hitServicesVersion}"] = new JObject
                    {
                        ["runtime"] = new JObject
                        {
                            ["MiniCover.HitServices.dll"] = new JObject()
                        }
                    };
                }
            }

            var libraries = json["libraries"] as JObject;
            if (libraries != null)
            {
                libraries[$"MiniCover.HitServices/{hitServicesVersion}"] = new JObject
                {
                    ["type"] = "project",
                    ["serviceable"] = false,
                    ["sha512"] = ""
                };
            }

            return JsonConvert.SerializeObject(json, Formatting.Indented);
        }

        public string UnpatchDepsJsonContent(string content)
        {
            var json = JsonConvert.DeserializeObject<JObject>(content);

            var targets = json["targets"] as JObject;
            if (targets != null)
            {
                foreach (var target in targets.PropertyValues().OfType<JObject>())
                {
                    var miniCoverProperties = target.Properties()
                        .Where(p => p.Name.StartsWith("MiniCover.HitServices/"))
                        .ToArray();

                    foreach (var property in miniCoverProperties)
                        property.Remove();
                }
            }

            var libraries = json["libraries"] as JObject;
            if (libraries != null)
            {
                var miniCoverProperties = libraries.Properties()
                    .Where(p => p.Name.StartsWith("MiniCover.HitServices/"))
                    .ToArray();

                foreach (var property in miniCoverProperties)
                    property.Remove();
            }

            return JsonConvert.SerializeObject(json, Formatting.Indented);
        }

        public DependencyContext LoadDependencyContext(IDirectoryInfo assemblyDirectory)
        {
            var depsJsonPath = assemblyDirectory.GetFiles("*.deps.json", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            if (depsJsonPath == null)
                return null;

            using (var depsJsonStream = depsJsonPath.OpenRead())
            {
                using (var reader = new DependencyContextJsonReader())
                {
                    return reader.Read(depsJsonStream);
                }
            }
        }

        public List<string> GetAdditionalPaths(string runtimeConfigContent)
        {
            var additionalPaths = new List<string>();
            if (string.IsNullOrEmpty(runtimeConfigContent))
            {
                return additionalPaths;
            }

            var runtimeConfig = JsonConvert.DeserializeObject<JObject>(runtimeConfigContent);
            if (runtimeConfig == null)
                return additionalPaths;

            var additionalProbingPaths = runtimeConfig["runtimeOptions"]?["additionalProbingPaths"];
            if (additionalProbingPaths != null)
            {
                foreach (var path in additionalProbingPaths.Values<string>())
                {
                    if (!path.Contains("|"))
                    {
                        additionalPaths.Add(path);
                    }
                }
            }
            return additionalPaths;
        }
    }
}
