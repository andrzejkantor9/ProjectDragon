using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        // [SerializeField] KeyCode toggleKey = KeyCode.Escape;
        [SerializeField]
        private UIInputConfig _uiInputConfig; 
        [SerializeField] GameObject uiContainer = null;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_uiInputConfig, "_uiInputConfig is null");
        }

        // Start is called before the first frame update
        void Start()
        {
            uiContainer.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (_uiInputConfig.WasShowHideUIPressedThisFrame)
            {
                uiContainer.SetActive(!uiContainer.activeSelf);
            }
        }
    }
}