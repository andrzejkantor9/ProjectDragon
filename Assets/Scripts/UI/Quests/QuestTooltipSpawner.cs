using UnityEngine;

using GameDevTV.Core.UI.Tooltips;

using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus questStatus = GetComponent<QuestItemUI>().Status;
            tooltip.GetComponent<QuestTooltipUI>().Setup(questStatus);
        }
    }
}