using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SavingInputConfig", menuName = "Input/SavingConfig")]
public class SavingInputConfig : ScriptableObject
{
    #region Config
    [field: SerializeField]
    public InputAction Save {get; private set;}
    [field: SerializeField]
    public InputAction Load {get; private set;}
    [field: SerializeField]
    public InputAction Delete {get; private set;}
    #endregion

    #region EngineMethods
    private void OnEnable()
    {
        Save.Enable();   
        Load.Enable();   
        Delete.Enable();   
    }
    private void OnDisable()
    {
        Save.Disable();   
        Load.Disable();   
        Delete.Disable();   
    }
    #endregion

    #region PublicMethods
    public bool WasSavePressedThisFrame => WasPressedThisFrame(Save);
    public bool WasLoadPressedThisFrame => WasPressedThisFrame(Load);
    public bool WasDeleteSavePressedThisFrame => WasPressedThisFrame(Delete);
    #endregion

    #region PrivateMethods
    private bool WasPressedThisFrame(InputAction inputAction) => inputAction.WasPressedThisFrame();
    #endregion
}