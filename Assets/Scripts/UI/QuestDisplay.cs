using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Player.Quests;
using UnityEngine;
using Utility;

namespace UI
{
    public class QuestDisplay : MonoBehaviour
    {
        public Canvas questDisplay;
        public List<QuestUIElement> questUIElements;

        private void Awake()
        {
            questUIElements.ApplyAction(e => e.ResetText());
            EventManager.AddListener<Action<QuestData>>("UI/Quest/UpdateQuestUI", DisplayQuest);
        }

        public static void UpdateQuestUI(QuestData progressData)
        {
            EventManager.TriggerEvent("UI/Quest/UpdateQuestUI", progressData);
        }

        private void DisplayQuest(QuestData data)
        {
            QuestUIElement element = questUIElements.Find(e => e.QuestID == data.ID);
            bool isQuestComplete = data.IsQuestCompleted;
            if (element)
            {
                if (!isQuestComplete)
                {
                   
                    UpdateQuestElement(element, data);
                }

                if (isQuestComplete)
                {
                    element.ResetText();
                    element.QuestID = default;
                }
            }
            else if (!isQuestComplete)
            {
                element = questUIElements.First(e => e.QuestID == default);
                element.QuestID = data.ID;
                UpdateQuestElement(element, data);
            }
        }

        private void UpdateQuestElement(QuestUIElement element, QuestData data)
        {
            element.image.enabled = true;
            element.title.text = data.relatedQuest.title;
            element.description.text = string.Empty;
            foreach (QuestCondition condition in data.relatedQuest.questConditions)
            {
                string intro = String.Empty;
                string body = String.Empty;
                switch (condition.conditionType)
                {
                    case QuestCondition.Type.Kill:
                    case QuestCondition.Type.Gather:
                    case QuestCondition.Type.Defend:
                        switch (condition.conditionType)
                        {
                            case QuestCondition.Type.Kill:
                                intro = $"Eliminate {condition.TargetEntity.name}s: ";
                                break;
                            case QuestCondition.Type.Gather:
                                intro = $"Gather {condition.TargetEntity.name}s: ";
                                break;
                            case QuestCondition.Type.Defend:
                                intro = $"Defend {condition.TargetEntity.name}. Current Health: ";
                                break;
                        }


                        body = $"{condition.CoundCondition}/{data.CountProgress}";
                        break;
                    case QuestCondition.Type.Escort:
                        intro = $"Escort {condition.TargetEntity.name} to ";
                        body = $"{condition.TargetLocation.name}";
                        break;

                    case QuestCondition.Type.Goto:
                        intro = "Go to ";
                        body = $"{condition.TargetLocation.name}";
                        break;
                }

                element.description.text += $"\n*{intro}{body}";
            }
        }
    }
}