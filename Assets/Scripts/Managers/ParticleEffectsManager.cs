using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ParticleEffectsManager : MonoBehaviour
    {
        public static ParticleEffectsManager PublicInstance
        {
            get
            {
                _instance = _instance ? _instance : FindObjectOfType<ParticleEffectsManager>();
                return _instance;
            }
        }


        public List<ParticleEffect> particleEffects = new List<ParticleEffect>();
        private static ParticleEffectsManager _instance;

        public ParticleEffect FindEffect(string effectsName)
        {
            for (int i = 0; i < particleEffects.Count; i++)
            {
                if (particleEffects[i].particleEffectName.ToLower().Equals(effectsName.ToLower()))
                    return particleEffects[i];
            }

            return null;
        }
    }

    [System.Serializable]
    public class ParticleEffect
    {
        public string particleEffectName;
        public ParticleSystem particleEffect;

        private ParticleSystem _particleObject;


        public void Play(Vector3 position, Vector3 normal, int emitAmount = 1)
        {
            if (_particleObject.Equals(null))
            {
                _particleObject = Object.Instantiate(particleEffect);
            }

            var transform = _particleObject.transform;
            transform.position = position;
            transform.forward = normal;

            _particleObject.Emit(emitAmount);
        }
    }
}