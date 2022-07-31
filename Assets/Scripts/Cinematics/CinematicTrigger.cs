using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        #region States
        private bool _hasPlayed;
        #endregion

        ////////////////////////////////////////////////////////////

        private void OnTriggerEnter(Collider other)
        {
            if(!_hasPlayed && other.CompareTag(Enums.EnumToString<Tags>(Tags.Player)))
            {
                GetComponent<PlayableDirector>().Play();
                _hasPlayed = true;
            }
        }
    }
}
