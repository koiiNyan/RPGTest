using RPG.Managers;
using RPG.Units.NPCs;
using RPG.Units.Player;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace RPG.Assistants
{
    public class ActivityAssistant
    {
		private int _triggeredEnemies;
        private readonly LinkedList<NPCInputComponent> _bots;
		/// <summary>
		/// Кстати, если пометить поле как readonly, мы получаем более безопасный код, но важно знать маленький нюанс:
		/// при компиляции кода в IL, при запросе к полю, мы создаем временную копию, по которой уже ссылаемся к полям,
		/// иначе говоря, readonly поля структур создают лишнюю копию всей структуры при чтении переменной, поэтому
		/// лучше не помечать поля значений как readonly, хотя оптимизация настолько несущес твенна, что можно просто забыть
		/// https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/
		/// </summary>
		private readonly PlayerUnit _player;

		public bool InFight => _triggeredEnemies > 0;

        public void RegistrationUnit(NPCUnit unit, bool bind = true)
        {
            if(bind)
			{
				return;
			}

			_bots.Remove(unit.Input as NPCInputComponent);
		}

		public IEnumerator CheckActivity(float distanceActivation, float activityCheckDelay, float targetFocusDistance, float resetDistance)
		{
			distanceActivation = distanceActivation * distanceActivation;
			targetFocusDistance = targetFocusDistance * targetFocusDistance;
			while (true)
			{
				foreach (var bot in _bots)
				{
					var distance = Vector3.SqrMagnitude(bot.transform.position - _player.transform.position);
					bot.Activity = distance < distanceActivation;

					//Неактивный или дружественный бот todo боты не сражаются друг с дружкой
					if (!bot.Activity || bot.Owner.State.Side == _player.State.Side) continue;

					//Если : игрок близко - бот триггерится
					if (distance < targetFocusDistance && bot.Owner.Target == null)
					{
						//ИГрок - новая цель бота
						bot.SetTarget(_player);
						_triggeredEnemies++;
					}
					//Проверка, нужно-ли сбросить состояние бота
					else if (bot.CheckResetState(resetDistance)) 
						_triggeredEnemies--;
				}

				yield return new WaitForSeconds(activityCheckDelay);
			}
		}

		public ActivityAssistant(LinkedList<NPCUnit> bots, PlayerUnit player)
        {
            _player = player;
			_bots = new LinkedList<NPCInputComponent>(bots.Select(t => t.Input as NPCInputComponent));
        }
    }
}
