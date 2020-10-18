using System;
using System.Collections;
using Extensions;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Interactivity
{
    public class EnemySpawner : MonoBehaviour
    {
        public bool repeatSpawn;
        public float spawnDelay = 0.25f;
        public bool displaySpawnRadius = true;
        public int spawnRadius = 1;
        public EnemySpawnInformation[] enemySpawnArray;

        private void Awake()
        {
            for (int i = 0; i < enemySpawnArray.Length; i++)
            {
                enemySpawnArray[i].CreateEntities();
            }

            StartCoroutine(SpawnLoop());
        }
        

        IEnumerator SpawnLoop()
        {
            int index = 0;
            while (!index.Equals(enemySpawnArray.Length))
            {
                yield return enemySpawnArray[index]
                    .UseEntities(transform, spawnRadius);
                index++;
            }

            yield return null;
        }


        private void OnDrawGizmos()
        {
            Color displayColor = Color.red;
            displayColor.a = 0.5f;

            Gizmos.color = displayColor;
            
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
    }


    [System.Serializable]
    public class EnemySpawnInformation
    {
        public BaseEnemy enemy;
        public int amount;
        public float individualSpawnDelay = 0.25f;


        public void CreateEntities()
        {
            ObjectManager.PoolGameObject(enemy.gameObject, amount);
        }

        public IEnumerator UseEntities(Transform spawnPos, int spawnRadius)
        {
            int enemyAmount = 0;
            while (!enemyAmount.Equals(amount))
            {
                GameObject obj = ObjectManager.DynamicInstantiate(enemy.gameObject);
                obj.SetActive(true);
                obj.transform.position = spawnPos.position.GetRandomPositionInRange(spawnRadius);
                yield return new WaitForSeconds(individualSpawnDelay);
                enemyAmount++;
            }

            yield return null;
        }
    }
}