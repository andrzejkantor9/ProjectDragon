using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Cache
        [SerializeField]
        Vector2 _newNodeOffset = new Vector2(250f, 0f);
        [SerializeField]
        private List<DialogueNode> _nodes = new List<DialogueNode>();

        [HideInInspector]
        private Dictionary<string, DialogueNode> _nodesLookup = new Dictionary<string, DialogueNode>();
        #endregion

        #region EngineMethods
        
        private void OnValidate()
        {
#if UNITY_EDITOR
            CalculateNodesLookup();
#endif
        }
        #endregion

        #region PublicMethods
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return _nodes;
        }

        public DialogueNode GetRootNode()
        {
            return _nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.ChildrenID)
            {
                if(_nodesLookup.ContainsKey(childID))
                {
                    yield return _nodesLookup[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach(DialogueNode node in GetAllChildren(currentNode))
            {
                if(node.IsPlayerSpeaking)
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach(DialogueNode node in GetAllChildren(currentNode))
            {
                if(!node.IsPlayerSpeaking)
                {
                    yield return node;
                }
            }
        }

#if UNITY_EDITOR
        public void CreateChildNode(DialogueNode parent)
        {
            DialogueNode childNode = CreateNewNode(!parent.IsPlayerSpeaking);
            childNode.SetNodePosition(parent.GetNodeRect().position + _newNodeOffset);
            parent.AddChild(childNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "deleted dialog node");
            _nodes.Remove(nodeToDelete);
            CalculateNodesLookup();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
#endif
        #endregion

        #region Interfaces
        public void OnBeforeSerialize()
        {       
#if UNITY_EDITOR     
            if(_nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(false);
                AddNode(newNode);
            }

            if(!String.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if(String.IsNullOrEmpty(AssetDatabase.GetAssetPath(node)))
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize(){}
        #endregion

        #region PrivateMethods
#if UNITY_EDITOR
        private DialogueNode CreateNewNode(bool isPlayerSpeaking)
        {
            DialogueNode newNode = MakeNode(isPlayerSpeaking);
            Undo.RegisterCreatedObjectUndo(newNode, "create dialogue node");

            Undo.RecordObject(this, "add dialog node");
            AddNode(newNode);
            return newNode;
        }

        private void CalculateNodesLookup()
        {
            _nodesLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                _nodesLookup[node.name] = node;
            }
        }

        private void AddNode(DialogueNode newNode)
        {
            _nodes.Add(newNode);
            CalculateNodesLookup();
        }

        private DialogueNode MakeNode(bool isPlayerSpeaking)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = System.Guid.NewGuid().ToString();
            newNode.SetIsPlayerSpeaking(isPlayerSpeaking);

            return newNode;
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete);
            }
        }
#endif
        #endregion
    }
}