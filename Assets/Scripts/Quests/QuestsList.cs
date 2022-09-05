using System;
using System.Collections.Generic;

using UnityEngine;

using GameDevTV.Inventories;

using RPG.Core;

namespace RPG.Quests
{
    public class QuestsList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        #region States
        private List<QuestStatus> _statuses = new List<QuestStatus>();
        #endregion

        #region Events
        public event Action onQuestUpdate;
        #endregion

        ///////////////////////////////////////////////////

        #region PublicMethods
        public IEnumerable<QuestStatus> Statuses => _statuses;

        public void AddQuest(Quest quest)
        {
            if(!HasQuest(quest))
            {
                QuestStatus newStatus = new QuestStatus(quest);
                _statuses.Add(newStatus);
                onQuestUpdate?.Invoke();
            }
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);

            if(status.IsCompleted())
            {
                GiveReward(quest);
            }
            onQuestUpdate?.Invoke();
        }
        #endregion

        #region PrivateMethods
        private bool HasQuest(Quest quest) => GetQuestStatus(quest) != null;

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach(QuestStatus status in _statuses)
            {
                if(status.QuestInQuestion == quest)
                    return status;
            }

            return null;
        }
        
        private void GiveReward(Quest quest)
        {
            foreach(Quest.Reward reward in quest.Rewards)
            {
                bool wasRewarded = GetComponent<Inventory>().AddToFirstEmptySlot(reward.Item, reward.Number);
                if(!wasRewarded)
                {
                    GetComponent<ItemDropper>().DropItem(reward.Item, reward.Number);
                }
            }
        }
        #endregion

        #region Interfaces
        public object CaptureState()
        {
            List<object> state = new List<object>();

            foreach(QuestStatus status in _statuses)
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if(stateList == null)
            {
                Debug.LogError("state list in save file is null");
                return;
            }

            _statuses.Clear();
            foreach(object objectState in stateList)
            {
                _statuses.Add(new QuestStatus(objectState));
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "HasQuest":
                return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest":
                return GetQuestStatus(Quest.GetByName(parameters[0])).IsCompleted();
            }

            return null;
        }
        #endregion
    }
}
