using UnityEngine;

namespace RPG.Attributes
{
    public class HitPointsBar : MonoBehaviour
    {
        #region Cache
        [SerializeField]
        private HitPoints _hitPoints;
        [SerializeField]
        private RectTransform _foreground;
        [SerializeField]
        private Canvas _rootCanvas;
        #endregion

        ////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_hitPoints, "_hitPoints is null");
            UnityEngine.Assertions.Assert.IsNotNull(_foreground, "_foreground is null");
            UnityEngine.Assertions.Assert.IsNotNull(_rootCanvas, "_rootCanvas is null");

            _hitPoints.onTakeDamage += EnableHPBar;
        }

        private void OnDestroy()
        {
            _hitPoints.onTakeDamage -= EnableHPBar;
        }

        private void Update()
        {
            float HitPointsFraction = _hitPoints.GetFraction();
            if (ShouldShowHPBar(HitPointsFraction))
            {
                gameObject.SetActive(false);
            }

            _foreground.localScale = new Vector3(HitPointsFraction, 1f, 1f);
        }
        #endregion

        #region Private
        private void EnableHPBar()
        {
            if (ShouldShowHPBar(_hitPoints.GetFraction()))
            {
                gameObject.SetActive(true);
            }
        }

        private bool ShouldShowHPBar(float HitPointsFraction)
        {
            return Mathf.Approximately(HitPointsFraction, 0f) || Mathf.Approximately(HitPointsFraction, 1f);
        }
        #endregion
    }
}