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
        private static readonly int Success = Animator.StringToHash("Success");

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            LevelManager.LevelStarted += () => _animator.SetBool(Show,false);
            LevelManager.LevelEnded += success =>
            {
                _animator.SetBool(Show,true);
                if(success) _animator.SetTrigger(Success);
            };
        }

        public void ButtonStartLevel()
        {
            LevelManager.OnLevelStart();
        }
    }
}