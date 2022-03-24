using RPG.ScriptableObjects;
using RPG.ScriptableObjects.Contexts;
using RPG.Units;

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace RPG.Editor
{
    public class SkillTreeEditorWindow : BaseCustomEditorWindow
    {
        private SkillTreeContext _selectSkillTree;
        private List<Skill> _skillDataList;

        private Dictionary<SkillData, bool?> _dicFoldoutStatuses;

        #region Settings

        private Vector2 _mainDataSize = new Vector2(200f, 300f);
        private Vector2 _scroll;

        #endregion

        public SkillTreeContext SelectSkillTree
        {
            get => _selectSkillTree;
            set
            {
                _selectSkillTree = value;
                _skillDataList = null;
                Init();
			}
        }
        
        [MenuItem("Extensions/Windows/Skill Tree Editor Window #n", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<SkillTreeEditorWindow>(false, "Skill Tree Editor Window", true);
            window.minSize = window.maxSize = new Vector2(300f, 450f);
        }

        public override void OnEnable()
        {
            if (EditorPrefs.HasKey("SkillTree:select"))
                SelectSkillTree = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString("SkillTree:select"), typeof(SkillTreeContext)) as SkillTreeContext;
        }

        protected override void OnDisable()
        {
            if (SelectSkillTree != null)
                EditorPrefs.SetString("SkillTree:select", AssetDatabase.GetAssetPath(SelectSkillTree));
        }

        private bool PrintHeader()
        {
            //Строка с полем для анимации
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(GetDefaultSpace);
            EditorGUILayout.LabelField("Skill tree:", GUILayout.MaxWidth(90f));
            GUILayout.Space(GetDefaultSpace / 2f);

            //Выбранный диалог
            var context = EditorGUILayout.ObjectField(SelectSkillTree, typeof(SkillTreeContext), false, GUILayout.Width(150f)) as SkillTreeContext;
            if (context != SelectSkillTree) SelectSkillTree = context;

            GUILayout.Space(GetDefaultSpace);
            GUI.color = new Color(0.5f, 1f, 0f, 1f);
            if (GUILayout.Button("Load", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                LoadSkillTreeWindow.ShowLoadSkillTreeWindow(this);
                return false;
            }

            GUI.color = new Color(0.5f, 1f, 0f, 1f);
            if (GUILayout.Button("Compile", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;

                for(int i = 0; i < _selectSkillTree.Size.x; i++)
                {
                    for (int j = 0; j < _selectSkillTree.Size.y; j++)
                    {
                        if (_skillDataList[i * _selectSkillTree.Size.y + j] == null) continue;
                        _skillDataList[i * _selectSkillTree.Size.y + j].GetSkillData().Position = new Vector2Int(i, j);
					}
                }

                typeof(SkillTreeContext).GetField("_skills", flags).SetValue(_selectSkillTree, _skillDataList);

                foreach (var skill in _skillDataList)
                {
                    if (skill == null) continue;
                    EditorUtility.SetDirty(skill);
                }
                EditorUtility.SetDirty(SelectSkillTree);
                //Сохраняем файл
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return false;
            }
            GUI.color = Color.white;

            EditorGUILayout.Space(25f);
            
            EditorGUILayout.LabelField("Size:", GUILayout.Width(40f));

            var sizeValue = EditorGUILayout.Vector2IntField("", _selectSkillTree.Size);
            if(sizeValue != _selectSkillTree.Size)
            {
                _selectSkillTree.Size = sizeValue;
                return false;
            }

            EditorGUILayout.LabelField("Description", GUILayout.Width(70f));
            _selectSkillTree.Description = EditorGUILayout.TextField(_selectSkillTree.Description);

            GUILayout.FlexibleSpace();
            //Конец лейбла и поля под анимацию
            EditorGUILayout.EndHorizontal();

            return true;
        }

        #region Table

        private bool PrintTreeTable()
        {
            DrawHeader("Tree");

            if (_skillDataList == null) Init();

            //Начало отрисовки скролла таблицы
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _selectSkillTree.Size.x; i++)
            {
                //Начало отрисовки одной строки
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < _selectSkillTree.Size.y; j++)
                {
                    //Начало отрисовки отдельного блока с данными скилла
                    EditorGUILayout.BeginHorizontal("box", GUILayout.Width(_mainDataSize.x), GUILayout.Height(_mainDataSize.y));
                    EditorGUILayout.BeginVertical();
                    PrintSkillData(i * _selectSkillTree.Size.y + j);
                    EditorGUILayout.EndVertical();
                    //Конец отрисовки отдельного блока с данными скилла
                    EditorGUILayout.EndHorizontal();
                }
                //Конец отрисовки одной строки
                EditorGUILayout.EndHorizontal();
            }

            //Конец отрисовки скролла таблицы
            EditorGUILayout.EndScrollView();
            return true;
        }

        private void PrintSkillData(int index)
        {
            _skillDataList[index] = EditorGUILayout.ObjectField(_skillDataList[index], typeof(Skill), false) as Skill;
            if (_skillDataList[index] == null) return;
            var skill = _skillDataList[index].GetSkillData();

            if (skill == null) return;

            if (!_dicFoldoutStatuses.ContainsKey(skill)) _dicFoldoutStatuses.Add(skill, false);

            skill.ID = PrintParam<string>(skill.ID, "ID");
            skill.MaxLevel = PrintParam<int>(skill.MaxLevel, "Level");
            skill.PassiveSkill = PrintParam<bool>(skill.PassiveSkill, "Passive");
            skill.EffectID = PrintParam<string>(skill.EffectID, "EffectID");
            if(!skill.PassiveSkill) skill.Delay = PrintParam<float>(skill.Delay, "Delay");

            #region
            /*
            //Начало отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", GUILayout.MaxWidth(50f));
            skill.ID = EditorGUILayout.TextField(skill.ID);
            //Конец отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.EndHorizontal();

            //Начало отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level", GUILayout.MaxWidth(50f));
            skill.MaxLevel = EditorGUILayout.IntField(skill.MaxLevel);
            //Конец отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.EndHorizontal();

            //Начало отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Passive", GUILayout.MaxWidth(50f));
            skill.PassiveSkill = EditorGUILayout.Toggle(skill.PassiveSkill);
            //Конец отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.EndHorizontal();

            //Начало отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EffectID", GUILayout.MaxWidth(50f));
            skill.EffectID = EditorGUILayout.TextField(skill.EffectID);
            //Конец отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.EndHorizontal();
            */
            #endregion

            skill.Icon = EditorGUILayout.ObjectField("Icon:", skill.Icon, typeof(Sprite), false) as Sprite;

            _dicFoldoutStatuses[skill] = EditorGUILayout.Foldout(_dicFoldoutStatuses[skill].Value, "Statuses");

            if (_dicFoldoutStatuses[skill].Value)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Type", GUILayout.Width(_mainDataSize.x / 3f));
                EditorGUILayout.LabelField("ID", GUILayout.Width(_mainDataSize.x / 3f));
                EditorGUILayout.LabelField("Value", GUILayout.Width(_mainDataSize.x / 3f));
                EditorGUILayout.EndHorizontal();
                for (int i = 0; i < skill.Statuses.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    skill.Statuses[i].Type = (StatsType)EditorGUILayout.EnumPopup(skill.Statuses[i].Type);
                    skill.Statuses[i].Data.Id = (ulong)EditorGUILayout.LongField((long)skill.Statuses[i].Data.Id);
                    skill.Statuses[i].Data.Value = EditorGUILayout.FloatField(skill.Statuses[i].Data.Value);

                    if(GUILayout.Button("x"))
                    {
                        skill.Statuses.RemoveAt(i);
                        return;
					}
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add status"))
                {
                    skill.Statuses.Add(new Status());
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private T PrintParam<T>(object value, string name)
        {
            //Начало отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(name, GUILayout.MaxWidth(50f));

            if (typeof(T) == typeof(string))// value is string str)
            {
                value = EditorGUILayout.TextField((string)value);
            }
            else if (typeof(T) == typeof(int))
            {
                value = EditorGUILayout.IntField((int)value);
            }
            else if (typeof(T) == typeof(bool))
            {
                value = EditorGUILayout.Toggle((bool)value);
            }
            else if(typeof(T) == typeof(float))
            {
                value = EditorGUILayout.FloatField((float)value);
			}
            else throw new ApplicationException($"Unsigned type <b>{typeof(T)}</b> in PrintParam");
            
            //Конец отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.EndHorizontal();

            return (T)value;
        }

        #endregion

        protected override void OnGUI()
        {
            base.OnGUI();
            //Отрисовка заголовка
            if (!PrintHeader()) return;
            //Отрисовка таблицы эффектов
            if (!PrintTreeTable()) return;
        }

        private void Init()
        {
            if (_selectSkillTree == null) return;

            var size = _selectSkillTree.Size.x * _selectSkillTree.Size.y;
            if (_skillDataList == null)
            {
                _skillDataList = _selectSkillTree.Skills;
            }

            if (_skillDataList.Count > size)
            {
                while (_skillDataList.Count != size) _skillDataList.RemoveAt(_skillDataList.Count - 1);
            }
            if (_skillDataList.Count < size)
            {
                while (_skillDataList.Count != size) _skillDataList.Add(null);
            }

            if (_dicFoldoutStatuses == null)
            {
                _dicFoldoutStatuses = new Dictionary<SkillData, bool?>(_skillDataList.Count);
            }
        }

        private void DrawHeader(string text)
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField(text, GUIEditorExtensions.GetHeaderLabelStyle(position));
        }
    }
}
