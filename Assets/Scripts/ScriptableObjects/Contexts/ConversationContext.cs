using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Contexts
{ 
    [CreateAssetMenu(fileName = "NewConversationContext", menuName = "Scriptable Objects/Conversation Context", order = 2)]
    public class ConversationContext : ScriptableObjectConfiguration
    {
        [SerializeField]
        private string _conversationID;
        [SerializeField, Tooltip("Остается-ли беседа после первого воспроизведения")]
        private bool _isLoop;
        [SerializeField]
        private string _description;
        [SerializeField]
        private DialogueContext _context = new DialogueContext();


        public string ID => _conversationID;
        public bool IsLoop => _isLoop;
        public string Description => _description;

        public DialogueContext Context => _context;

        public (LinkedList<AnswerContext>, LinkedList<DialogueContext>) GetAllAnswerContexts()
        {
            var answers = new LinkedList<AnswerContext>();
            var dialogues = new LinkedList<DialogueContext>();

            if (_context != null)
            {
                dialogues.AddLast(_context);
                Calc(_context, answers, dialogues);
            }

            return (answers, dialogues);
		}

        private void Calc(ConversationBaseContext context, LinkedList<AnswerContext> answers, LinkedList<DialogueContext> dialogues)
        {
            var dialogue = context as DialogueContext;
            var answer = context as AnswerContext;

            if (dialogue != null)
            {
                if (dialogue.Dialogue != null)
                {
                    dialogues.AddLast(dialogue.Dialogue);
                    Calc(dialogue.Dialogue, answers, dialogues);
                }
                if (dialogue.Answers != null && dialogue.Answers.Count != 0)
                {
                    foreach (var a in dialogue.Answers) answers.AddLast(a);
                    foreach (var an in dialogue.Answers) Calc(an, answers, dialogues);
                }
            }
            if (answer != null && answer.Dialogue != null)
            {
                dialogues.AddLast(answer.Dialogue);
                Calc(answer.Dialogue, answers, dialogues);
            }
        }
    }
}
