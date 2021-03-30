using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Effects
{
    public class ParticleFXManager : MonoBehaviour
    {
        #region Singleton Imp

        private static ParticleFXManager _ins;

        public static ParticleFXManager Instance
        {
            get
            {
                _ins = _ins
                    ? _ins
                    : FindObjectOfType<ParticleFXManager>() ??
                      new GameObject("Particle FX Manager").AddComponent<ParticleFXManager>();
                return _ins;
            }
        }

        #endregion

        public List<ParticleFX> particleFXs = new List<ParticleFX>();


        [Serializable]
        public struct ParticleFX
        {
            public string particleName;
            public ParticleSystem particleFXPrefab;


            public static bool operator !=(ParticleFX a, ParticleFX b)
            {
                return a.particleName != b.particleName;
            }

            public static bool operator ==(ParticleFX a, ParticleFX b)
            {
                return !(a != b);
            }
        }


        public void PlayFX(string fxName, Vector3 fxPos)
        {
            ParticleFX fx;
            fx = particleFXs.FirstOrDefault(p => p.particleName.Equals(fxName));


            if (fx != default)
                StartCoroutine(
                    PlayAndResetFX(ObjectManager.DynamicComponentInstantiate(fx.particleFXPrefab, false, 50), fxPos));
        }

        private IEnumerator PlayAndResetFX(ParticleSystem dynamicComponentInstantiate, Vector3 spawnPos)
        {
            dynamicComponentInstantiate.gameObject.SetActive(true);
            dynamicComponentInstantiate.transform.position = spawnPos;
            dynamicComponentInstantiate.Play();
            while (dynamicComponentInstantiate.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }

            dynamicComponentInstantiate.gameObject.SetActive(false);
        }
    }
}