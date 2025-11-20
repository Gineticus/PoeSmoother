using System;
using System.IO;
using System.Reflection;
using LibBundle3.Nodes;
using LibGGPK3.Records;

namespace PoeSmoother.Patches;

public class MuteCryingBaby : IPatch
{
    public string Name => "Mute Crying Baby & Goblins";
    public object Description => "Removes crying baby and goblin sounds.";

    private readonly string[] _extensions = {
        ".aoc",
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
                if (Array.Exists(_extensions, ext => file.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    var record = file.Record;
                    var bytes = record.Read();
                    string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                    // Remove all text after SoundEvents
                    int index = data.IndexOf("SoundEvents");
                    if (index != -1)
                    {
                        data = data[..index];
                        var newBytes = System.Text.Encoding.Unicode.GetBytes(data);
                        record.Write(newBytes);
                    }
                }
            }
        }
    }

    public void Apply(DirectoryNode root)
    {
        // go to metadata/pet/
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
            {
                foreach (var d1 in dir.Children)
                {
                    if (d1 is DirectoryNode petDir && petDir.Name == "pet")
                    {
                        // Process babybosseshumans directory
                        foreach (var d2 in petDir.Children)
                        {
                            if (d2 is DirectoryNode babyDir && babyDir.Name == "babybosseshumans")
                            {
                                RecursivePatcher(babyDir);
                            }
                            // Process goblinband/goblinbandleader.aoc file
                            else if (d2 is DirectoryNode goblinDir && goblinDir.Name == "goblinband")
                            {
                                foreach (var d3 in goblinDir.Children)
                                {
                                    if (d3 is FileNode file && file.Name == "goblinbandleader.aoc")
                                    {
                                        var record = file.Record;
                                        var bytes = record.Read();
                                        string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                                        int index = data.IndexOf("SoundEvents");
                                        if (index != -1)
                                        {
                                            data = data[..index];
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
        }
    }
}
