using RPG.Commands;
using RPG.Units;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.Contexts
{
    [CreateAssetMenu(fileName = "NewSkillTreeContext", menuName = "Scriptable Objects/Skill Tree Context", order = 3)]
    public class SkillTreeContext : GeneralConfiguration
    {
        [SerializeField, Tooltip("Классовая категория, к которой относится дерево прокачки")]
        private SkillTreeType _treeType;
        [SerializeField, Tooltip("Идентификатор названия")]
        private string _descriptionID;
        [SerializeField]
        private List<Skill> _skills = new List<Skill>();
        [SerializeField]
        private Vector2Int _size;

        public Vector2Int Size { get => _size; set => _size = value; }

        public SkillTreeType Type => _treeType;
        public string Description
        {
            get => _descriptionID;
#if UNITY_EDITOR
            set => _descriptionID = value;
#endif
        }

        public List<Skill> Skills => _skills;
        /*{
            get
            {
                var list = new List<Skill>(_skills.Count);

                foreach (var element in _skills)
                {
                    list.Add(element.Clone());
                }

                return list;
            }
        }*/
    }
}
