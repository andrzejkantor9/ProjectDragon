using UnityEngine;

using TMPro;

using Utilities;

using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour 
    {
        #region Cache
        [Header("CACHE")]
        [SerializeField]
        private TextMeshProUGUI _title;
        [SerializeField]
        private Transform _objectiveConainer;
        [SerializeField]
        private GameObject _objectivePrefab;
        [SerializeField]
        private GameObject _objectiveIncompletePrefab;
        [SerializeField]
        private TextMeshProUGUI _rewardText;
        #endregion

        //////////////////////////////////////////////////////////////////

        #region PublicMethods
        public void Setup(QuestStatus status)
        {
            Quest quest = status.QuestInQuestion;
            _title.text = quest.Title;
            _objectiveConainer.GetComponent<DestroyObjects>().DestroyWithSpecifiedParameters();

            foreach(Quest.Objective objective in quest.Objectives)
            {
                GameObject prefab = status.IsObjectiveComplete(objective.Reference) ?
                    _objectivePrefab : _objectiveIncompletePrefab;

                GameObject objectiveInstance = Instantiate(prefab, _objectiveConainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.Description;
            }

            _rewardText.text = GetRewardText(quest);
        }
        #endregion

        #region PrivateMethods
        private string GetRewardText(Quest quest)
        {
            string rewardText = "";
            foreach(Quest.Reward reward in quest.Rewards)
            {
                if(rewardText != "")
                {
                    rewardText += ", ";
                }
                if(reward.Item)
                {
                    rewardText += reward.Number + "x ";
                    rewardText += reward.Item.GetDisplayName();
                }
            }

            if(rewardText == "")
            {
                rewardText = "No reward";
            }
            return rewardText;
        }
        #endregion
    }
}