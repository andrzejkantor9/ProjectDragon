using UnityEngine;

using TMPro;

using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        #region Cache
        [Header("CACHE")]
        [SerializeField]
        private TextMeshProUGUI _title;
        [SerializeField]
        private TextMeshProUGUI _progress;
        #endregion

        #region States
        private QuestStatus _questStatus;
        #endregion

        //////////////////////////////////////////////////

        #region PublicMethods
        public QuestStatus Status => _questStatus;
        
        public void Setup(QuestStatus status)
        {
            _questStatus = status;
            _title.text = _questStatus.QuestInQuestion.Title;
            _progress.text = $"{_questStatus.CompletedCount} / {_questStatus.QuestInQuestion.ObjectiveCount}";
        }
        #endregion
    }
}
