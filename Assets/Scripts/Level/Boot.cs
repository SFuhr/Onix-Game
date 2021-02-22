using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class Boot : MonoBehaviour
    {
        [SerializeField] private int nextLevelId;

        [Header("Settings")]
        [SerializeField] private bool useVSync = true;
        [SerializeField] [Min(0)] private int maxFrameRate = 60;

        private void OnEnable()
        {
            QualitySettings.vSyncCount = useVSync ? 1 : -1;
            Application.targetFrameRate = !useVSync ? maxFrameRate : Application.targetFrameRate;

            SceneManager.LoadSceneAsync(nextLevelId, LoadSceneMode.Additive);
        }
    }
}