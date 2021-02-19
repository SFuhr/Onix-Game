using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class Boot : MonoBehaviour
    {
        [SerializeField] private int nextLevelId;

        [Header("Settings")]
        [SerializeField] private bool useVSync = true;

        private void OnEnable()
        {
            QualitySettings.vSyncCount = useVSync ? 1 : -1;

            SceneManager.LoadSceneAsync(nextLevelId, LoadSceneMode.Additive);
        }
    }
}