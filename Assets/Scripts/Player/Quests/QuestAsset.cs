using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


namespace Player.Quests
{
    [CreateAssetMenu(fileName = "New Quest Asset", menuName = "Quest/Quest", order = 0)]
    public class QuestAsset : ScriptableObject
    {
        public string title;
        public List<QuestCondition> questConditions;
        public Guid ID { get; set; }

        private void OnEnable()
        {
            ID = ID == default ? new Guid() : ID;
        }
    }


    [Serializable]
    public class QuestData
    {
        public int CountProgress { get; set; }
        public Vector3 CurrentLocation { get; set; }
        public string CurrentTargetEntity { get; set; }
        public Guid ID { get; set; }
        public bool IsQuestCompleted { get; set; }

        public QuestAsset relatedQuest;



        public QuestData(QuestAsset asset)
        {
            ID = asset.ID;
            relatedQuest = asset;
        }

      
    }
}