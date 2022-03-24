using Newtonsoft.Json;

using RPG.Units;
using RPG.Units.Items;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [Serializable]
    public class EquipmentSet
    {
        public EquipmentItem Item;
        public EquipmentComponent Instance;
        public Transform ParentBone;

		public override string ToString()
		{
            return string.Concat(Item.ToString(), " : ", Instance.name);
		}
	}

    [Serializable]
    public abstract class BaseItem
    {
        /// <summary>
        /// Идентификатор для определения всех свойств предмета
        /// </summary>
        /// <remarks>
        /// Не использовать в идентификаторах символ '%'
        /// </remarks>
        public string ID;
        public string DescriptionID;
        public ItemType Type;
        public RarityItemType Rarity;
        public Sprite Icon;
        public bool Unstackable;
        public int MaxStack;
        public int Count;
        public List<Status> Statuses = new List<Status>();
        public string EffectID;

        public abstract BaseItem Clone();

        protected BaseItem InternalClone(BaseItem item)
        {
            var statuses = new List<Status>(Statuses.Count);
            foreach (var element in Statuses) statuses.Add(element.Clone());
            item.ID = ID;
            item.DescriptionID = DescriptionID;
            item.Type = Type;
            item.Rarity = Rarity;
            item.Icon = Icon;
            item.Unstackable = Unstackable;
            item.MaxStack = MaxStack;
            item.Statuses = statuses;
            item.EffectID = EffectID.Clone() as string;

            return item;
        }

		public static bool operator==(BaseItem a, BaseItem b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.ID == b.ID;
		}
		public static bool operator !=(BaseItem a, BaseItem b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;
            return a.ID != b.ID;
		}

        public override bool Equals(object obj)
        {
            if(obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (GetType() != obj.GetType()) return false;
            return ID == ((BaseItem)obj).ID;
        }
        //ReferenceEquals(this, obj);
        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
		{
            return string.Concat(ID, " : ", Count);
		}
	}

    [Serializable]
    public class EquipmentItem : BaseItem
    {
        public EquipmentType Equipment;
        public EquipmentComponent Prefab;

		public override BaseItem Clone()
		{
            var clone = new EquipmentItem 
            {
                Equipment = Equipment,
                Prefab = Prefab
            };

            return InternalClone(clone);
		}
	}

	//todo ЗАГЛУШКА ДЛЯ ПРЕДМЕТОВ, ЛОГИКА ТИПОВ КОТОРЫХ НЕ РЕАЛИЗОВАНА
	public class EmptyTempItem : BaseItem
	{
		public override BaseItem Clone()
		{
            return InternalClone(new EmptyTempItem());
		}
	}

    [Serializable]
    public class SkillData
    {
        public string ID;
        [JsonIgnore]
        public string DescriptionID = "Сила физических атак увеличивается на";//todo
        [JsonIgnore]
        public Sprite Icon;
        [JsonIgnore]
        public int MaxLevel;
        [HideInInspector]
        public int CurrentLevel;
        [JsonIgnore]
        public bool PassiveSkill;
        [JsonIgnore]
        public List<Status> Statuses = new List<Status>();
        [JsonIgnore]
        public string EffectID;
        [JsonIgnore]
        public Vector2Int Position;
        [JsonIgnore]
        public float Delay;

		public override string ToString()
		{
            return $"ID: {ID}-Level: {CurrentLevel}";
		}
	}

    #region Conversations    

    public abstract class ConversationBaseContext : BaseContext
    {
        public DialogueContext Dialogue;
        public PhraseType Type;
	}

    [Serializable]
    public class DialogueContext : ConversationBaseContext
    {
        public List<AnswerContext> Answers = new List<AnswerContext>();

		public override string ToString()
		{
            return string.Concat(Type, " : ", ContextID, " | Answers: ", Answers != null ? Answers.Count.ToString() : "null");
		}
	}

    [Serializable]
    public class AnswerContext : ConversationBaseContext
    {
        //Картинка для особого действия
        public Sprite Sprite;
    }

	#endregion

	#region Quests

    [Serializable]
    public class QuestBaseContext : BaseContext
    {
        [JsonIgnore]
        public string DescriptionID;
        public List<ParagraphQuest> ParagraphIDs = new List<ParagraphQuest>();
        [JsonIgnore]
        public List<string> RewardIDs = new List<string>();

        public IdentificatorParagraphData CreateIdentificator(ParagraphQuest paragraph) => new IdentificatorParagraphData(DescriptionID, paragraph.ParagraphID);

        public ParagraphQuest GetParagraph(string id)
        {
            foreach (var paragraph in ParagraphIDs)
            {
                if (paragraph.ParagraphID == id) return paragraph;
            }
            return null;
        }

        public ParagraphQuest GetParagraph(IdentificatorParagraphData data) => GetParagraph(data.ParagraphID);

        public QuestBaseContext Clone()
        {
            var paragraphs = new List<ParagraphQuest>(ParagraphIDs.Count);
            foreach (var element in ParagraphIDs) paragraphs.Add(element.Clone());
            var rewards = new List<string>(RewardIDs.Count);
            foreach (var element in RewardIDs) rewards.Add(element.Clone() as string);

            return new QuestBaseContext
            {
                DescriptionID = DescriptionID.Clone() as string,
                ParagraphIDs = paragraphs,
                RewardIDs = rewards,
                ContextID = ContextID.Clone() as string
            };
		}

		public override string ToString()
		{
            var count = 0;
            foreach (var par in ParagraphIDs) if (par.Type != ParagraphStateType.Completed) count++;

            return string.Concat(ContextID, " : ", count);
		}
	}

    [Serializable]
    public class ParagraphQuest
    {
        public ParagraphStateType Type;
        public int Condition;
        [JsonIgnore]
        public string ObjectCondition;
        public string ParagraphID;

        public ParagraphQuest Clone()
        {
            return new ParagraphQuest
            {
                Type = Type,
                Condition = Condition,
                ObjectCondition = ObjectCondition.Clone() as string,
                ParagraphID = ParagraphID.Clone() as string
            };
		}

		public override string ToString()
		{
            return string.Concat(ParagraphID, " : ", Type, " : ", Condition);
		}
	}

    [Serializable]
    public struct IdentificatorParagraphData
    {
        public string QuestID;
        public string ParagraphID;

        public IdentificatorParagraphData(string questID, string paragraphID)
        {
            QuestID = questID;
            ParagraphID = paragraphID;
		}

		public override string ToString()
		{
            return string.Concat(QuestID, ": ", ParagraphID);
		}

		public override bool Equals(object obj) => obj is IdentificatorParagraphData data ? data.QuestID == QuestID && data.ParagraphID == ParagraphID : false;
        public override int GetHashCode() => base.GetHashCode();
    }

    #endregion

    public abstract class BaseContext
    {
        public string ContextID;
    }

    public class ResultHandler
    {
        public bool IsCompleted { get; private set; }
        public bool Result { get; private set; }

        private ResultHandler() { }

        public static ResultHandler GetHandler(out Action<bool> action)
        {
            var handler = new ResultHandler();

            action = new Action<bool>(t =>
            {
                handler.IsCompleted = true;
                handler.Result = t;
            });

            return handler;
		}
	}
}
