using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

using GameDevTV.Utils;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        #region Config
        [field: SerializeField]
        public bool IsPlayerSpeaking {get; private set;} = false;
        [field: SerializeField]
        public string Text {get; private set;}
        [field: SerializeField]
        public List<string> ChildrenID {get; private set;} = new List<string>();
        [field: SerializeField]
        private Rect _nodePosition  = new Rect(0f, 0f, 200f, 100f);
        [field: SerializeField]
        public string OnEnterAction {get; private set;} = "";
        [field: SerializeField]
        public string OnExitAction {get; private set;} = "";
        [SerializeField]
        private Condition _condition;
        #endregion

        #region PublicMethods
        public Rect GetNodeRect() => _nodePosition;
#if UNITY_EDITOR
        public void SetText(string text)
        {
            if(text != Text)
            {
                Undo.RecordObject(this, "change dialogue text");
                Text = text;
                EditorUtility.SetDirty(this);
            }
        }
        public void SetNodePosition(Vector2 position)
        {
            Undo.RecordObject(this, "change node position");
            _nodePosition.position = position;
            EditorUtility.SetDirty(this);
        }

        public void AddChild(DialogueNode node)
        {
            Undo.RecordObject(this, "add child node");
            ChildrenID.Add(node.name);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(DialogueNode node)
        {
            Undo.RecordObject(this, "remove child node");
            ChildrenID.Remove(node.name);
            EditorUtility.SetDirty(this);
        }

        public void SetIsPlayerSpeaking(bool isPlayerSpeaking)
        {
            Undo.RecordObject(this, "set IsPlayerSpeaking");
            IsPlayerSpeaking = isPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return _condition.Check(evaluators);
        }
        public bool HasChild(DialogueNode node) => ChildrenID.Contains(node.name);
        #endregion
    }
}