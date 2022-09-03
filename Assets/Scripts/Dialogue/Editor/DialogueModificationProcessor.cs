using System.IO;

using UnityEditor;

namespace RPG.Dialogue.Editor
{
    public class DialogueModificationProcessor : AssetModificationProcessor
    {
        //it was workaround for when renaming scriptable object changed itselt with its sub-asset
        // private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        // {
        //     Dialogue dialogue = AssetDatabase.LoadMainAssetAtPath(sourcePath) as Dialogue;
        //     if(dialogue && Path.GetDirectoryName(sourcePath) == Path.GetDirectoryName(destinationPath))
        //     {
        //         dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);
        //     }

        //     return AssetMoveResult.DidNotMove;
        // }
    }
}
