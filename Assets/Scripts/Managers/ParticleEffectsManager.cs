using System.Collections.Generic;
using Spyro.Optimisation.ObjectManagement;
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
        public GameObject particleEffect;

        private GameObject _particleObject;


        public T Play<T>(Vector3 position, Vector3 normal, int emitAmount = 1) where T : Component
        {
            _particleObject = ObjectManager.DynamicInstantiate(particleEffect);

            var transform = _particleObject.transform;
            transform.position = position;
            transform.forward = normal;
            T element = _particleObject.GetComponent<T>();

            switch (element)
            {
                case ParticleSystem effect:
                    effect.Emit(emitAmount);
                    break;
                
                case TrailRenderer renderer:
                    renderer.SetPosition(0,position);
                    break;
            }

            return element;
        }
    }
}