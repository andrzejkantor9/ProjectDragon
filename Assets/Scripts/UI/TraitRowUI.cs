using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using RPG.Stats;
using RPG.Core;

namespace RPG.UI
{
    public class TraitRowUI : MonoBehaviour
    {
        #region Config
        //[Header("CONFIG")]
        #endregion

        #region Cache
        [Header("CACHE")]
    	//[Space(8f)]
        [SerializeField]
        Trait _trait;
        [SerializeField]
        TextMeshProUGUI _valueText;
        [SerializeField]
        Button _minusButton;
        [SerializeField]
        Button _plusButton;

        TraitStore _playerTraitStore = null;
        #endregion

        #region States
        #endregion

        #region Events & Statics
        public static event Action onAllocatePoints;
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        private void Start()
        {
            _playerTraitStore = GameManager.PlayerGameObject().GetComponent<TraitStore>();

            _minusButton.onClick.AddListener(() => Allocate(-1));
            _plusButton.onClick.AddListener(() => Allocate(+1));
            _playerTraitStore.onUpdateUI += AfterAllocate;

            AfterAllocate();
        }

        private void OnEnable()
        {
            onAllocatePoints += AfterAllocate;
        }

        private void OnDisable()
        {
            onAllocatePoints -= AfterAllocate;
        }

        private void OnDestroy() 
        {
            if(_playerTraitStore)
                _playerTraitStore.onUpdateUI -= AfterAllocate;
        }
        #endregion

        #region PublicMethods
        public void Allocate(int points)
        {
            _playerTraitStore.AssignPoints(_trait, points);

            if(onAllocatePoints != null)
                onAllocatePoints();
        }
        #endregion

        #region Interfaces & Inheritance
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        private void AfterAllocate()
        {
            _minusButton.interactable = _playerTraitStore.CanAssignPoints(_trait, -1);
            _plusButton.interactable = _playerTraitStore.CanAssignPoints(_trait, 1);

            _valueText.text = _playerTraitStore.GetProposedPoints(_trait).ToString();
        }
        #endregion
    }
}
