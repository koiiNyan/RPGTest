using RPG.ScriptableObjects.Contexts;
using RPG.Units;
using RPG.Units.Items;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace RPG.Editor
{
    public class ItemWindow : BaseCustomEditorWindow
    {
        private readonly string _texturePath = "Assets/Editor/PreviewRenderTexture.renderTexture";
        private readonly string _previewObjectPath = "Assets/Editor/Render_EDITOR_OBJECT.prefab";
        private RenderTexture _texture;
        private PreviewRenderEditor _previewObject;
        private dynamic _context;

        private int _currentSelectItem;
        private string _filter;


        private float _previewStep = 250f;
        private Vector2 _scroll;
        private GUIStyle _style;
        private Color _evenColor;
        private Color _unevenColor;

        public BaseItemContext SelectItems
        {
            get => _context as BaseItemContext;
            set
            {
                _context = value;
                _currentSelectItem = 0;
            }
        }

        [MenuItem("Extensions/Windows/Item Window #i", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<ItemWindow>(false, "Item Window", true);
            window.minSize = window.maxSize = new Vector2(300f, 450f);
        }

        public override void OnEnable()
        {
            if (EditorPrefs.HasKey("ItemTest:select"))
                SelectItems = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString("ItemTest:select"), typeof(BaseItemContext)) as BaseItemContext;

            FindOrCreateRenderObjects();
        }

        protected override void OnDisable()
        {
            if (SelectItems != null)
                EditorPrefs.SetString("ItemTest:select", AssetDatabase.GetAssetPath(SelectItems));

            RemoveRenderObjects();
        }

        private bool PrintHeader()
        {
            //Строка с полем для анимации
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(GetDefaultSpace / 3f);
            EditorGUILayout.LabelField("Items:", GUILayout.MaxWidth(38f));

            //Выбранный квест
            var newRef = EditorGUILayout.ObjectField(SelectItems, typeof(BaseItemContext), false, GUILayout.Width(180f)) as BaseItemContext;
            if (newRef != SelectItems) SelectItems = newRef;

            if (SelectItems != null)
            {
                GUILayout.Space(GetDefaultSpace / 5f);
                EditorGUILayout.LabelField("Type:", GUILayout.MaxWidth(38f));
                SelectItems.Type = (ItemType)EditorGUILayout.EnumPopup(SelectItems.Type, GUILayout.Width(120f));
                GUILayout.Space(5f);

                EditorGUILayout.LabelField("Rarity:", GUILayout.MaxWidth(43f));
                SelectItems.Rarity = (RarityItemType)EditorGUILayout.EnumPopup(SelectItems.Rarity, GUILayout.Width(120f));
            }

            GUILayout.Space(GetDefaultSpace);
            GUI.color = new Color(0.5f, 1f, 0f, 1f);
            if (GUILayout.Button("Load", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                LoadItemWindow.ShowLoadItemWindow(this);
                return false;
            }

            GUI.color = new Color(0.5f, 1f, 0f, 1f);
            if (GUILayout.Button("Compile", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                foreach(BaseItem element in _context.Items)
                {
                    element.Type = SelectItems.Type;
                    element.Rarity = SelectItems.Rarity;
				}
                EditorUtility.SetDirty(SelectItems);
                //Сохраняем файл
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return false;
            }

            GUI.color = Color.white;

            GUILayout.FlexibleSpace();
            //Конец лейбла и поля под анимацию
            EditorGUILayout.EndHorizontal();

            if (SelectItems == null) return false;
            return true;
        }

        private bool PrintList()
        {
            var width = position.width * 0.4f - 20f;
            EditorGUILayout.BeginVertical("box", GUILayout.Width(width));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter", GUILayout.Width(50f));
            _filter = EditorGUILayout.TextField(_filter, GUILayout.ExpandWidth(true))?.ToLower();
            EditorGUILayout.EndHorizontal();

            //Начало отрисовки скролла списка
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            var withFilter = !string.IsNullOrEmpty(_filter);
            var count = (int)_context.Items.Count;
            for (int i = 0; i < count; i++)
            {
                BaseItem item = _context.Items[i];

                //Если есть фильтр, пропускаем что не проходит его
                if(withFilter && !item.ID.ToLower().Contains(_filter)) continue;

                GUI.color = i == _currentSelectItem 
                    ? GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Purple] 
                    : i % 2 == 0 
                        ? _evenColor 
                        : _unevenColor;

                //Начало отрисовки плашки с элементом
                EditorGUILayout.BeginHorizontal("box");
                GUI.color = Color.white;

                if (GUILayout.Button(item.ID, _style, GUILayout.ExpandWidth(true)))
                {
                    _currentSelectItem = i;
                }

                GUI.color = Color.white;

                if (string.IsNullOrEmpty(_filter))
                {
                    if (GUILayout.Button("Up", GUILayout.Width(34f)))
                    {
                        if (i == 0) return false;
                        var pref = _context.Items[i - 1];
                        _context.Items[i - 1] = _context.Items[i];
                        _context.Items[i] = pref;
                        return false;
                    }
                    if (GUILayout.Button("Down", GUILayout.Width(45f)))
                    {
                        if (i + 1 == (int)_context.Items.Count) return false;
                        var next = _context.Items[i + 1];
                        _context.Items[i + 1] = _context.Items[i];
                        _context.Items[i] = next;
                        return false;
                    }
                }
                if (GUILayout.Button("X", GUILayout.Width(20f)))
                {
                    _context.Items.RemoveAt(i);
                    if (_currentSelectItem >= _context.Items.Count) _currentSelectItem = Mathf.Clamp((int)_context.Items.Count - 1, 0, (int)_context.Items.Count - 1);
                    return false;
                }
                //Конец отрисовки плашки с элементом
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(100f)))
            {
				switch (SelectItems.Type)
				{
					case ItemType.Other:
                        _context.Items.Add(new EmptyTempItem());
                        break;
					case ItemType.Ingredient:
                        _context.Items.Add(new EmptyTempItem());
                        break;
					case ItemType.Material:
                        _context.Items.Add(new EmptyTempItem());
                        break;
					case ItemType.Ammunition:
                        _context.Items.Add(new EmptyTempItem());
                        break;
					case ItemType.Equipment:
                        _context.Items.Add(new EquipmentItem());
                        break;
				}
			}
			GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            //Конец отрисовки скролла списка
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
            return true;
		}


		private bool PrintPreview()
        {
            if (_context.Items.Count == 0) return true;
            var width = position.width * 0.6f - 20f;
            EditorGUILayout.BeginVertical("box", GUILayout.Width(width));

            BaseItem item = _context.Items[_currentSelectItem];

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID:", GUILayout.Width(width * 0.3f));
            item.ID = EditorGUILayout.TextField(item.ID, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            item.Icon = EditorGUILayout.ObjectField("Icon: ", item.Icon, typeof(Sprite), false) as Sprite;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Unstackable:", GUILayout.Width(width * 0.3f));
            item.Unstackable = EditorGUILayout.Toggle(item.Unstackable, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            if (!item.Unstackable)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("MaxStack:", GUILayout.Width(width * 0.3f));
                item.MaxStack = EditorGUILayout.IntField(item.MaxStack, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EffectID:", GUILayout.Width(width * 0.3f));
            item.EffectID = EditorGUILayout.TextField(item.EffectID, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description:", GUILayout.Width(width * 0.3f));
            item.DescriptionID = EditorGUILayout.TextArea(item.DescriptionID, GUILayout.ExpandWidth(true), GUILayout.Height(50f));
            EditorGUILayout.EndHorizontal();

            if (!PrintAdditionalData(item, width)) return false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type", GUILayout.Width(width / 3f));
            EditorGUILayout.LabelField("ID", GUILayout.Width(width / 3f));
            EditorGUILayout.LabelField("Value", GUILayout.Width(width / 3f));
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < item.Statuses.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                item.Statuses[i].Type = (StatsType)EditorGUILayout.EnumPopup(item.Statuses[i].Type);
                item.Statuses[i].Data.Id = (ulong)EditorGUILayout.LongField((long)item.Statuses[i].Data.Id);
                item.Statuses[i].Data.Value = EditorGUILayout.FloatField(item.Statuses[i].Data.Value);

                if (GUILayout.Button("x"))
                {
                    item.Statuses.RemoveAt(i);
                    return false;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add status"))
            {
                item.Statuses.Add(new Status());
            }

            //Конец отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.EndHorizontal();

            if (_texture == null || _previewObject == null) FindOrCreateRenderObjects();
            _previewObject.SetItemCellValue(item);

            var rect = new Rect(position.width - _previewStep, position.height - _previewStep, _previewStep, _previewStep);
            EditorGUI.DrawPreviewTexture(rect, _texture);

            EditorGUILayout.EndVertical();
            return true;
        }

        private bool PrintAdditionalData(BaseItem item, float width)
        {
            switch (SelectItems.Type)
            {
                /*case ItemType.Other:
                    break;
                case ItemType.Ingredient:
                    break;
                case ItemType.Material:
                    break;
                case ItemType.Ammunition:
                    break;*/
                case ItemType.Equipment:
                    var equip = item as EquipmentItem;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Equipment:", GUILayout.Width(width * 0.15f));
                    equip.Equipment = (EquipmentType)EditorGUILayout.EnumPopup(equip.Equipment);
                    EditorGUILayout.LabelField("Prefab:", GUILayout.Width(width * 0.15f));
                    equip.Prefab = EditorGUILayout.ObjectField(equip.Prefab, typeof(EquipmentComponent), false) as EquipmentComponent;
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            return true;
		}

        protected override void OnGUI()
        {
            InitData();
            base.OnGUI();
            //Отрисовка заголовка
            if (!PrintHeader()) return;

            //Отрисовка блоков
            EditorGUILayout.BeginHorizontal();

            if (!PrintList())
            {
                EditorGUILayout.EndHorizontal();//чтобы не ломать форматирование
                return;
            }
            if (!PrintPreview())
            {
                EditorGUILayout.EndHorizontal();//чтобы не ломать форматирование
                return;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void FindOrCreateRenderObjects()
        {
            if (_previewObject != null && _texture != null) return;
            _texture = AssetDatabase.LoadAssetAtPath<RenderTexture>(_texturePath);

            var renderer = FindObjectsOfType<PreviewRenderEditor>();

            if (renderer.Length > 1)
            {
                for (int i = 0; i < renderer.Length; i++)
                {
                    if (_previewObject == null) _previewObject = renderer[i];
                    else DestroyImmediate(renderer[i].gameObject);
                }
            }

            if (_previewObject == null)
            {
                _previewObject = Instantiate(AssetDatabase.LoadAssetAtPath<PreviewRenderEditor>(_previewObjectPath));
            }
        }

        private void RemoveRenderObjects()
        {
            if (_previewObject != null)
            {
                DestroyImmediate(_previewObject.gameObject);
            }

            var renderer = FindObjectsOfType<PreviewRenderEditor>();

            for (int i = 0; i < renderer.Length; i++)
            {
                DestroyImmediate(renderer[i].gameObject);
            }
        }

        private void InitData()
        {
            if(_style == null)
            _style = new GUIStyle("label")
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };

            _evenColor = new Color(0.8584906f, 0.8584906f, 0.8584906f, 1f);
            _unevenColor = new Color(0.8764706f, 0.873754f, 0.990566f, 1f);
        }
	}
}
