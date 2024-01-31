using UnityEngine;

namespace JellyMerge
{
    public abstract class ParticleBehaviour : MonoBehaviour
    {
        public abstract void OnParticleActivated();
        public abstract void OnParticleDisabled();
    }
}