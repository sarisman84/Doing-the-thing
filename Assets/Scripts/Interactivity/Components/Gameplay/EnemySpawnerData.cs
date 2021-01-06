using System.Collections;
using Extensions;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Interactivity.Components.Gameplay
{
    [CreateAssetMenu(fileName = "New Spawn Data", menuName = "Data/Enemy Data", order = 0)]
    public class EnemySpawnerData : ScriptableObject
    {
        public BaseEnemy enemy;
        public int amount;
        public float individualSpawnDelay = 0.25f;


       

        public IEnumerator SpawnEntities(Transform spawnPos, int spawnRadius)
        {
            int enemyAmount = 0;
            while (!enemyAmount.Equals(amount))
            {
                GameObject obj = ObjectManager.DynamicInstantiate(enemy.gameObject, true, amount);
                obj.SetActive(true);
                obj.transform.position = spawnPos.position.GetRandomPositionInRange(spawnRadius);
                yield return new WaitForSeconds(individualSpawnDelay);
                enemyAmount++;
            }

            yield return null;
        }

        public void SpawnEntities(MonoBehaviour monoBehaviour,Transform spawnPos, int spawnRadius)
        {
            monoBehaviour.StartCoroutine(SpawnEntities(spawnPos, spawnRadius));
        }
    }
}