using UnityEngine;
using UnityEngine.UI;

using TMPro;

using RPG.Debug;
using RPG.Dialogue;
using RPG.Core;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour 
    {
        #region Cache
        [Header("CACHE")]
        [SerializeField]
        private Button _nextButton;
        [SerializeField]
        TextMeshProUGUI _AIText;
        [SerializeField]
        private GameObject _AIResponseGameObject;
        [SerializeField]
        private Transform _choiceRoot;
        [SerializeField]
        private GameObject _choiceButtonPrefab;
        [SerializeField]
        private Button _quitButton;
        [SerializeField]
        private TextMeshProUGUI _conversantName;
        private PlayerConversant _playerConversant;
        #endregion

        ////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Start()
        {
            if(CheckAndSetConversant())
            {
                _playerConversant.onConversationUpdated += RefreshUI;
            }
            RefreshUI();
        }

        private void OnEnable() 
        {
            _nextButton.onClick.AddListener(() => _playerConversant.NextDialogue());
            _quitButton.onClick.AddListener(() => _playerConversant.QuitDialogue());
        }

        private void OnDestroy() 
        {
            if(CheckAndSetConversant())
            {
                _playerConversant.onConversationUpdated -= RefreshUI;
            }        
        }
        #endregion

        #region PrivateMethods

        private void RefreshUI()
        {
            gameObject.SetActive(_playerConversant.IsActive()); 
            if(!_playerConversant.IsActive()) 
                return;

            _conversantName.text = _playerConversant.GetConversantName();
            _AIResponseGameObject.SetActive(!_playerConversant.IsChoosing);
            _choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing);
            
            if(_playerConversant.IsChoosing)
            {
                BuildChoiceList();
            }
            else
            {
                _AIText.text = _playerConversant.GetText();
                _nextButton.gameObject.SetActive(_playerConversant.HasNext());
                CustomLogger.Log($"ai text: {_AIText.text}", LogFrequency.Regular);
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform child in _choiceRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (DialogueNode choiceNode in _playerConversant.GetChoices())
            {
                GameObject prefab = Instantiate(_choiceButtonPrefab, _choiceRoot);
                //if tmp_text is null, it is wrong prefab
                prefab.GetComponentInChildren<TMP_Text>().text = choiceNode.Text;

                Button button = prefab.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    _playerConversant.SelectChoice(choiceNode);
                });
            }
        }

        private bool CheckAndSetConversant()
        {
            if(!_playerConversant && GameManager.PlayerGameObject())
            {
                _playerConversant = GameManager.PlayerGameObject().GetComponent<PlayerConversant>();
            }
            bool isSet = _playerConversant != null;
            if(!isSet)
            {
                UnityEngine.Debug.LogWarning("missing player player converstant script");
            }

            return _playerConversant;
        }
        #endregion
    }
}