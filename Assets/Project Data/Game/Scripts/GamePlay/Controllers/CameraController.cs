using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JellyMerge
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private static CameraController instance;

        [Header("Settings")]
        public float oneUnitMoveTime = 0.1f;

        [Header("References")]
        public Transform backPlaneTransform;

        private Camera cameraRef;
        private Transform transformRef;

        private static float heightToWidthRelation = 1f;

        public static Vector2 FrustrumSize
        {
            get
            {
                float frustumHeight = 2.0f * (instance.transformRef.position.y * 2) * Mathf.Tan(instance.cameraRef.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float frustumWidth = frustumHeight * instance.cameraRef.aspect;

                return new Vector2(frustumWidth, frustumHeight);
            }
        }

#if UNITY_EDITOR
        private Vector3 levelCenterCached;
        private Vector2Int levelSizeCached;
#endif

        private void Awake()
        {
            instance = this;
            transformRef = transform;
            cameraRef = GetComponent<Camera>();

            Vector3 bottomLeftPosition = cameraRef.ScreenToWorldPoint(Vector3.zero.SetZ(transform.position.y));
            Vector3 topRightPosition = cameraRef.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height).SetZ(transform.position.y));

            heightToWidthRelation = transform.position.y / (topRightPosition.x - bottomLeftPosition.x);

        }

        public static void Init(Vector3 levelCenter, Vector2Int levelSize, bool smoothMovement = false)
        {
            instance.InitCamera(levelCenter, levelSize, smoothMovement);
        }

        private void InitCamera(Vector3 levelCenter, Vector2Int levelSize, bool smoothMovement = false)
        {
            float cameraHeight = 1;

            float playgroundWidth = (levelSize.x > levelSize.y ? levelSize.x : levelSize.y) + 1.5f;
            cameraHeight = playgroundWidth * heightToWidthRelation;

            float zOffset = cameraHeight * Mathf.Tan((90f - transform.eulerAngles.x) * Mathf.Deg2Rad);

            Vector3 position = levelCenter.SetY(cameraHeight).AddToZ(-zOffset);
            float animationTime = 0;

            if (smoothMovement)
            {
                float moveDistance = (transform.position - position).magnitude;

                if (moveDistance != 0)
                {
                    animationTime = moveDistance * oneUnitMoveTime;
                    transform.DOMove(position, animationTime);
                }
            }
            else
            {
                transform.position = position;
            }

            // backplane setup
            backPlaneTransform.position = levelCenter;

            Vector3 scale = new Vector3(levelSize.x, levelSize.y, 1f);

            if (smoothMovement)
                backPlaneTransform.DOScale(scale, animationTime);
            else
                backPlaneTransform.localScale = scale;


#if UNITY_EDITOR
            instance.levelCenterCached = levelCenter;
            instance.levelSizeCached = levelSize;
#endif
        }

    }
}