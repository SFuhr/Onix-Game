using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UI;
using UnityEngine;

namespace Level
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI savedScore;
        [SerializeField] private TextMeshProUGUI tempScore;
        
        [Tooltip("Save location: game directory/data.save")]
        [SerializeField] private bool saveProgressOnDevice = true;

        [SerializeField] [HideInInspector] private ProgressData data;
        
        private FileStream _file;
        
        private static Action<int> _increaseScore;
        public static void OnIncreaseScore(int score) => _increaseScore?.Invoke(score);
        
        private const int MaxScoreToShow = 99999;
        private static string SaveDirectory => $"{Application.dataPath}/Saves/";
        private static string SaveName => "data.save";
        private static string SavePath => $"{SaveDirectory}{SaveName}";
        
        private void Awake()
        {
            LevelManager.LevelLoaded += LoadData;
            
            LevelManager.LevelStarted += UpdateUiScore;
            
            LevelManager.LevelEnded += success =>
            {
                // save or remove temp score
                data.IncreaseLevel(success);
                UpdateUiScore();
                SaveData();
            };
            _increaseScore += value =>
            {
                data.AddTemp(value);
                UpdateUiScore();
            };
        }
        
        private void UpdateUiScore()
        {
            tempScore.text = data.GetTempScore <= 0 ? "" : $"+{data.GetTempScore.ToString()}";
            var score = data.GetSavedScore < MaxScoreToShow ? $"{data.GetSavedScore.ToString()}" : $"{MaxScoreToShow.ToString()}+";
            savedScore.text = score;
        }

        private void LoadData()
        {
            if (saveProgressOnDevice)
            {
                // load saved data
                CheckDirectory();

                if (File.Exists(SavePath))
                {
                    _file = File.OpenRead(SavePath);
                    
                    BinaryFormatter bf = new BinaryFormatter();
                    var save = (ProgressData) bf.Deserialize(_file);
                    data = save;
                    
                    _file.Close();
                    
                    UpdateUiScore();
                }
                
            }
            ProgressBar.Instance.SetCurrentLevel(data.GetLevel);
        }

        private void SaveData()
        {
            if(!saveProgressOnDevice) return;
            CheckDirectory();
            
            _file = File.Exists(SavePath) ? File.OpenWrite(SavePath) : File.Create(SavePath);
            
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(_file,data);
                
            _file.Close();
        }

        private void CheckDirectory()
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }
        
        [Serializable]
        public class ProgressData
        {
            [SerializeField] private int level;
            [SerializeField] private int savedScore;
            [SerializeField] private int tempScore;

            public void AddTemp(int value)
            {
                tempScore += value;
            }

            public void IncreaseLevel(bool success)
            {
                if (success)
                {
                    savedScore += tempScore;
                    level++;
                    ProgressBar.Instance.SetCurrentLevel(level);
                }
                tempScore = 0;
            }

            public int GetLevel => level;
            public int GetSavedScore => savedScore;
            public int GetTempScore => tempScore;
        }
    }
}