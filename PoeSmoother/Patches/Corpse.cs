using System;
using System.IO;
using System.Reflection;
using LibBundle3.Nodes;
using LibGGPK3.Records;

namespace PoeSmoother.Patches;

public class Corpse : IPatch
{
    public string Name => "Corpse Patch";
    public object Description => "Removes corpses from the game.";

    public void Apply(DirectoryNode root)
    {
        // go to metadata/monsters/monster.ot
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
            {
                foreach (var d1 in dir.Children)
                {
                    if (d1 is DirectoryNode subDir && subDir.Name == "monsters")
                    {
                        foreach (var d2 in subDir.Children)
                        {
                            if (d2 is FileNode file && file.Name == "monster.ot")
                            {
                                var record = file.Record;
                                var bytes = record.Read();
                                string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                                data = data.Replace("Life\r\n{\r\n}", "Life\r\n{\r\n\ton_spawned_dead = \"RemoveEffects(); DisableRendering();\"\r\n\ton_death = \"Delay( 1.0, { DisableRendering(); } );\"\r\n}");
                                data = data.Replace("slow_animations_go_to_idle = true\r\n}", "slow_animations_go_to_idle = true\r\n\ton_start_Revive = \"RemoveEffects(); EnableRendering();\"\r\n}");

                                var newBytes = System.Text.Encoding.Unicode.GetBytes(data);
                                record.Write(newBytes);
                            }
                        }
                    }
                }
            }
        }
    }
}
