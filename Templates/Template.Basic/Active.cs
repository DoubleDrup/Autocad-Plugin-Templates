using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace Basic
{
    public static class Active
    {
        public static Document Document 
            => Application.DocumentManager.MdiActiveDocument;
        
        public static Editor Editor 
            => Document.Editor;
        
        public static Database Database 
            => Document.Database;
    }
}