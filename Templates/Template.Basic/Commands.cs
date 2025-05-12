using Autodesk.AutoCAD.Runtime;

namespace Basic;

public class Commands
{
    [CommandMethod("Test")]
    public static void Execute()
    {
        Active.Editor.WriteMessage("\nHello World!");
    }
}