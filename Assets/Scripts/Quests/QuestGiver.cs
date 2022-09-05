using UnityEngine;

using RPG.Core;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        #region Cache
        [Header("CACHE")]
        [SerializeField]
        private Quest _quest;
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////

        #region PublicMethods
        public void GiveQuest()
        {
            QuestsList questsList = GameManager.PlayerGameObject.GetComponent<QuestsList>();
            questsList.AddQuest(_quest);
        }
        #endregion
    }
}