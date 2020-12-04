using System;
using System.Collections.Generic;
using Extensions;
using Interactivity;
using Interactivity.Pickup;
using UI;
using UnityEngine;
using Utility;

namespace Player.Quests
{
    [RequireComponent(typeof(InteractionController))]
    public class QuestHandler : MonoBehaviour
    {
        public List<QuestAsset> assignedQuests = new List<QuestAsset>();
        List<QuestData> currentQuests = new List<QuestData>();
        private InteractionController _interactionController;

        private void Awake()
        {
            _interactionController = GetComponent<InteractionController>();
            assignedQuests.ApplyAction((qA, i) => AddQuest(qA));
            EventManager.AddListener<Action<QuestAsset>>("Quest/Handler/AddQuest", AddQuest);
        }

        private void OnEnable()
        {
            _interactionController.ONPickupCallback += OnPickup;
            _interactionController.ONKillCallback += OnKill;
        }

        private void OnKill(IDamageable obj)
        {
            currentQuests.ApplyAction(a => { a.CheckProgress(1, obj.gameObject.name); });
        }

        private void OnDisable()
        {
            _interactionController.ONPickupCallback -= OnPickup;
            _interactionController.ONKillCallback -= OnKill;
        }

        private void OnPickup(BasePickup obj)
        {
            currentQuests.ApplyAction(a => { a.IsQuestCompleted = a.CheckProgress(1, obj.gameObject.name); });
        }

        public void AddQuest(QuestAsset quest)
        {
            currentQuests.Add(new QuestData(quest));
        }

        public void DisplayQuest(QuestData progressData)
        {
            QuestDisplay.UpdateQuestUI(progressData);
        }

        private void Update()
        {
            currentQuests.ApplyAction(d =>
            {
                DisplayQuest(d);
                if (d.IsQuestCompleted)
                {
                    Debug.Log($"{d.relatedQuest.title}: Completed");
                    currentQuests.Remove(d);
                }
            });
        }
    }
}