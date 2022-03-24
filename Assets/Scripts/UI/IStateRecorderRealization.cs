using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace RPG.UI
{
    public partial class WidgetQuests
    {
        [Inject]
        private SignalBus _loader;



        public JObject GetSaveData()
        {
            var obj = new JObject();

            if(_selectQuest != null) obj.Add("select", _selectQuest.ContextID);
            obj.Add("active", JToken.FromObject(_activeQuests));
            obj.Add("completed", JToken.FromObject(_completedQuests));

			return obj;
        }

        public void SetSaveData(JToken token)
        {
            _completedQuests = token["completed"].ToObject<LinkedList<QuestBaseContext>>();
            var activeList = token["active"].ToObject<LinkedList<QuestBaseContext>>();
            
            _activeQuests.Clear();
            //Добавление квестов
            foreach (var q in activeList)
            {
                AddQuest(q.ContextID);
                
            }
            //Заполнение состояний
            foreach(var q in _activeQuests)
            {
                var quest = activeList.First(t => t.ContextID == q.ContextID);

                for(int i = 0; i < q.ParagraphIDs.Count; i++)
                {
                    q.ParagraphIDs[i].Type = quest.ParagraphIDs[i].Type;
                    q.ParagraphIDs[i].Condition = quest.ParagraphIDs[i].Condition;

                    _loader.FireId<(IdentificatorParagraphData, ParagraphStateType)>(Constants.LoadIdentifier, (new IdentificatorParagraphData(q.ContextID, q.ParagraphIDs[i].ParagraphID), quest.ParagraphIDs[i].Type));
				}
			}

            _selectQuest = _activeQuests.FirstOrDefault(t=> t.ContextID == token["select"].Value<string>());
            UpdateVisualQuest();
        }
    }
}

namespace RPG.UI.Blocks
{
	public partial class SkillTreeBlock
	{
        public JObject GetSaveData()
        {
            var array = new JArray();
            foreach (var list in _skillLists)
            {
                foreach (var skill in list)
                {
                    if (skill.CurrentLevel == 0) continue;
                    array.Add(JToken.FromObject(skill));
                }
            }

            var obj = new JObject
            {
                { "values", array }
            };
            return obj;
        }

        public void SetSaveData(JToken token)
        {
            IEnumerable<SkillData> list = new LinkedList<SkillData>();
            foreach (var l in _skillLists) list = list.Union(l);

            foreach (var l in list)
            {
                while (l.CurrentLevel != 0)//todo костыль, чтобы удалить все статусы от нескольких уровней таланта
                {
                    l.CurrentLevel--;
                    OnSkillDownEventHandler?.Invoke(l);
                }
            }

            foreach (var el in token["values"] as JArray)
            {
                var key = el[nameof(SkillData.ID)].Value<string>();
                var skill = list.First(t => t.ID == key);
                skill.CurrentLevel = el[nameof(SkillData.CurrentLevel)].Value<int>();

                OnSkillUpEventHandler?.Invoke(InputSetType.None, skill);
            }
        }
    }
}
