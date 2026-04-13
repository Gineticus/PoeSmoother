using LibBundle3.Nodes;
using PoeRedux.Patches.Black;
using System.Text;

namespace PoeRedux.Patches;

public class BlackScreen : IPatch
{
    public string Name => "Black Screen Patch (Experimental)";
    public object Description => "Makes the screen black.";

    public void Apply(DirectoryNode root)
    {
            new Aoc().Apply(root);
            new Env().Apply(root);
            new Epk().Apply(root);
            new Ffx().Apply(root);
            new Hlsl().Apply(root);
            new Mat().Apply(root);
            new Pet().Apply(root);
            new NoCorpse().Apply(root);
    }
}
