using UnityEngine;

using RPG.Quests;
using RPG.Core;
using Utilities;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        #region Cache
        [SerializeField]
        private QuestItemUI _questPrefab;
        private QuestsList _questList;
        #endregion

        //////////////////////////////////////////////////////////

        #region EngineMethods
        private void Start()
        {
            _questList = GameManager.PlayerGameObject().GetComponent<QuestsList>();
            if (!_questList)
                UnityEngine.Assertions.Assert.IsNotNull(_questList, "questList is null");

            _questList.onQuestUpdate += ReSetupUI;
            ReSetupUI();
        }

        private void ReSetupUI()
        {
            GetComponent<DestroyObjects>().DestroyWithSpecifiedParameters();
            foreach (QuestStatus status in _questList.Statuses)
            {
                var uiInstance = Instantiate<QuestItemUI>(_questPrefab, transform);
                uiInstance.Setup(status);
            }
        }
        #endregion
    }
}
