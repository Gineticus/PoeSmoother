using System;
using System.IO;
using System.Reflection;
using LibBundle3.Nodes;
using LibGGPK3.Records;

namespace PoeSmoother.Patches;

public class Camera : IPatch
{
    public string Name => "Camera Patch";
    public object Description => "Allows adjusting the default camera zoom level.";

    private readonly string[] _directory = {
        "metadata",
    };

    private readonly string[] extensions = {
        ".ot",
        ".otc",
    };

    private double zoomLevel;

    public Camera(double zoom = 2.4)
    {
        zoomLevel = zoom;
    }

    public void SetZoomLevel(double zoom)
    {
        zoomLevel = zoom;
    }

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
                    if (file.Name == "character.ot") {
                        continue;
                    }

                    var record = file.Record;
                    var bytes = record.Read();
                    string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());
                    if (!(data.Contains("CreateCameraZoomNode") || data.Contains("ClearCameraZoomNodes") || data.Contains("CreateCameraLookAtNode") || data.Contains("SetCustomCameraSpeed")))
                    {
                        continue;
                    }

                    List<string> lines = data.Split("\r\n").ToList();

                    for (int i = lines.Count - 1; i >= 0; i--)
                    {
                        if (lines[i].Contains("CreateCameraZoomNode") || lines[i].Contains("ClearCameraZoomNodes") || lines[i].Contains("CreateCameraLookAtNode") || lines[i].Contains("SetCustomCameraSpeed"))
                        {
                            int start = lines[i].IndexOf("CreateCameraZoomNode") >= 0 ? lines[i].IndexOf("CreateCameraZoomNode") :
                                        lines[i].IndexOf("ClearCameraZoomNodes") >= 0 ? lines[i].IndexOf("ClearCameraZoomNodes") :
                                        lines[i].IndexOf("CreateCameraLookAtNode") >= 0 ? lines[i].IndexOf("CreateCameraLookAtNode") :
                                        lines[i].IndexOf("SetCustomCameraSpeed");

                            int end = lines[i].IndexOf(';', start);
                            lines[i] = lines[i][..start] + lines[i][(end + 1)..];
                        }

                    }
                    string newData = string.Join("\r\n", lines);
                    var newBytes = System.Text.Encoding.Unicode.GetBytes(newData);
                    record.Write(newBytes);
                }
            }
        }
    }

    public void Apply(DirectoryNode root)
    {
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && _directory.Contains(dir.Name))
            {
                RecursivePatcher(dir);

                // go to metadata/characters/character.ot
                foreach (var dir1 in dir.Children)
                {
                    if (dir1 is DirectoryNode subDir && subDir.Name == "characters")
                    {
                        foreach (var dir2 in subDir.Children)
                        {
                            if (dir2 is FileNode file && file.Name == "character.ot")
                            {
                                var record = file.Record;
                                var bytes = record.Read();
                                string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                                if (data.Contains("CreateCameraZoomNode"))
                                {
                                    continue;
                                }

                                List<string> lines = data.Split("\r\n").ToList();
                                int index = lines.FindIndex(x => x.Contains("team = 1"));
                                if (index == -1) continue;
                                string zoomLevelString = zoomLevel.ToString().Replace(',', '.');
                                lines.Insert(index + 1, $"\ton_initial_position_set = \"CreateCameraZoomNode(5000.0, 5000.0, {zoomLevelString});\" ");
                                string newData = string.Join("\r\n", lines);
                                var newBytes = System.Text.Encoding.Unicode.GetBytes(newData);
                                record.Write(newBytes);
                            }
                        }
                    }
                }
            }
        }
    }
}