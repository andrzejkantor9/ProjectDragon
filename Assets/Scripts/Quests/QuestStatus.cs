using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        #region States
        public Quest QuestInQuestion {get; private set;}
        private List<string> _completedObjectives = new List<string>();
        private object objectState;
        #endregion

        #region Data
        [System.Serializable]
        class QuestStatusRecord
        {
            public string QuestName {get; private set;}
            public List<string> CompletedObjectives {get; private set;}

            public QuestStatusRecord(string name, List<string> completedObjectives)
            {
                QuestName = name;
                CompletedObjectives = completedObjectives;
            }
        }
        #endregion

        ///////////////////////////////////////////////////

        #region Constructors
        public QuestStatus(Quest quest)
        {
            QuestInQuestion = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            QuestInQuestion = Quest.GetByName(state.QuestName);
            _completedObjectives = state.CompletedObjectives;
        }
        #endregion

        #region PublicMethods
        public int CompletedCount => _completedObjectives.Count;

        public bool IsObjectiveComplete(string objective) => _completedObjectives.Contains(objective);

        public bool IsCompleted()
        {
            foreach(Quest.Objective objective in QuestInQuestion.Objectives)
            {
                if(!_completedObjectives.Contains(objective.Reference))
                    return false;
            }

            return true;
        }

        public void CompleteObjective(string objective)
        {
            if(QuestInQuestion.HasObjective(objective))
            {
                _completedObjectives.Add(objective);
            }
        }

        public object CaptureState()
        {
            QuestStatusRecord state = new QuestStatusRecord(QuestInQuestion.name, _completedObjectives);
            return state;
        }
        #endregion
    }
}
