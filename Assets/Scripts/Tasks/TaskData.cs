using System.Collections.Generic;
using Blindsided.Utilities;
using TimelessEchoes.Skills;
using TimelessEchoes.Upgrades;
using UnityEngine;

namespace TimelessEchoes.Tasks
{
    [ManageableData]
    [CreateAssetMenu(fileName = "TaskData", menuName = "SO/Task Data")]
    public class TaskData : ScriptableObject
    {
        public string taskName;
        public string taskID;
        public Sprite taskIcon;
        public Skill associatedSkill;
        public float xpForCompletion;
        public float taskDuration;
        public List<ResourceDrop> resourceDrops = new();

        [System.Serializable]
        public class Persistent
        {
            public int totalTimesCompleted;
            public float timeSpent;
            public float experienceGained;
        }

        [HideInInspector]
        public Persistent persistent = new();
    }
}
