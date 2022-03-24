using RPG;

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Configurations
{
    [CreateAssetMenu(fileName = "NewQuestConfiguration", menuName = "Scriptable Objects/Common/Quest Configuration", order = 2)]
    public class QuestConfiguration : GeneralConfiguration
    {
        [SerializeField]
        private QuestQueueData[] _datas;

        public Queue<string> GetQuestByID(string unitID)
        {
            var data = _datas.FirstOrDefault(t => t.UnitID == unitID);

            if (data.Data == null) return null;
            var array = data.Data.Select(t=> t.QuestID);

            return new Queue<string>(array);
        }

        public LinkedList<IdentificatorParagraphData> GetParagraphByID(string unitID)
        {
            var array = _datas.FirstOrDefault(t => t.UnitID == unitID).Data;

            return array != null ? new LinkedList<IdentificatorParagraphData>(array) : null;
        }

        [System.Serializable]
        private struct QuestQueueData
        {
            public string UnitID;
            public IdentificatorParagraphData[] Data;
        }
    }
}
