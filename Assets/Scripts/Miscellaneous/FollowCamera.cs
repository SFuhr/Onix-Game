using System;
using UnityEngine;

namespace Miscellaneous
{
    public class FollowCamera : MonoBehaviour
    {
        private static Action<Transform> _setFollowTarget;
        public static void OnSetFollowTarget(Transform target) => _setFollowTarget?.Invoke(target);
        
        [Header("Follow Settings")]
        [SerializeField] [Range(0, 10)] private float speed = 1f;
        [SerializeField] private float yOffset;
        [SerializeField] private bool ignoreX = true;
        [SerializeField] private float yLowestPoint = -100;
        
        private bool TargetExists => _target != null;
        private float ZPos => transform.position.z;
        
        private Transform _target;

        private void OnEnable()
        {
            _setFollowTarget += target => _target = target;
        }

        public void LateUpdate()
        {
            if (!TargetExists) return;

            var pos = transform.position;
            var targetPosition = _target.position;
            
            targetPosition.y += yOffset;
            targetPosition.x = ignoreX ? pos.x : targetPosition.x;
            targetPosition.y = targetPosition.y < yLowestPoint ? yLowestPoint : targetPosition.y;
            
            var distance = Vector2.Distance(pos, targetPosition) * speed;
            
            Vector3 newPos = Vector2.Lerp(pos, targetPosition,Time.deltaTime * distance);
            newPos.z = ZPos;

            transform.position = newPos;
        }
    }
}