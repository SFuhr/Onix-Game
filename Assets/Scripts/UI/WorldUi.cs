using Level;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WorldUi : MonoBehaviour
    {
        [SerializeField] private Button playButton;

        private Animator _animator;
        
        private static readonly int Show = Animator.StringToHash("Show");

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            LevelManager.LevelStart += () => _animator.SetBool(Show,false);
            LevelManager.LevelEnd += b => _animator.SetBool(Show,true);
        }

        public void ButtonStartLevel()
        {
            LevelManager.OnLevelStart();
        }
    }
}