using Autodesk.AutoCAD.Runtime;

namespace AutoCAD.Template.Basic;

public class Commands
{
    [CommandMethod("Test")]
    public static void Execute()
    {
        Active.Editor.WriteMessage("\nHello World!");
    }
}