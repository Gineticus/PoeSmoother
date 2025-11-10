using System;
using System.IO;
using System.Reflection;
using LibBundle3.Nodes;
using LibGGPK3.Records;

namespace PoeSmoother.Patches;

public class Light : IPatch
{
    public string Name => "Light Patch";
    public object Description => "Disables the default light effect in the game.";
    private readonly string[] extensions = {
        ".env",
    };

    private void RecursivePatcher(DirectoryNode dir)
    {
        foreach (var d in dir.Children)
        {
            if (d is DirectoryNode childDir)
            {
                RecursivePatcher(childDir);
            }
            else if (d is FileNode file)
            {

                if (Array.Exists(extensions, ext => file.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {

                    var record = file.Record;
                    var bytes = record.Read();
                    string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                    data = data.Replace("\"directional_light\"", "\"xirectional_light\"")
                        .Replace("\"player_light\"", "\"xlayer_light\"")
                        .Replace("\"environment_mapping\"", "\"xnvironment_mapping\"")
                        .Replace("\"global_illumination\"", "\"xlobal_illumination\"");
                    
                    var newBytes = System.Text.Encoding.Unicode.GetBytes(data);
                    record.Write(newBytes);
                }
            }
        }
    }

    public void Apply(DirectoryNode root)
    {
        // go to metadata/environmentsettings/
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
            {
                foreach (var d2 in dir.Children)
                {
                    if (d2 is DirectoryNode subDir && subDir.Name == "environmentsettings")
                    {
                        RecursivePatcher(subDir);
                    }
                }
            }
        }
    }
}