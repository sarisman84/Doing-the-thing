using System.Collections.Generic;
using UnityEngine;

namespace Player.Quests
{
    [CreateAssetMenu(fileName = "New QuestLibrary", menuName = "QuestAsset/Library", order = 0)]
    public class QuestLibrary : ScriptableObject
    {
        public List<QuestAsset> questList;
    }
}