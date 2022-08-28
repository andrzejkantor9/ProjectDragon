using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UIInputConfig", menuName = "Input/UIConfig", order = 0)]
public class UIInputConfig : ScriptableObject
{
    #region Config
    [field: SerializeField]
    public InputAction ShowHideUI {get; private set;}
    #endregion

    #region EngineMethods
    private void OnEnable()
    {
        ShowHideUI.Enable();   
    }
    private void OnDisable()
    {
        ShowHideUI.Disable();   
    }
    #endregion

    #region PublicMethods
    public bool WasShowHideUIPressedThisFrame => ShowHideUI.WasPressedThisFrame();
    #endregion
}