using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{    
    public class SavingSystem : MonoBehaviour
    {
        private const string LAST_SCENE_ID = "lastSceneBuildIndex";
        #region PublicMethods
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            if(state.ContainsKey(LAST_SCENE_ID))
            {
                sceneBuildIndex = (int)state[LAST_SCENE_ID];                    
            }

            yield return SceneManager.LoadSceneAsync(sceneBuildIndex);
            RestoreState(state);
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }       

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }    

        public void Delete(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            File.Delete(path);
            Logger.Log($"delete save from: {path}", LogFrequency.Sporadic);
        }   
        #endregion

        #region PrivateMethods
        private void SaveFile(string saveFile, object state)
        {
            if(state == null) return;

            string path = GetPathFromSaveFile(saveFile);
            Logger.Log($"saving to: {path}", LogFrequency.Sporadic);

            using (FileStream fileStream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, state);
            }
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            Logger.Log($"loading from: {path}", LogFrequency.Sporadic);

            if(File.Exists(path))
            {
                using (FileStream fileStream = File.Open(path, FileMode.Open))
                {
                    if(fileStream.Length == 0) return new Dictionary<string, object>();

                    BinaryFormatter formatter = new BinaryFormatter();
                    return (Dictionary<string, object>)formatter.Deserialize(fileStream);
                }
            }

            return new Dictionary<string, object>();
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                state[saveableEntity.GetUniqueIndentifier()] = saveableEntity.CaptureState();
            }

            state[LAST_SCENE_ID] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                string key = saveableEntity.GetUniqueIndentifier();
                if (state.ContainsKey(key))
                    saveableEntity.RestoreState(state[key]);
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            string path = Path.Combine(Application.persistentDataPath, $"{saveFile}.sav");
            return path;
        }
        #endregion
    }
}
