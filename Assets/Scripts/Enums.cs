using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public enum EquipmentType : byte
    {
        Helm,
        Armor,
        Scapular,
        MainHand,
        AdditionalHand,
        Pants,
        Foots,
	}
    public enum ItemType : byte
    {
        Other,
        Ingredient,
        Material,
        Ammunition,
        Equipment
    }

    public enum RarityItemType : byte
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
	}

    public enum SkillTreeType : byte
    {
        Strength,
        Agility,
        Magic,
        Condition,
        General
    }
    public enum SideType : byte
	{
		None = 0,
		Friendly = 1,
		Enemy = 2,
	}

	public enum WeaponStyleType : byte
	{
		None = 0,
		TwoHandedSwords = 1,
		ShieldAndSword = 2,
		Bow = 3,
		Mage = 4,
	}

    public enum PhraseType : byte
    {
        /// <summary>
        /// После фразы диалог прерывается
        /// </summary>
        None,
        /// <summary>
        /// После фразы вызывается следующая фраза
        /// </summary>
        Phrase,//
        /// <summary>
        /// После фразы ожидается ответ
        /// </summary>
        Answers,//
        /// <summary>
        /// После фразы вызывается магазин
        /// </summary>
        Trade,
        /// <summary>
        /// После фразы выдается/сдается квест
        /// </summary>
        Quest
    }

    public enum ParagraphStateType
    {
        Inactive,
        Active,
        Completed,
        Failed,
    }

    [Flags]
    public enum ActionMapType : byte
    {
        None = 0,
        Unit = 1,
        Camera = 2,
        UI = 4
    }

    public enum AIStateType : byte
	{
		Idle,
		Patrolling,
		Pursuit,
	}

	public enum CameraPositionType : byte
	{
		Default,
		Dialogue,
	}

    public enum LocalizationType : byte
    {
        Rus,
        Eng
	}

    public enum InputSetType : byte
    {
        None = 0,
        InputE = 1,
	}

    public enum WidgetType : byte
    {
        Battle,
        Dialogues,
        Quests,
        Skills,
        Inventories,
        Menu,
	}

    public enum OptionType : byte
    {
        Ok,
        Confirm,
        Cancel,
        Back
	}

    public enum ComplicationMode : byte
    {
        Easy,
        Medium,
        Hard,
        VeryHard
	}

    public enum AudioSourceType : byte
    {
        General,
        Soundtrack,
        Effect,
        Dialogue,
	}

    public enum BotPrefabType : byte
    {
        Melee,
        Range,
        Mage,
        Dealer,
        Farmer
	}
}
