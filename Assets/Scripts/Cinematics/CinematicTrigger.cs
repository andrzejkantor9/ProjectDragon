using UnityEngine;
using UnityEngine.Playables;

using RPG.Core;

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
            if(!_hasPlayed && other.CompareTag(Enums.EnumToString<Enums.Tags>(Enums.Tags.Player)))
            {
                GetComponent<PlayableDirector>().Play();
                _hasPlayed = true;
            }
        }
    }
}
