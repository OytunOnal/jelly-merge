#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JellyMerge
{
    public class ParticleSetuper : MonoBehaviour
    {
        [SerializeField] GameObject[] particleObjects;

        private List<ParticleSystemRenderer> renderers;

        private void Awake()
        {
            renderers = new List<ParticleSystemRenderer>();

            for (int i = 0; i < particleObjects.Length; i++)
            {
                renderers.Add(particleObjects[i].GetComponent<ParticleSystemRenderer>());
            }
        }

        public void SetMaterial(Material material)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].material = material;
            }
        }
    }
}