using UnityEditor;

namespace Shadee.Dialogues
{
    public static class UndoAndDirty
    {
        public static void Mark(this UnityEngine.Object target, string undoDescription)
        {
#if UNITY_EDITOR
          Undo.RecordObject(target, undoDescription);
          EditorUtility.SetDirty(target);
#endif
        }
    }
}
