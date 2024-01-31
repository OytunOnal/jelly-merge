#pragma warning disable 0649

using System.Collections;
using UnityEngine;
using Watermelon;

namespace Watermelon.JellyMerge
{
    public class CellBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Transform graphicsHolderTransform;

        private Animator animator;
        private Pool simpleCubePool;
        private Pool jellyCubePool;
        private Pool hitParticlePool;
        private Pool disableParticlePool;
        private JellyBehaviour jellyRef;
        private SimpleJellyBehaviour simpleJellyRef;

        private GraphicsType graphicsType;

        private bool disableAfterMove = false;
        private bool wasMoved = false;
        private bool showHitParticle = false;

        private int hideParameter;

        public enum GraphicsType
        {
            Physical,
            Simple,
        }

        private ColorId colorId;
        public ColorId ColorID
        {
            get { return colorId; }
        }

        private bool isBorder;
        public bool IsBorder
        {
            get { return isBorder; }
        }

        public bool IsMovable
        {
            get { return colorId != ColorId.None && !isBorder; }
        }

        public Vector2Int IndexPosition
        {
            get { return new Vector2Int((int)transformRef.position.x, (int)transformRef.position.z); }
        }

        public Vector3 Position
        {
            get { return transformRef.position; }
        }

        private static float animationTime = 0.2f;
        public static float AnimationTime
        {
            get { return animationTime; }
        }

        protected Transform transformRef;

        protected void Awake()
        {
            transformRef = transform;
            animator = GetComponent<Animator>();

            hideParameter = Animator.StringToHash("Hide");
        }

        public void Init(ColorId cellColor, GraphicsType graphicsType)
        {
            disableAfterMove = false;
            transformRef.localScale = Vector3.one;

            colorId = cellColor;

            InitPools();
            InitGraphics(graphicsType);
        }

        private void InitPools()
        {
            simpleCubePool = PoolManager.GetPoolByName("SimpleCube");
            jellyCubePool = PoolManager.GetPoolByName("JellyCube");
            hitParticlePool = PoolManager.GetPoolByName("HitParticle");
            disableParticlePool = PoolManager.GetPoolByName("DisappearingParticle");
        }

        public void InitGraphics(GraphicsType graphicsType)
        {
            this.graphicsType = graphicsType;

            if (graphicsType == GraphicsType.Simple)
            {
                simpleJellyRef = simpleCubePool.GetPooledObject(transformRef.position).GetComponent<SimpleJellyBehaviour>();
                simpleJellyRef.Init(graphicsHolderTransform, colorId);
            }
            else
            {
                jellyRef = jellyCubePool.GetPooledObject(transformRef.position).GetComponent<JellyBehaviour>();
                jellyRef.Init(graphicsHolderTransform, colorId);
            }
        }

        public void Move(Vector2Int vector, bool disableAfterMove)
        {
            this.disableAfterMove = disableAfterMove;
            wasMoved = true;

            if (vector != Vector2Int.zero)
            {
                transformRef.DOMove(transformRef.position + new Vector3(vector.x, 0f, vector.y), animationTime).SetEasing(Ease.Type.SineIn).OnComplete(() => OnMovementComplete());

                if (graphicsType == GraphicsType.Simple)
                {
                    // calculating movement strength for apropriate animations blending depend on movement speed (distance)
                    float strength = Mathf.Clamp(vector.magnitude, 0f, 4f) / 5f * Random.Range(0.85f, 1.15f);
                    simpleJellyRef.PlayMoveAnimation(vector, strength);
                }
            }
            else
            {
                OnMovementComplete();
            }
        }

        public void OnMovementComplete()
        {
            if (disableAfterMove)
            {
                Disable();
            }
            else
            {
                transformRef.position = new Vector3(Mathf.RoundToInt(transformRef.position.x), 0f, Mathf.RoundToInt(transformRef.position.z));
            }

            wasMoved = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!disableAfterMove)
            {
                if (wasMoved)
                {
                    showHitParticle = true;
                }
                else
                {
                    ShowHitParticle();
                }
            }
        }

        private void ShowHitParticle()
        {
            if (Random.Range(0f, 1f) < LevelController.ParticleSpawnChance)
            {
                GameObject particle = hitParticlePool.GetPooledObject(transformRef.position);
                PlaceHitParticle(particle.transform, LevelController.LastMove);
                particle.GetComponent<ParticleSetuper>().SetMaterial(ColorsController.GetColorMaterial(colorId));
            }
        }

        public void PlayHitAnimation(Vector2Int moveDirection)
        {
            StartCoroutine(HitAnimationCoroutine(moveDirection));
        }

        private IEnumerator HitAnimationCoroutine(Vector2Int moveDirection)
        {
            yield return new WaitForSeconds(animationTime * 0.9f);

            if (showHitParticle)
            {
                ShowHitParticle();
            }
            yield return new WaitForSeconds(animationTime * 0.1f);

            if (graphicsType == GraphicsType.Simple)
            {
                simpleJellyRef?.Bounce();
            }
            else
            {
                jellyRef?.Bounce();
            }
        }

        private void PlaceHitParticle(Transform particle1, Vector2Int moveDirection)
        {
            particle1.position = transformRef.position.SetY(0.465f) + new Vector3(moveDirection.x, 0, moveDirection.y) * -0.4f;

            if (moveDirection == Vector2Int.up)
            {
                particle1.localEulerAngles = new Vector3(0f, 0f, 0f);
            }
            else if (moveDirection == Vector2Int.right)
            {
                particle1.localEulerAngles = new Vector3(0f, 90f, 0f);
            }
            else if (moveDirection == Vector2Int.down)
            {
                particle1.localEulerAngles = new Vector3(0f, 180f, 0f);
            }
            else if (moveDirection == Vector2Int.left)
            {
                particle1.localEulerAngles = new Vector3(0f, 270f, 0f);
            }
        }

        public void Hide()
        {
            animator.SetTrigger(hideParameter);
        }

        public void ShowDisappearedParticle()
        {
            disableParticlePool.GetPooledObject(new PooledObjectSettings().SetPosition( transformRef.position).SetEulerRotation(Vector3.right * 90f)).GetComponent<ParticleSetuper>().SetMaterial(ColorsController.GetColorMaterial(colorId));
        }

        public void OnHideAnimationExit()
        {
            Disable();
        }

        private void Disable()
        {
            LevelController.OnCellDeactivated();

            if (simpleJellyRef != null)
            {
                simpleJellyRef.Disable();
            }

            if (jellyRef != null)
            {
                jellyRef.Disable();
            }

            simpleJellyRef = null;
            jellyRef = null;

            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (simpleJellyRef != null)
            {
                simpleJellyRef.Disable();
                simpleJellyRef = null;
            }

            if (jellyRef != null)
            {
                jellyRef.Disable();
                jellyRef = null;
            }
        }
    }
}