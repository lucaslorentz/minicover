using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace MiniCover.Utils
{
    public class DepsJsonUtils
    {
        public static void PatchDepsJson(string depsJsonFile)
        {
            var content = File.ReadAllText(depsJsonFile);
            var json = JsonConvert.DeserializeObject<JObject>(content);

            var targets = json["targets"] as JObject;

            if (targets != null)
            {
                foreach (var target in targets.PropertyValues().OfType<JObject>())
                {
                    target["MiniCover.HitServices/1.0.0"] = new JObject
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
                libraries["MiniCover.HitServices/1.0.0"] = new JObject
                {
                    ["type"] = "project",
                    ["serviceable"] = false,
                    ["sha512"] = ""
                };
            }

            var newContent = JsonConvert.SerializeObject(json, Formatting.Indented);

            File.WriteAllText(depsJsonFile, newContent);
        }

        public static void UnpatchDepsJson(string depsJsonFile)
        {
            var content = File.ReadAllText(depsJsonFile);
            var json = JsonConvert.DeserializeObject<JObject>(content);

            var targets = json["targets"] as JObject;
            if (targets != null)
            {
                foreach (var target in targets.PropertyValues().OfType<JObject>())
                {
                    target.Remove("MiniCover.HitServices/1.0.0");
                }
            }

            var libraries = json["libraries"] as JObject;
            if (libraries != null)
            {
                libraries.Remove("MiniCover.HitServices/1.0.0");
            }

            var newContent = JsonConvert.SerializeObject(json, Formatting.Indented);

            File.WriteAllText(depsJsonFile, newContent);
        }
    }
}
