using Newtonsoft.Json.Linq;

using RPG.Assistants;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.Player
{
    public class PlayerStateComponent : UnitStateComponent, ILevelProgress
    {
        [SerializeField]
        private int _levelExpStep = 10;
        [SerializeField]
        private int _pointsInLevel = 4;

        public byte Level { get; private set; } = 1;
        public uint Experience { get; private set; }
        public ushort FreePoints { get; set; } = 4;

        public void AddExp(int count)
        {
#if UNITY_EDITOR
            if (count < 0) Editor.EditorExtensions.ConsoleLog($"A negative amount of experience has been transferred to method <b>{nameof(AddExp)}</b>", Editor.PriorityMessageType.Critical);
#endif
            Experience += (uint)count;

            if (Experience < Level * _levelExpStep) return;

            Experience -= Level * (uint)_levelExpStep;
            Level++;

            Debug.Log("<b>LEVEL UP!</b> current level: " + Level);//todo
		}

		public override JObject GetSaveData()
		{
			var obj = base.GetSaveData();

            obj.Add("level", Level);
            obj.Add("experience", Experience);
            obj.Add("points", FreePoints);

            return obj;
		}

		public override void SetSaveData(JToken token)
		{
			base.SetSaveData(token);

            Level = token["level"].Value<byte>();
            Experience = token["experience"].Value<ushort>();
            FreePoints = token["points"].Value<ushort>();
        }
	}

    public interface ILevelProgress
    {
        byte Level { get; }
        uint Experience { get; }
        ushort FreePoints { get; set; }
    }
}