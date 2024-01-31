#pragma warning disable 0649

using UnityEngine;

namespace Watermelon.JellyMerge
{
    public class SimpleJellyBehaviour : MonoBehaviour
    {
        [SerializeField] Renderer rendererRef;
        [SerializeField] Animator animatorRef;

        private int upParameter;
        private int downParameter;
        private int leftParameter;
        private int rightParameter;
        private int bounceParameter;

        private int animatorMovementLayerIndex;

        private void Awake()
        {
            upParameter = Animator.StringToHash("Forward");
            downParameter = Animator.StringToHash("Back");
            leftParameter = Animator.StringToHash("Left");
            rightParameter = Animator.StringToHash("Right");
            bounceParameter = Animator.StringToHash("Bounce");

            animatorMovementLayerIndex = animatorRef.GetLayerIndex("Movement Layer");
        }

        public void Init(Transform graphicsHolder, ColorId cellColor)
        {
            transform.SetParent(graphicsHolder);
            transform.localPosition = new Vector3(0f, 0f, 0f);

            rendererRef.material = ColorsController.GetColorMaterial(cellColor);
        }

        public void PlayMoveAnimation(Vector2Int movementVector, float movementStrength)
        {
            animatorRef.SetLayerWeight(animatorMovementLayerIndex, movementStrength);

            if (movementVector.y > 0)
            {
                animatorRef.SetTrigger(downParameter);
            }
            else if (movementVector.y < 0)
            {
                animatorRef.SetTrigger(upParameter);
            }
            else if (movementVector.x > 0)
            {
                animatorRef.SetTrigger(leftParameter);
            }
            else if (movementVector.x < 0)
            {
                animatorRef.SetTrigger(rightParameter);
            }
        }

        public void Bounce()
        {
            animatorRef.SetTrigger(bounceParameter);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}