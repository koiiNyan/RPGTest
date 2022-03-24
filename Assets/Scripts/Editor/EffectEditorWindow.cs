using RPG.ScriptableObjects.Configurations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RPG.Editor
{
    public class EffectEditorWindow : BaseCustomEditorWindow
    {
        //Название поля родительского конфига с массивом EffectData
        private const string c_fieldNameWithMainData = "_mainDatas";
        //Путь к подгрузке всех конфигов
        private const string _configPath = "//Configurations";

        //Список всех загруженных конфигов по указанному пути
        private List<BaseEffectConfiguration> _configs;
        //Название файлов всех загруженных конфигов
        private string[] _configNames;
        //Выбранный в данный момент конфиг файл
        private BaseEffectConfiguration _selectConfig;
        //Название класса эффектов, которые описывает выбранный конфиг
        private string _nameTypeGenericChild;
        //Текущий индекс выбранного конфига из массива загруженных
        private int _currentConfigIndex;

        //Массив базовых данных эффектов выбранного конфига
        private List<EffectData> _dataInSelectFile;
        //Словарь с названиями полей и массивом полей значений выбранного конфига эффектов
        private Dictionary<string, List<float>> _additionalDataInSelectFile;

        //Кол-во отрисовываемых колонок таблицы
        private int _columns;
        //Кол-во отрисовываемых строк таблицы
        private int _rows;

        #region Settings

        private Vector2 _mainDataSize = new Vector2(250f, 300f);
        private Vector2 _oldSizeWindow;
        private Vector2 _scroll;
        private bool _valueChange;

        #endregion

        [MenuItem("Extensions/Windows/Effect Editor Window #x", priority = 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<EffectEditorWindow>(false, "Effect Editor Window", true);
            window.minSize = window.maxSize = new Vector2(300f, 450f);
        }

        public override void OnEnable()
        {
            if (EditorPrefs.HasKey("EffectEditor:selectIndex"))
            {
                _currentConfigIndex = EditorPrefs.GetInt("EffectEditor:selectIndex");
            }
            _configs = RPGExtensions.FindAllAssetsByType<BaseEffectConfiguration>(SearchOption.TopDirectoryOnly, _configPath).ToList();
            _configNames = _configs.Select(t => t.name).ToArray();

            LoadDataInSelectFile();
        }

        protected override void OnDisable()
        {
            EditorPrefs.SetInt("EffectEditor:selectIndex", _currentConfigIndex);
        }

        private bool PrintHeader()
        {
            //Строка с полем для анимации
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(GetDefaultSpace);
            EditorGUILayout.LabelField("Select config:", GUILayout.MaxWidth(90f));
            GUILayout.Space(GetDefaultSpace / 2f);

            //Текст выводимый в попап
            var content = new GUIContent(_configs[_currentConfigIndex].name);
            var popupWidth = 2 * GUI.skin.label.CalcSize(content).x;

            var checkChange = _currentConfigIndex;
            _currentConfigIndex = EditorGUILayout.Popup(_currentConfigIndex, _configNames, GUILayout.Width(popupWidth), GUILayout.MaxWidth(200f));

            //Подгрузка новых данных из нового выбранного файла
            if(checkChange != _currentConfigIndex)
            {
                LoadDataInSelectFile();
            }

            GUILayout.Space(GetDefaultSpace);

            EditorGUILayout.LabelField("AutoSave:", GUILayout.Width(80f));

            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Cyan];
            if (GUILayout.Button("Save", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                //Сохраняем файл
                EditorUtility.SetDirty(_selectConfig);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Green];
            if (GUILayout.Button("Add effect", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionMediumSize))
            {
                foreach(var pair in _additionalDataInSelectFile)
                {
                    pair.Value.Add(default(float));
				}
                _dataInSelectFile.Add(default(EffectData));
                CalculateTableSize();
                return false;
			}

            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            //Конец лейбла и поля под анимацию
            EditorGUILayout.EndHorizontal();

            return true;
        }

		#region Table

		private bool PrintEffectsTable()
        {
            DrawHeader("Effects");

            if (_dataInSelectFile == null || _additionalDataInSelectFile == null) LoadDataInSelectFile();

            var length = _dataInSelectFile.Count;

            //Начало отрисовки скролла таблицы
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            //Начало отрисовки строк таблицы
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < _rows; i++)
            {
                if (!PrintLineWithEffects(ref length)) return false;
            }

            //Конец отрисовки строк таблицы
            EditorGUILayout.EndVertical();
            //Конец отрисовки скролла таблицы
            EditorGUILayout.EndScrollView();
            return true;
        }

        private bool PrintLineWithEffects(ref int length)
        {
            //Начало отрисовки одной строки с блоками эффектов
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < _columns && length != 0; i++, length--)
            {
                PrintEffect(_dataInSelectFile.Count - length);
            }

            //Конец отрисовки одной строки с блоками эффектов
            EditorGUILayout.EndHorizontal();
            return true;
        }

        private void PrintEffect(int index)
        {
            //Начало отрисовки отдельного блока с данными эффектов
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(_mainDataSize.x), GUILayout.Height(_mainDataSize.y));
            //Начало отрисовки всех полей одного эффекта друг под дружкой
            EditorGUILayout.BeginVertical();

            PrintEffectData(index);
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(_nameTypeGenericChild, GUIEditorExtensions.SmallHeaderLabelStyle);
            PrintAdditionalData(index);

            //Конец отрисовки всех полей одного эффекта друг под дружкой
            EditorGUILayout.EndVertical();
            //Конец отрисовки отдельного блока с данными эффектов
            EditorGUILayout.EndHorizontal();
        }

        private void PrintEffectData(int index)
        {
            var data = _dataInSelectFile[index];

            //Начало отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", GUILayout.MaxWidth(50f));
            data.Id = EditorGUILayout.TextField(data.Id);
            //Конец отрисовки в одну строку названия поля и самого поля
            EditorGUILayout.EndHorizontal();

            data.Duration = EditorGUILayout.FloatField("Duration:", data.Duration);
            data.Sprite = EditorGUILayout.ObjectField("Sprite:", data.Sprite, typeof(Sprite), false) as Sprite;

            _dataInSelectFile[index] = data;
        }

        private void PrintAdditionalData(int index)
        {
            foreach (var pair in _additionalDataInSelectFile)
            {
                //Начало отрисовки в одну строку названия поля и самого поля
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(pair.Key, GUILayout.MaxWidth(140f));
                pair.Value[index] = EditorGUILayout.FloatField(pair.Value[index]);
                //Конец отрисовки в одну строку названия поля и самого поля
                EditorGUILayout.EndHorizontal();
            }
        }

        #endregion

        protected override void OnGUI()
        {
            base.OnGUI();
            try
            {
                //Пересчет размеров таблицы исходя из размеров окна
                if (_oldSizeWindow.x != position.width || _oldSizeWindow.y != position.height)
                {
                    _oldSizeWindow = new Vector2(position.width, position.height);
                    CalculateTableSize();
                    return;
                }

                //Отрисовка заголовка
                if (!PrintHeader()) return;
                //Отрисовка таблицы эффектов
                if (!PrintEffectsTable()) return;
            }
            catch(System.ApplicationException e0)
            {
                throw e0;
			}
            catch(System.ArgumentOutOfRangeException e1)
            {
                throw e1;
			}
            catch(System.ArgumentException e)
            {
                return;
			}
        }

        private void LoadDataInSelectFile()
        {
            _selectConfig = _configs[_currentConfigIndex];

            var field = _selectConfig.GetType().GetField(c_fieldNameWithMainData, GUIEditorExtensions.GetPrivateReflectionFlags);

            if(field == null || _selectConfig == null)
            {
                if (EditorApplication.isPlaying) return;
                else return;
                //Debug.Log($"LoadDataInSelectFile in <b>{nameof(EffectEditorWindow)}</b> failed, because field == null ({field == null}) or _selectConfig == null ({_selectConfig == null})");
			}
            _dataInSelectFile = field.GetValue(_selectConfig) as List<EffectData>;

            //Отражаем все поля конфига
            _additionalDataInSelectFile = new Dictionary<string, List<float>>();
            var fields = _selectConfig.GetType().GetFields(GUIEditorExtensions.GetPrivateReflectionFlags);
            foreach (var f in fields)
            {
                //Если поле имеет тип массива флотов и помечено, как сериализуемое - берем
                if (f.FieldType != typeof(List<float>) || f.GetCustomAttribute(typeof(SerializeField)) == null) continue;

                _additionalDataInSelectFile.Add(f.Name.Replace("_", ""), f.GetValue(_selectConfig) as List<float>);
            }

            _nameTypeGenericChild = _selectConfig.GetType().BaseType.GenericTypeArguments[0].Name;

            CalculateTableSize();
        }

        private void CalculateTableSize()
        {
            //Сколько колонок умещается в окне
            _columns = (int)Mathf.Floor(position.width / _mainDataSize.x);
            //Сколько потребуется отрисовать строк
            var length = _dataInSelectFile.Count;
            _rows = (int)Mathf.Ceil((float)length / _columns);
        }

        private void DrawHeader(string text)
        {
            GUILayout.Space(5f);
            EditorGUILayout.LabelField(text, GUIEditorExtensions.GetHeaderLabelStyle(position));
        }
    }
}
