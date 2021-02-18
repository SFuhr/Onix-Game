using System;
using UnityEngine;

namespace Miscellaneous
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] [Range(0, 10)] private float speed = 1f;
        [SerializeField] private float yOffset;
        [SerializeField] private bool ignoreX = true;
        [SerializeField] private float yLowestPoint = -100;
        
        private Transform _target;
        
        private static Action<Transform> _setFollowTarget;
        public static void OnSetFollowTarget(Transform target) => _setFollowTarget?.Invoke(target);
        private bool TargetExists => _target != null;
        private float ZPos => transform.position.z;
        private Vector3 Position => transform.position;
        
        private void OnEnable()
        {
            // smooth start camera effect
            transform.position = new Vector3(Position.x,0,Position.z);
            
            _setFollowTarget += target => _target = target;
        }

        public void LateUpdate()
        {
            if (!TargetExists) return;

            var targetPosition = _target.position;
            
            targetPosition.y += yOffset;
            targetPosition.x = ignoreX ? Position.x : targetPosition.x;
            targetPosition.y = targetPosition.y < yLowestPoint ? yLowestPoint : targetPosition.y;
            
            var distance = Vector2.Distance(Position, targetPosition) * speed;
            
            Vector3 newPos = Vector2.Lerp(Position, targetPosition,Time.deltaTime * distance);
            newPos.z = ZPos;

            transform.position = newPos;
        }
    }
}