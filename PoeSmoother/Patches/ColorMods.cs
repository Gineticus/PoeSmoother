using LibBundle3.Nodes;
using LibGGPK3.Records;
using PoeSmoother.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PoeSmoother.Patches;

public class ColorMods : IPatch
{
    public string Name => "Color Mods Patch";
    public object Description => "Changes colors of mods in the game.";
    public List<ColorModsOption> ColorModsOptions { get; set; } = new()
    {
        new ColorModsOption("map_monsters_reflect_%_physical_damage", "yellow", true),
        new ColorModsOption("map_monsters_reflect_%_elemental_damage", "green", true),
        new ColorModsOption("map_player_cannot_expose", "red", true),
        new ColorModsOption("map_players_no_regeneration_including_es", "red", true),
        new ColorModsOption("map_player_non_curse_aura_effect_+%", "red", true),
        new ColorModsOption("map_monsters_avoid_elemental_ailments_%", "red", true),
        new ColorModsOption("map_monsters_cannot_be_leeched_from", "red", true),
        new ColorModsOption("map_monsters_cannot_be_stunned", "red", true),
        new ColorModsOption("map_additional_player_maximum_resistances_%", "red", true),
        new ColorModsOption("map_monsters_are_hexproof", "red", true),
        new ColorModsOption("map_player_cooldown_speed_+%_final", "red", true),
        new ColorModsOption("chance_%_to_drop_additional_divine_orb", "yellow", true),
        new ColorModsOption("map_boss_additional_divine_orb_to_drop", "yellow", true),
        new ColorModsOption("%_chance_to_duplicate_dropped_currency", "blue", true),
        new ColorModsOption("%_chance_to_duplicate_dropped_divination_cards", "blue", true),
    };

    private readonly Dictionary<string, string> _color_conversions = new()
    {
        { "red", "premiumchatoutline" },
        { "green", "quest" },
        { "blue", "divination" },
        { "yellow", "necropolisupside" },
        { "pink", "archnemesismodchaospurple" },
    };
    private enum ReadState
    {
        WritingData,
        ReadingData,
        ReadingDescription,
        ReadingToDescription
    }

    private readonly string[] _extensions = {
        ".txt",
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
                    var lines = data.Split("\r\n").ToList();

                    ReadState state = ReadState.ReadingToDescription;

                    string? currentAnnotation = null;
                    bool? currentIsEnabled = null;

                    int linesToWrite = 0;

                    for (int i = 0; i < lines.Count; i++)
                    {
                        string line = lines[i];

                        if (line.StartsWith("description"))
                        {
                            state = ReadState.ReadingDescription;
                            continue;
                        }

                        if (state == ReadState.ReadingToDescription) continue;

                        // Read the description on the next line.
                        if (state == ReadState.ReadingDescription)
                        {
                            string[] description = line.Split(' ');
                            if (description.Length < 2)
                            {
                                state = ReadState.ReadingToDescription;
                                continue;
                            }

                            string modType = description[1];

                            if (ColorModsOptions.FirstOrDefault(x => x.Name == modType) is ColorModsOption option
                                && _color_conversions.TryGetValue(option.Color.ToLower(), out string? annotation))
                            {
                                currentAnnotation = annotation;
                                currentIsEnabled = option.IsEnabled;
                                state = ReadState.ReadingData;
                            }
                            else
                            {
                                currentAnnotation = null;
                                state = ReadState.ReadingToDescription;
                            }

                            continue;
                        }

                        if (state == ReadState.ReadingData)
                        {
                            // Replace tabs in line with nothing.
                            string firstNumber = line.Replace("\t", "").Split(' ')[0];

                            // May be a "lang" value. If the value is a number write those lines.
                            if (int.TryParse(firstNumber, out int value))
                            {
                                state = ReadState.WritingData;
                                linesToWrite = value;
                                continue;
                            }
                            ;
                        }

                        if (state == ReadState.WritingData)
                        {
                            if (line.Contains('<')) // Already annotated.
                            {
                                if (currentIsEnabled == false)
                                {
                                    // Remove annotation.
                                    // <.*?>{{value}} -> "value".
                                    string pattern = "<.*?>{{(.*?)}}";
                                    string replacement = Regex.Replace(line, pattern, new MatchEvaluator(match =>
                                    {
                                        return $"{match.Groups[1].Value}";
                                    }));
                                    lines[i] = replacement;
                                }
                                else
                                {
                                    // Replace text between brackets with new annotation.
                                    string pattern = "<.*?>";
                                    string replacement = Regex.Replace(line, pattern, new MatchEvaluator(match =>
                                    {
                                        return $"<{currentAnnotation}>";
                                    }));
                                    lines[i] = replacement;
                                }
                            }
                            else
                            {
                                // Surround the value with the annotation.
                                // "value" -> "<annotation>{{value}}".

                                string pattern = "\".*?\"";
                                string replacement = Regex.Replace(line, pattern, new MatchEvaluator(match =>
                                {
                                    return $"\"<{currentAnnotation}>{{{{{match.Value.Replace("\"", "")}}}}}\"";
                                }));
                                lines[i] = replacement;
                            }

                            linesToWrite--;
                            if (linesToWrite == 0)
                            {
                                state = ReadState.ReadingData;
                                continue;
                            }
                        }
                    }

                    var newData = string.Join("\r\n", lines);
                    var newBytes = System.Text.Encoding.Unicode.GetBytes(newData);
                    record.Write(newBytes);
                }
            }
        }
    }

    public void Apply(DirectoryNode root)
    {
        // go to metadata/statdescriptions/
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
            {
                foreach (var dir1 in dir.Children)
                {
                    if (dir1 is DirectoryNode subDir && subDir.Name == "statdescriptions")
                    {
                        RecursivePatcher(subDir);
                    }
                }
            }
        }
    }
}