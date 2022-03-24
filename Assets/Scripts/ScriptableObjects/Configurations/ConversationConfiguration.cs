using RPG;

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Configurations
{
    [CreateAssetMenu(fileName = "NewConversationConfiguration", menuName = "Scriptable Objects/Common/Conversation Configuration", order = 2)]
    public class ConversationConfiguration : GeneralConfiguration
    {
        [SerializeField]
        private ContextQueueData[] _datas;

        public Queue<string> GetQueueByID(string unitID)
        {
            var array = _datas.FirstOrDefault(t => t.UnitID == unitID).ContextID;

            return array != null ? new Queue<string>(array) : null;
        }

        [System.Serializable]
        private struct ContextQueueData
        {
            public string UnitID;
            public string[] ContextID;
		}
    }
}