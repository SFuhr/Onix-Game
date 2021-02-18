using Level;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainCanvas : MonoBehaviour
    {
        [SerializeField] private Button playButton;

        private Animator _animator;
        
        private static readonly int Show = Animator.StringToHash("Show");

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            LevelManager.LevelStarted += () => _animator.SetBool(Show,false);
            LevelManager.LevelEnded += b => _animator.SetBool(Show,true);
        }
    }
}