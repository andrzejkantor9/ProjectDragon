using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using UnityEditor;

using RPG.Core;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField]
        string _uniqueIdentifier = String.Empty;
        static Dictionary<string, SaveableEntity> uuidDictionary = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return _uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDictionary = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if(stateDictionary.ContainsKey(typeString))
                    saveable.RestoreState(stateDictionary[typeString]);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(!Application.IsPlaying(gameObject) && !String.IsNullOrEmpty(gameObject.scene.path))
            {
                SerializedObject serializedObject = new SerializedObject(this);
                SerializedProperty property = serializedObject.FindProperty("_uniqueIdentifier");
                
                if(String.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
                {
                    property.stringValue = Guid.NewGuid().ToString();
                    serializedObject.ApplyModifiedProperties();
                }

                uuidDictionary[property.stringValue] = this;
            }
        }

        private bool IsUnique(string identifier)
        {
            if(!uuidDictionary.ContainsKey(identifier))
                return true;
            if(uuidDictionary[identifier] == this)
                return true;

            if(uuidDictionary[identifier] == null)
            {
                uuidDictionary.Remove(identifier);
                return true;
            }
            if(uuidDictionary[identifier].GetUniqueIdentifier() != identifier)
            {
                uuidDictionary.Remove(identifier);
                return true;
            }

            return false;
        }
#endif
    }
}