using Player;
using UnityEngine;

namespace Items
{
    [AddComponentMenu("Pickup/Star")]
    public class Star : BaseItem
    {
        [SerializeField] private int scoreGiven = 1;
        [SerializeField] private Animator anim;
        private static readonly int Used = Animator.StringToHash("Used");

        protected override void UseActions()
        {
            anim.SetTrigger(Used);
            // give score to the player
            Score.OnIncreaseScore(scoreGiven);
        }
    }
}