using RPG.Units.NPCs;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Assistants
{
    public class BotAssistant
    {
        private LinkedList<NPCUnit> _bots;

        public void OnUpdate(float delta)
        {
            foreach(var bot in _bots)
            {
                if (!bot.gameObject.activeInHierarchy || !bot.gameObject.activeSelf) continue;
                (bot.Input as NPCInputComponent).OnUpdate(delta); //todo подумать над упролщением, если будут проблемы с производительностью
			}
		}

        public BotAssistant(LinkedList<NPCUnit> bots) => _bots = bots;
    }
}
