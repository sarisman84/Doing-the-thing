using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Player.Quests
{
    [CreateAssetMenu(fileName = "New Condition", menuName = "QuestAsset/Condition", order = 0)]
    public class QuestCondition : ScriptableObject
    {
        public enum Type
        {
            Kill,
            Gather,
            Escort,
            Defend,
            Goto
        }

        public Type conditionType;
        public int CoundCondition => countCondition;
        [SerializeField] private int countCondition;
        public GameObject TargetEntity => targetEntity;
        [SerializeField] private GameObject targetEntity;
        public Location TargetLocation => targetLocation;
        [SerializeField] private Location targetLocation;
     
    }

    [Serializable]
    public struct Location
    {
        public string name;
        public Vector3 position;
    }


    public static class QuestConditionExtension
    {
        public static bool AreConditionsMet(this IEnumerable<QuestCondition> conditions, object value)
        {
            foreach (QuestCondition condition in conditions)
            {
                switch (condition.conditionType)
                {
                    case QuestCondition.Type.Kill:
                    case QuestCondition.Type.Gather:
                    case QuestCondition.Type.Defend:
                        if (value is int || value is float)
                        {
                            int countResult = (int)Convert.ChangeType(value, typeof(int));
                            return condition.CoundCondition <= countResult;
                        }
                        continue;


                    case QuestCondition.Type.Escort:
                    case QuestCondition.Type.Goto:
                        if (value is Vector3)
                        {
                            Vector3 currentLocation = (Vector3) Convert.ChangeType(value, typeof(Vector3));
                            return condition.TargetLocation.position.IsInTheVicinityOf(currentLocation);
                        }
                        continue;

                }
            }

            return true;
        }
    }
}