using UnityEngine;

//Program Files (x86)\Unity\<engine version>\Editor\Data\Resources\ScriptTemplates
namespace RPG.UI
{
    public class UISwitcher : MonoBehaviour
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        GameObject _defaultUI;
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
        #endregion

        #region States
        #endregion

        #region Events & Statics
        //[Header("EVENTS")]
    	//[Space(8f)]
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        private void Start() 
        {
            SwitchTo(_defaultUI);
        }
        #endregion

        #region PublicMethods
        public void SwitchTo(GameObject ToDisplay)
        {
            if(ToDisplay.transform.parent != transform)
                return;

            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(child.gameObject == ToDisplay);
            }
        }
        #endregion

        #region Interfaces & Inheritance
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        #endregion
    }
}
