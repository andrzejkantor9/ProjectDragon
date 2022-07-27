using System;

using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        #region States
        private IAction _currentAction;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region PublicFunctionality
        public void StartAction(IAction action)
        {
            if(_currentAction == action) 
                return;

            if(_currentAction != null)
                _currentAction.Cancel();
                
            _currentAction = action;
        }
        #endregion
    }
}