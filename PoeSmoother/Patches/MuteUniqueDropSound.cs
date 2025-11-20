using System;
using System.IO;
using System.Reflection;
using LibBundle3.Nodes;
using LibGGPK3.Records;

namespace PoeSmoother.Patches;

public class MuteUniqueDropSound : IPatch
{
    public string Name => "Mute Unique Drop Sound";
    public object Description => "Removes the sound effect when unique items drop.";

    private int FindClosingBrace(string text, int startIndex)
    {
        int braceCount = 0;
        bool foundOpenBrace = false;

        for (int i = startIndex; i < text.Length; i++)
        {
            if (text[i] == '{')
            {
                braceCount++;
                foundOpenBrace = true;
            }
            else if (text[i] == '}')
            {
                braceCount--;
                if (foundOpenBrace && braceCount == 0)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public void Apply(DirectoryNode root)
    {
        // go to metadata/effects/misc/unique_drop/uniquedrop.aoc
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
            {
                foreach (var d1 in dir.Children)
                {
                    if (d1 is DirectoryNode effectsDir && effectsDir.Name == "effects")
                    {
                        foreach (var d2 in effectsDir.Children)
                        {
                            if (d2 is DirectoryNode miscDir && miscDir.Name == "misc")
                            {
                                foreach (var d3 in miscDir.Children)
                                {
                                    if (d3 is DirectoryNode uniqueDir && uniqueDir.Name == "unique_drop")
                                    {
                                        foreach (var d4 in uniqueDir.Children)
                                        {
                                            if (d4 is FileNode file && file.Name == "uniquedrop.aoc")
                                            {
                                                var record = file.Record;
                                                var bytes = record.Read();
                                                string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                                                if (string.IsNullOrEmpty(data)) continue;

                                                // Remove SoundEvents section
                                                int soundIndex = data.IndexOf("SoundEvents");
                                                if (soundIndex != -1)
                                                {
                                                    int endBrace = FindClosingBrace(data, soundIndex);
                                                    if (endBrace != -1)
                                                    {
                                                        data = data.Remove(soundIndex, endBrace - soundIndex + 1);
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
        }
    }
}
