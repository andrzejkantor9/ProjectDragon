using UnityEngine;
using UnityEngine.UI;

using GameDevTV.Inventories;

using RPG.Shops;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        #region Config
        //[Header("CONFIG")]
        #endregion

        #region Cache
        [Header("CACHE")]
    	//[Space(8f)]
        [SerializeField]
        private ItemCategory _category;

        private Button _button;
        #endregion

        #region States
        private Shop _currentShop;
        #endregion

        #region Events
        //[Header("EVENTS")]
    	//[Space(8f)]
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SelectFilter);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(SelectFilter);
        }
        #endregion

        #region PublicMethods
        public void SetShop(Shop currentShop) => _currentShop = currentShop;

        public void RefreshUI()
        {
            _button.interactable = _currentShop.Filter != _category;
        }
        #endregion

        #region Interfaces
        #endregion

        #region Events
        #endregion
	
        #region StaticMethods
        #endregion

        #region PrivateMethods
        private void SelectFilter()
        {
            _currentShop.SelectFilter(_category);
        }
        #endregion
    }
}
