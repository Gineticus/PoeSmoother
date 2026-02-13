using LibBundle3.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeSmoother.Patches;

public class Minimap : IPatch
{
    public string Name => "Minimap Patch";
    public object Description => "Reveals the entire minimap by default with custom colors.";

    // Properties for colors/transparency (set from UI)
    public float RevealThreshold { get; set; } = 0.18f; // Full reveal

    // Unrevealed background (black low opacity)
    public float UnrevealedR { get; set; } = 0.0f;
    public float UnrevealedG { get; set; } = 0.0f;
    public float UnrevealedB { get; set; } = 0.0f;
    public float UnrevealedA { get; set; } = 0.01f;

    // Revealed area (clear)
    public float RevealedR { get; set; } = 0.0f;
    public float RevealedG { get; set; } = 0.0f;
    public float RevealedB { get; set; } = 0.0f;
    public float RevealedA { get; set; } = 0.0f;

    // Outline (white bright)
    public float OutlineR { get; set; } = 1.0f;
    public float OutlineG { get; set; } = 1.0f;
    public float OutlineB { get; set; } = 1.0f;
    public float OutlineA { get; set; } = 1.0f;

    // Exploration front (blue by default)
    public float FrontR { get; set; } = 0.0f;
    public float FrontG { get; set; } = 0.5f;
    public float FrontB { get; set; } = 1.0f;
    public float FrontA { get; set; } = 0.5f;

    public void Apply(DirectoryNode root)
    {
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "shaders")
            {
                foreach (var f in dir.Children)
                {
                    if (f is FileNode file1 && file1.Name == "minimap_visibility_pixel.hlsl")
                    {
                        var record = file1.Record;
                        var bytes = record.Read();
                        string data = System.Text.Encoding.ASCII.GetString(bytes.ToArray());

                        if (data.Contains("res_color = max(res_color, "))
                        {
                            continue;
                        }

                        List<string> lines = data.Split("\r\n").ToList();
                        int index = lines.FindIndex(line => line.Contains("res_color = float4(1.0f, 0.0f, 0.0f, 1.0f);"));
                        if (index == -1) continue;
                        lines.Insert(index + 1, $"\tres_color = max(res_color, {RevealThreshold});");

                        string newData = string.Join("\r\n", lines);
                        var newBytes = System.Text.Encoding.ASCII.GetBytes(newData);
                        record.Write(newBytes);
                    }

                    if (f is FileNode file2 && file2.Name == "minimap_blending_pixel.hlsl")
                    {
                        var record = file2.Record;
                        var bytes = record.Read();
                        string data = System.Text.Encoding.ASCII.GetString(bytes.ToArray());

                        // Unrevealed/revealed background
                        const string origWalkable = "float4 walkable_color = float4(1.0f, 1.0f, 1.0f, 0.01f);";
                        string newWalkable = $"float4 walkable_color = float4({UnrevealedR}, {UnrevealedG}, {UnrevealedB}, {UnrevealedA});";  // Unrevealed
                        data = data.Replace(origWalkable, newWalkable);

                        // Outline and revealed blend
                        const string origRevealed = "float4 walkability_map_color = lerp(walkable_color, float4(0.5f, 0.5f, 1.0f, 0.5f), walkable_to_edge_ratio);";
                        string newRevealed = $"float4 walkability_map_color = lerp(walkable_color, float4({OutlineR}, {OutlineG}, {OutlineB}, {OutlineA}), walkable_to_edge_ratio);";  // This is the outline - White by default
                        data = data.Replace(origRevealed, newRevealed);

                        // Exploration front (blue line)
                        const string origFront = "float4(0.0f, 0.5f, 1.0f, 0.5f)";
                        string newFront = $"float4({FrontR}, {FrontG}, {FrontB}, {FrontA})";  // Blue front
                        data = data.Replace(origFront, newFront);

                        var newBytes = System.Text.Encoding.ASCII.GetBytes(data);
                        record.Write(newBytes);
                    }
                }
            }
        }
    }
}