using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Assistants
{
    public class DispatcherAssistant
    {
        private LinkedList<ActionDelay> _stack = new LinkedList<ActionDelay>();



        public void OnUpdate()
        {
            var action = _stack.First;

            while (action != null)
            {
                action.Value.Frames = action.Value.Frames - 1;
                if (action.Value.Frames < 0)
                {
                    action.Value.Action.Invoke();

                    var remove = action;
                    action = action.Next;
                    _stack.Remove(remove);
                    continue;
                }
                action = action.Next;
            }
		}

        public void AddAction(Action action, int frames = 1)
        {
            _stack.AddLast(new ActionDelay { Action = action, Frames = frames });
        }

        private class ActionDelay
        {
            public int Frames;
            public Action Action;
		}
    }
}