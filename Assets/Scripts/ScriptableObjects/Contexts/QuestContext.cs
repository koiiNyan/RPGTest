using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Contexts
{
    [CreateAssetMenu(fileName = "NewQuestContext", menuName = "Scriptable Objects/Quest Context", order = 2)]
    public class QuestContext : ScriptableObjectConfiguration
    {
#pragma warning disable IDE0090

        [SerializeField]
		private QuestBaseContext _context = new QuestBaseContext();

#pragma warning restore IDE0090

		public string QuestID => _context.ContextID;

#if UNITY_EDITOR
        public QuestBaseContext GetContext_Editor => _context;
#endif

        public QuestBaseContext Context => _context.Clone();
    }
}
