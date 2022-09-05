using System.Collections.Generic;

using UnityEngine;

using GameDevTV.Inventories;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]
    public class Quest : ScriptableObject
    {
        #region Cache
        [Header("CACHE")]
        [SerializeField]
        private List<Objective> _objectives = new List<Objective>();
        [SerializeField]
        private List<Reward> _rewards = new List<Reward>();
        #endregion

        #region Data
        [System.Serializable]
        public class Reward
        {
            [field: SerializeField][field: Min(1)]
            public int Number {get; private set;}
            [field: SerializeField]
            public InventoryItem Item {get; private set;}
        }

        [System.Serializable]
        public class Objective
        {
            [field: SerializeField]
            public string Reference {get; private set;}
            [field: SerializeField]
            public string Description {get; private set;}
        }
        #endregion

        /////////////////////////////

        #region PublicMethods
        public string Title => name;
        public int ObjectiveCount => _objectives.Count;
        public IEnumerable<Objective> Objectives => _objectives;
        public IEnumerable<Reward> Rewards => _rewards;

        public bool HasObjective(string inObjective)
        {
            foreach (Objective objective in _objectives)
            {
                if(objective.Reference == inObjective)
                    return true;
            }

            return false;
        }

        public static Quest GetByName(string questName)
        {
            foreach(Quest quest in Resources.LoadAll<Quest>(""))
            {
                if(quest.name == questName)
                    return quest;
            }

            return null;
        }
        #endregion

    }
}