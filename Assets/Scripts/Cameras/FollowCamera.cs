using System;
using UnityEngine;

namespace Cameras
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform target;
        [SerializeField] [Range(0, 10)] private float speed = 1f;
        [SerializeField] private float yOffset;
        [SerializeField] private bool ignoreX = true;

        private bool TargetExists => target != null;
        private float ZPos => transform.position.z;
        
        public void LateUpdate()
        {
            if (!TargetExists) return;

            var pos = transform.position;
            var targetPosition = target.position;
            targetPosition.y += yOffset;
            var distance = Vector2.Distance(pos, targetPosition) * speed;
            
            targetPosition.x = ignoreX ? pos.x : targetPosition.x;

            Vector3 newPos = Vector2.Lerp(pos, targetPosition,Time.deltaTime * distance);
            newPos.z = ZPos;

            transform.position = newPos;
        }
    }
}