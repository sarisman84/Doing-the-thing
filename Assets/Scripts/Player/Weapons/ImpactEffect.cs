using System.Collections;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
    [CreateAssetMenu(fileName = "New Impact Effect", menuName = "Weapons/Impact Effect", order = 0)]
    public class ImpactEffect : ScriptableObject
    {
        public AudioClip impactClip;
        public ParticleSystem impactEffect;
        public bool useWeaponModifiersToScaleParticleEffect = false;

        private void PlayAudio(Vector3 impactPoint)
        {
            //Play Audio using the clip
        }

        public IEnumerator PlayImpactEffect(Vector3 impactPoint, Vector3 normalDirection, float particleEffectSize = 1f)
        {
            PlayAudio(impactPoint);
            ParticleSystem impactFX = ObjectManager.DynamicComponentInstantiate(impactEffect, false, 500);
            impactFX.gameObject.SetActive(true);
            impactFX.transform.position = impactPoint;
            impactFX.transform.rotation = Quaternion.LookRotation(normalDirection.normalized, Vector3.up);
            if (useWeaponModifiersToScaleParticleEffect)
                impactFX.transform.localScale *= particleEffectSize;
            impactFX.Play();
            yield return new WaitUntil(() => !impactFX.isPlaying);
            if (useWeaponModifiersToScaleParticleEffect)
                impactFX.transform.localScale = Vector3.one;
            impactFX.gameObject.SetActive(false);
        }
    }
}