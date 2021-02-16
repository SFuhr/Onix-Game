using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class Boot : MonoBehaviour
    {
        [SerializeField] private int nextLevelId;

        private void OnEnable()
        {
            SceneManager.LoadSceneAsync(nextLevelId, LoadSceneMode.Additive);
        }
    }
}