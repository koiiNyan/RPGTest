using RPG.ScriptableObjects;
using RPG.ScriptableObjects.Contexts;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Editor
{
    public class LoadItemWindow : LoadContextWindow<ItemWindow, BaseItemContext>
    {
        public static void ShowLoadItemWindow(ItemWindow parent)
        {
            var window = GetWindow<LoadItemWindow>(true, "Load Item Window", true);
            window._parent = parent;
        }
    }

    public class LoadQuestWindow : LoadContextWindow<QuestWindow, QuestContext> 
    {
        public static void ShowLoadQuestWindow(QuestWindow parent)
        {
            var window = GetWindow<LoadQuestWindow>(true, "Load Quest Window", true);
            window._parent = parent;
        }
    }
    public class LoadConversationWindow : LoadContextWindow<ConversationWindow, ConversationContext> 
    {
        public static void ShowLoadConversationWindow(ConversationWindow parent)
        {
            var window = GetWindow<LoadConversationWindow>(true, "Load Conversation Window", true);
            window._parent = parent;
        }
    }

    public class LoadSkillTreeWindow : LoadContextWindow<SkillTreeEditorWindow, SkillTreeContext>
    {
        public static void ShowLoadSkillTreeWindow(SkillTreeEditorWindow parent)
        {
            var window = GetWindow<LoadSkillTreeWindow>(true, "Load Skill Tree Window", true);
            window._parent = parent;
		}
	}

    public class LoadContextWindow<TParent, TFind> : EditorWindow where TParent : BaseCustomEditorWindow where TFind : ScriptableObjectConfiguration
    {
        protected TParent _parent;

        private ConversationContext _selectContext;
        private LinkedList<TFind> _contexts;

        private float _heightSingleLine = 31f;

        private void OnEnable()
        {
            _contexts = new LinkedList<TFind>(RPGExtensions.FindAllAssetsByType<TFind>());
            minSize = maxSize = new Vector2(800f, _heightSingleLine * _contexts.Count);
        }

        private void PrintButton(TFind context)
        {
            var rect = EditorGUILayout.BeginHorizontal("box");

            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Green];
            if(GUILayout.Button("Select", GUILayout.Width(50f)))
            {
                if (_parent is ConversationWindow conversation)
                {
                    conversation.SelectConversation = context as ConversationContext;
                }
                else if (_parent is QuestWindow quest)
                {
                    quest.SelectQuest = context as QuestContext;
                }
                else if(_parent is SkillTreeEditorWindow skill)
                {
                    skill.SelectSkillTree = context as SkillTreeContext;
				}
                else if(_parent is ItemWindow item)
                {
                    item.SelectItems = context as BaseItemContext;
				}
                else throw new ApplicationException($"The Type {_parent.GetType().Name} is not supported in {nameof(LoadContextWindow<TParent, TFind>)}");
                Close();
            }
            GUI.color = Color.white;

            EditorGUILayout.ObjectField(context, typeof(TFind), false, GUILayout.Width(200f));

            if (context is ConversationContext convers) EditorGUILayout.LabelField(convers.Description);
            else if (context is QuestContext quest) EditorGUILayout.LabelField(quest.GetContext_Editor.DescriptionID);
            else if (context is SkillTreeContext skill) EditorGUILayout.LabelField(skill.Description);
            else if (context is BaseItemContext item) EditorGUILayout.LabelField(string.Concat(item.Type, " : ", item.Rarity));
            else throw new ApplicationException($"The Type {_parent.GetType().Name} is not supported in {nameof(LoadContextWindow<TParent, TFind>)}");

            EditorGUILayout.EndHorizontal();
		}

        private void OnGUI()
        {
            foreach(var context in _contexts) PrintButton(context);
        }
    }
}
