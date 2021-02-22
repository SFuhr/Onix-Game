using System;
using UnityEngine;

namespace Miscellaneous
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] [Range(0, 10)] private float speed = 1f;
        [SerializeField] private float yOffset;
        [SerializeField] private bool ignoreTargetX = true;
        [SerializeField] private float yLowestPoint = -100;
        
        private Transform _target;
        private float _xOffset;
        
        private bool TargetExists => _target != null;
        private float ZPos => transform.position.z;
        private Vector3 Position => transform.position;

        public void SetTarget(Transform target)
        {
            _target = target;
            _xOffset = _target.position.x;
        }

        public void LateUpdate()
        {
            if (!TargetExists) return;

            var targetPosition = _target.position;
            
            targetPosition.y += yOffset;
            targetPosition.x = ignoreTargetX ? Position.x : targetPosition.x + _xOffset;
            targetPosition.y = targetPosition.y < yLowestPoint ? yLowestPoint : targetPosition.y;
            
            var distance = Vector2.Distance(Position, targetPosition) * speed;
            
            Vector3 newPos = Vector2.Lerp(Position, targetPosition,Time.deltaTime * distance);
            newPos.z = ZPos;

            transform.position = newPos;
        }
    }
}