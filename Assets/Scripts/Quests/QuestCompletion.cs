using UnityEngine;

using RPG.Core;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private Quest _quest;
        [SerializeField]
        private string _objective;
        #endregion

        ////////////////////////////////////////////////////

        #region PublicMethods
        public void CompleteObjective()
        {
            QuestsList questsList = GameManager.PlayerGameObject.GetComponent<QuestsList>();
            questsList.CompleteObjective(_quest, _objective);
            Logger.Log($"complete objective: {_objective}", LogFrequency.Rare);
        }
        #endregion

    }
}