using UnityEngine;
using UnityEngine.InputSystem;

namespace GameDevTV.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [field: SerializeField]
        public InputAction ShowHideUIInputAction {get; private set;}
        // [SerializeField]
        // private UIInputConfig _uiInputConfig; 
        [SerializeField] GameObject _uiContainer = null;

        ///////////////////////////////////////////////////////////////

        void Start()
        {
            _uiContainer.SetActive(false);
        }

        private void OnEnable()
        {
            ShowHideUIInputAction.Enable();   
        }
        private void OnDisable()
        {
            ShowHideUIInputAction.Disable();   
        }

        void Update()
        {
            if (ShowHideUIInputAction.WasPressedThisFrame())
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            _uiContainer.SetActive(!_uiContainer.activeSelf);
        }
    }
}