using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using RPG.Core;
using RPG.Debug;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private string _playerName;
        private Dialogue _currentDialogue;
        #endregion

        #region States
        [Header("STATES")]
        [SerializeField]
        private AIConversant _currentAIConversant;
        private DialogueNode _currentNode;
        public bool IsChoosing {get; private set;}
        #endregion

        #region Events
        public event Action onConversationUpdated;
        #endregion

        ////////////////////////////////////////////////////////////

        #region PublicMethods
        public string GetText()
        {
            if(!_currentNode)
            {
                return "this prolly ain't the righteous text...";
            }
            return _currentNode.Text;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(_currentDialogue.GetPlayerChildren(_currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            CustomLogger.Log($"set current dialogue text to: {chosenNode.Text}", LogFrequency.Rare);
            TriggerEnterAction();
            IsChoosing = false;
            NextDialogue();
        }

        public void NextDialogue()
        {
            int playerResponsesCount = FilterOnCondition(_currentDialogue.GetPlayerChildren(_currentNode)).Count();
            if(playerResponsesCount > 0)
            {
                IsChoosing = true;
                TriggerExitAction();
                if(onConversationUpdated != null)
                    onConversationUpdated();
                return;
            }

            TriggerExitAction();
            DialogueNode[] childNodes = FilterOnCondition(_currentDialogue.GetAIChildren(_currentNode)).ToArray();
            if(childNodes.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, childNodes.Length);
                _currentNode = childNodes[randomIndex];
                TriggerEnterAction();
            }
            
            if(onConversationUpdated != null)
                onConversationUpdated();
        }

        public bool HasNext() 
        {
            return FilterOnCondition(_currentDialogue.GetAllChildren(_currentNode)).Count() > 0;
        }

        public void StartDialogue(AIConversant aIConversant, Dialogue newDialogue)
        {
            _currentAIConversant = aIConversant;
            _currentDialogue = newDialogue;
            _currentNode = _currentDialogue.GetRootNode();
            TriggerEnterAction();
            if(onConversationUpdated != null)
                onConversationUpdated();
        }

        public void QuitDialogue()
        {
            TriggerExitAction();
            _currentDialogue = null;
            _currentAIConversant = null;
            _currentNode = null;
            IsChoosing = false;
            if(onConversationUpdated != null)
                onConversationUpdated();
        }
        public bool IsActive() => _currentDialogue != null;

        public string GetConversantName()
        {
            if(IsChoosing)
                return _playerName;

            return _currentAIConversant.ConversantName;
        }
        #endregion

        #region PrivateMethods
        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach(DialogueNode node in inputNode)
            {
                if(node.CheckCondition(GetEvaluators()))
                    yield return node;
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if(_currentNode)
                TriggerAction(_currentNode.OnEnterAction);
        }

        private void TriggerExitAction()
        {
            if(_currentNode)
                TriggerAction(_currentNode.OnExitAction);
        }

        private void TriggerAction(string action)
        {
            if(!String.IsNullOrEmpty(action))
            {
                CustomLogger.Log($"execute dialogue action: {action}", LogFrequency.Regular);

                foreach(DialogueTrigger trigger in _currentAIConversant.GetComponents<DialogueTrigger>())
                {
                    trigger.Trigger(action);
                }
            }
        }
        #endregion
    }
}