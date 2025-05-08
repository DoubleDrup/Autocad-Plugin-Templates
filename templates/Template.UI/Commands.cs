using Autodesk.AutoCAD.Runtime;

namespace UI;

public class Commands
{
    [CommandMethod("Test")]
    public static void Execute()
    {
        Active.Editor.WriteMessage("\nHello World!");
    }
}