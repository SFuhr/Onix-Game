using System;
using Items;
using Level;
using TMPro;
using UnityEngine;

namespace Player
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI savedScore;
        [SerializeField] private TextMeshProUGUI tempScore;

        [SerializeField] private SaveData data;
        
        private static Action<int> _increaseScore;
        public static void OnIncreaseScore(int score) => _increaseScore?.Invoke(score);

        private void Awake()
        {
            LevelManager.LevelStarted += () => UpdateUiScore();
            
            LevelManager.LevelEnded += success =>
            {
                // save or remove temp score
                data.LevelEnded(success);
                UpdateUiScore();
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
            savedScore.text = data.GetSavedScore.ToString();
        }
        
        [Serializable]
        public struct SaveData
        {
            [SerializeField] private int savedScore;
            [SerializeField] private int tempScore;

            public void AddTemp(int value)
            {
                tempScore += value;
            }

            public void LevelEnded(bool success)
            {
                if (success) savedScore += tempScore;
                tempScore = 0;
            }

            public int GetSavedScore => savedScore;
            public int GetTempScore => tempScore;
        }
    }
}