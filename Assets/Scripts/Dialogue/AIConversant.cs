using UnityEngine;

using RPG.Interactions;
using RPG.Core;
using RPG.Attributes;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private Dialogue _dialogue;
        [field: SerializeField]
        public string ConversantName {get; private set;}
        #endregion

        ////////////////////////////////////////////////////////////

        #region Interfaces
        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(GameObject playerController)
        {
            HitPoints hitPoints = GetComponent<HitPoints>();
            if (!_dialogue || (hitPoints && hitPoints.IsDead))
                return false;

            if(InputManager.IsPointerPressed())
            {
                playerController.GetComponent<PlayerConversant>().StartDialogue(this, _dialogue);
            }
            return true;
        }
        #endregion
    }
}