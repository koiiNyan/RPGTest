using RPG.Units;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPG.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Scriptable Objects/Skills/Skill", order = 1)]
    public class Skill : ScriptableObject
    {
        [SerializeField]
        private SkillData _data = new SkillData();

#if UNITY_EDITOR
        public SkillData GetSkillData() => _data;
#endif

        public SkillData CreateData()
        {
            var statuses = new List<Status>(_data.Statuses.Count);
            foreach (var status in _data.Statuses) statuses.Add(status.Clone());

            return new SkillData()
            {
                ID = _data.ID.Clone() as string,
                DescriptionID = _data.DescriptionID.Clone() as string,
                Icon = _data.Icon,
                MaxLevel = _data.MaxLevel,
                PassiveSkill = _data.PassiveSkill,
                Statuses = statuses,
                EffectID = _data.EffectID.Clone() as string,
                Position = _data.Position,
                Delay = _data.Delay
            };
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Skill))]
    public class SkillEditor : UnityEditor.Editor
    {
        private Skill _target;
        private void OnEnable()
        {
            _target = target as Skill;

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(20f);
            EditorGUILayout.LabelField("Preview");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = false;
            EditorGUILayout.ObjectField(_target.GetSkillData().Icon, typeof(Sprite), false, GUILayout.Width(256f), GUILayout.Height(256f));
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
//Depth - глубина

//breadth - ширина
