#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JellyMerge
{
    public class JellyBehaviour : MonoBehaviour
    {
        [SerializeField] Renderer rendererRef;
        [SerializeField] Rigidbody[] topBonesRigidbodies;
        [SerializeField] Transform followingBaseTransform;

        private List<Vector3> defaultBonesPositions;

        private void OnEnable()
        {
            if (defaultBonesPositions == null)
            {
                defaultBonesPositions = new List<Vector3>();

                for (int i = 0; i < topBonesRigidbodies.Length; i++)
                {
                    defaultBonesPositions.Add(topBonesRigidbodies[i].transform.localPosition);
                }
            }
        }

        public void Init(Transform graphicsHolder, ColorId cellColor)
        {
            followingBaseTransform.SetParent(graphicsHolder);
            followingBaseTransform.localPosition = new Vector3(0f, 0.476f, 0f);

            for (int i = 0; i < topBonesRigidbodies.Length; i++)
            {
                topBonesRigidbodies[i].isKinematic = false;
            }

            rendererRef.material = ColorsController.GetColorMaterial(cellColor);
        }

        [Button("Bounce")]
        public void Bounce()
        {
            for (int i = 0; i < topBonesRigidbodies.Length; i++)
            {
                topBonesRigidbodies[i].AddForce(Random.Range(-150f, 150f), Random.Range(100f, 200f), Random.Range(-150f, 150f));
            }
        }

        public void Disable()
        {
            for (int i = 0; i < topBonesRigidbodies.Length; i++)
            {
                topBonesRigidbodies[i].isKinematic = true;
                topBonesRigidbodies[i].transform.localPosition = defaultBonesPositions[i];
            }

            followingBaseTransform.SetParent(transform);
            followingBaseTransform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}