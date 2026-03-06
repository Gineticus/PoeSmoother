using LibBundle3.Nodes;

namespace PoeSmoother.Patches;

public class Camera : IPatch
{
    public string Name => "Camera Patch";
    public object Description => "Allows adjusting the default camera zoom level.";

    public double ZoomLevel { get; set; } = 2.4;

    private readonly string[] _extensions = {
        ".ot",
        ".otc",
    };

    private readonly string[] _functions = {
        "CreateCameraZoomNode",
        "ClearCameraZoomNodes",
        "CreateCameraLookAtNode",
        "CreateCameraPanNode",
        "ClearCameraPanNode",
        "SetCustomCameraSpeed",
        "RemoveCustomCameraSpeed",
    };
    
    private string RemoveCameraFunctions(string data)
    {
        foreach (var func in _functions)
        {
            int pos = 0;
            while ((pos = data.IndexOf(func, pos, StringComparison.Ordinal)) != -1)
            {
                // Check if this is actually a function call (optionally preceded by identifier.)
                int start = pos;
                
                // Look backwards for optional prefix (like "camera_controller.")
                while (start > 0 && (char.IsLetterOrDigit(data[start - 1]) || data[start - 1] == '_' || data[start - 1] == '.'))
                {
                    start--;
                }
                
                // Skip whitespace after function name
                int parenPos = pos + func.Length;
                while (parenPos < data.Length && char.IsWhiteSpace(data[parenPos]))
                {
                    parenPos++;
                }
                
                // Check if followed by opening parenthesis
                if (parenPos >= data.Length || data[parenPos] != '(')
                {
                    pos++;
                    continue;
                }
                
                // Find matching closing parenthesis
                int depth = 1;
                int endPos = parenPos + 1;
                while (endPos < data.Length && depth > 0)
                {
                    if (data[endPos] == '(') depth++;
                    else if (data[endPos] == ')') depth--;
                    endPos++;
                }
                
                if (depth != 0)
                {
                    pos++;
                    continue; // Unmatched parentheses
                }
                
                // Skip whitespace and check for semicolon
                while (endPos < data.Length && char.IsWhiteSpace(data[endPos]))
                {
                    endPos++;
                }
                
                if (endPos < data.Length && data[endPos] == ';')
                {
                    endPos++; // Include the semicolon
                    data = data.Remove(start, endPos - start); // Remove the entire function call
                    pos = start; // Continue from where we removed
                }
                else
                {
                    pos++;
                }
            }
        }
        return data;
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

                if (Array.Exists(_extensions, ext => file.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    if (file.Name == "character.ot")
                    {
                        continue;
                    }

                    var record = file.Record;
                    var bytes = record.Read();
                    string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                    if (!_functions.Any(func => data.Contains(func)))
                    {
                        continue;
                    }

                    data = RemoveCameraFunctions(data);
                    
                    var newBytes = System.Text.Encoding.Unicode.GetBytes(data);
                    record.Write(newBytes);
                }
            }
        }
    }

    public void Apply(DirectoryNode root)
    {
        // go to metadata/
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
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
                                List<string> lines = data.Split("\r\n").ToList();
                                string zoomLevelString = ZoomLevel.ToString().Replace(',', '.');

                                if (data.Contains("CreateCameraZoomNode"))
                                {
                                    int x = lines.FindIndex(line => line.Contains("CreateCameraZoomNode"));
                                    lines[x] = $"\ton_initial_position_set = {{CreateCameraZoomNode(5000.0, 5000.0, {zoomLevelString});}} ";
                                }
                                else
                                {
                                    int index = lines.FindIndex(x => x.Contains("team = 1"));
                                    if (index == -1) continue;
                                    lines.Insert(index + 1, $"\ton_initial_position_set = {{CreateCameraZoomNode(5000.0, 5000.0, {zoomLevelString});}} ");
                                }
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