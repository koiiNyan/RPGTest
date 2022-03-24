using RPG.Commands;
using RPG.Units;
using RPG.Units.NPCs;
using RPG.Units.Player;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Assistants
{
    public class DataStateAssistant
    {
        private LinkedList<NPCUnit> _units;
		private Unit[] _player;

		private float _delta;

        public void OnUpdate(float delta)
        {
			_delta = delta;

			UpdateConditions(_player);
			UpdateConditions(_units);

			UpdateEffects(_player);
			UpdateEffects(_units);
		}

		private void UpdateConditions(IEnumerable<Unit> units)
		{
			UnitStateComponent state;
			foreach (var unit in units)
			{
				state = unit.State;

				state.Parameters.Health = Mathf.Min(state.Parameters.Health + state.Parameters[StatsType.HPRegInSec] * _delta, state.Parameters[StatsType.Health]);
				state.Parameters.Mana = Mathf.Min(state.Parameters.Mana + state.Parameters[StatsType.MPRegInSec] * _delta, state.Parameters[StatsType.Mana]);
				state.Parameters.Stamina = Mathf.Min(state.Parameters.Stamina + state.Parameters[StatsType.SPRegInSec] * _delta, state.Parameters[StatsType.Stamina]);
			}
		}

		private void UpdateEffects(IEnumerable<Unit> units)
		{
			foreach(var unit in units)
			{
				var removeEffects = new LinkedList<BaseCommandEffect>();
				foreach(var effect in unit.State.Effects.GetAllEffects)
				{
					effect.CurrentDuration -= _delta;

					if(effect.CurrentDuration <= 0f)
					{
						effect.OnEnd();
						removeEffects.AddLast(effect);
					}
					else
                    {
						effect.OnUpdate(_delta);
                    }
				}

				if(removeEffects.Count > 0)	unit.State.Effects.RemoveEffects(removeEffects);
			}
		}

		public DataStateAssistant(LinkedList<NPCUnit> units, PlayerUnit player)
		{
			_units = units; _player = new Unit[] { player };
		}
    }
}
