#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

using UnityEngine;

using RPG.Core;
using System;

namespace RPG.Dialogue.Editor
{
#if UNITY_EDITOR
    public class DialogueEditor : EditorWindow
    {
        #region Statics
        private const float CANVAS_SIZE = 4000f;
        private const float BACKGROUND_SIZE = 50f;
        #endregion

        #region States
        private Dialogue _selectedDialogue;
        [NonSerialized]
        private GUIStyle _playerNodeStyle;
        [NonSerialized]
        private GUIStyle _nodeStyle;
        [NonSerialized]
        private DialogueNode _draggedNode;
        [NonSerialized]
        private Vector2 _draggingOffset;
        [NonSerialized]
        private DialogueNode _nodeToCreate;
        [NonSerialized]
        private DialogueNode _nodeToDelete;
        [NonSerialized]
        private DialogueNode _linkingParentNode;
        [NonSerialized]
        private bool _draggingCanvas = false;
        [NonSerialized]
        private Vector2 _draggingCanvasOffset;
        private Vector2 _scrollPosition;
        #endregion

        #region StaticMethods
        [MenuItem("Window/DialogueEditor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "DialogueEditor");
        }

        [OnOpenAsset()]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if(dialogue)
                ShowEditorWindow();

            return dialogue != null;
        }
        #endregion

        #region EngineMethods
        private void OnEnable()
        {
            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            _nodeStyle.normal.textColor = Color.white;
            _nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            _nodeStyle.border = new RectOffset(12, 12, 12, 12);

            _playerNodeStyle = new GUIStyle();
            _playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            _playerNodeStyle.normal.textColor = Color.white;
            _playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            _playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChange()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;
            if(dialogue)
            {
                _selectedDialogue = dialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {      
            if(!_selectedDialogue)
                EditorGUILayout.LabelField("No dialogue selected");
            else
            {
                ProcessEvents();

                EditorGUILayout.LabelField($"{_selectedDialogue.name}");
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect textureCoordinates = new Rect(0f, 0f, CANVAS_SIZE / BACKGROUND_SIZE, CANVAS_SIZE / BACKGROUND_SIZE);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoordinates);

                foreach (var node in _selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (var node in _selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if(_nodeToCreate != null)
                {
                    _selectedDialogue.CreateChildNode(_nodeToCreate);
                    _nodeToCreate = null;
                }
                
                if(_nodeToDelete != null)
                {
                    _selectedDialogue.DeleteNode(_nodeToDelete);
                    _nodeToDelete = null;
                }
            }
        }
        #endregion

        #region PrivateMethods
        private void ProcessEvents()
        {
            if(Event.current.type == EventType.MouseDown && _draggedNode == null)
            {
                _draggedNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                if(_draggedNode != null)
                {
                    _draggingOffset = _draggedNode.GetNodeRect().position - Event.current.mousePosition;
                    Selection.activeObject = _draggedNode;
                }
                else
                {
                    _draggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                    Selection.activeObject = _selectedDialogue;
                }
            }
            else if(Event.current.type == EventType.MouseDrag && _draggedNode != null)
            {
                Undo.RecordObject(_selectedDialogue, "drag dialogue node");
                _draggedNode.SetNodePosition(Event.current.mousePosition + _draggingOffset);
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _draggedNode != null)
            {
                _draggedNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingCanvas)
            {
                _draggingCanvas = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = _nodeStyle;
            if(node.IsPlayerSpeaking)
            {
                style = _playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetNodeRect(), style);

            string newText = EditorGUILayout.TextField(node.Text);

            node.SetText(newText);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-"))
            {
                _nodeToDelete = node;
            }
            DrawLinkButtons(node);

            if (GUILayout.Button("+"))
            {
                _nodeToCreate = node;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetNodeRect().xMax, node.GetNodeRect().center.y);

            foreach (DialogueNode childNode in _selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetNodeRect().xMin, childNode.GetNodeRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0f;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset, 
                    Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode foundNode = null;
            foreach (var node in _selectedDialogue.GetAllNodes())
            {
                if(node.GetNodeRect().Contains(mousePosition))
                    foundNode = node;
            }

            return foundNode;
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (_linkingParentNode == null)
            {
                if (GUILayout.Button("LINK"))
                {
                    _linkingParentNode = node;
                }
            }
            else if (_linkingParentNode == node)
            {
                if (GUILayout.Button("CANCEL"))
                {
                    _linkingParentNode = null;
                }

            }
            else if (_linkingParentNode.HasChild(node))
            {
                if (GUILayout.Button("UNLINK"))
                {
                    Undo.RecordObject(_selectedDialogue, "Remove dialogue link");
                    _linkingParentNode.RemoveChild(node);
                    _linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("CHILD"))
                {
                    Undo.RecordObject(_selectedDialogue, "Add dialogue link");
                    _linkingParentNode.AddChild(node);
                    _linkingParentNode = null;
                }
            }
        }
        #endregion
    }
#endif
}
