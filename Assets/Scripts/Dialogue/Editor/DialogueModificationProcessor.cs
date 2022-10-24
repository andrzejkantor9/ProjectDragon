using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPG.Dialogue.Editor
{
#if UNITY_EDITOR
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
#endif
}
