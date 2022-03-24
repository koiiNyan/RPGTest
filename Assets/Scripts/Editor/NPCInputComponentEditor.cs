using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG.Units.NPCs;
using System.Linq;

namespace RPG.Editor
{
    //[CustomEditor(typeof(NPCInputComponent))]
    [CanEditMultipleObjects]
    public class NPCInputComponentEditor : UnityEditor.Editor
    {
        private static string _pointsParentName = "_PATROLLING_POINTS";
        private static bool _isFoldout;
        private static Transform _pointsParent;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //Лучше всегда вызывать в начале отрисовки, чтобы данные в инспекторе были актуальны
            serializedObject.Update();
            

            if (EditorApplication.isPlaying) return;

            _isFoldout = EditorGUILayout.Foldout(_isFoldout, "Editor Settings");
            if (!_isFoldout) return;

            EditorGUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Green];
            if(GUILayout.Button("Create Transforms", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionBigSize))
            {
                var vectors = (target as NPCInputComponent).GetType().GetField("_patrollingPoints", GUIEditorExtensions.GetPrivateReflectionFlags).GetValue(target) as Vector3[];
                if (vectors.Length == 0) return;

                for (int i = 0; i < vectors.Length; i++)
                {
                    CreatePoint(vectors[i], i, target as NPCInputComponent);
				}
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Yellow];
            if (GUILayout.Button("Compile Vectors", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionBigSize))
            {
                var field = (target as NPCInputComponent).GetType().GetField("_patrollingPoints", GUIEditorExtensions.GetPrivateReflectionFlags);

                var points = FindObjectsOfType<EditorPatrollingPoint>().Where(t => t.Target == target).ToList();
                points.Sort(Comparer);

                field.SetValue(target, points.Select(t => t.transform.position).ToArray());
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.color = GUIEditorExtensions.ColorGUI[GUIEditorExtensions.ColorGUIType.Red];
            if (GUILayout.Button("Remove Temp Objects", GUIEditorExtensions.ButtonStyleFontSize16, GUIEditorExtensions.ButtonOptionBigSize))
            {
                FindOrCreateCreateParent();
                DestroyImmediate(_pointsParent.gameObject);
                _pointsParent = null;
            }

            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            //Лучше всегда вызывать для обновления инспектора
            serializedObject.ApplyModifiedProperties();
        }

        private void FindOrCreateCreateParent()
        {
            _pointsParent = FindObjectsOfType<Transform>().FirstOrDefault(t => t.name == EditorConstants.EditorGameObjectName + _pointsParentName);

            if (_pointsParent != null) return;
            _pointsParent = new GameObject().transform;
            _pointsParent.position = _pointsParent.eulerAngles = Vector3.zero;
            _pointsParent.localScale = Vector3.one;
            _pointsParent.name = EditorConstants.EditorGameObjectName + _pointsParentName;
        }

        private void CreatePoint(Vector3 position, int index, NPCInputComponent target)
        {
            FindOrCreateCreateParent();

            var point = new GameObject();
            point.transform.SetParent(_pointsParent, false);
            point.name = EditorConstants.EditorGameObjectName + "PatrollingPoint";
            point.transform.position = position;
            point.transform.eulerAngles = Vector3.zero;
            point.transform.localScale = new Vector3(.2f, 3f, .2f);
            var component = point.AddComponent<EditorPatrollingPoint>();
            component.Target = target;
            component.Index = index;
        }

        private int Comparer(EditorPatrollingPoint x, EditorPatrollingPoint y)
        {
            if (x.Index < y.Index) return -1;
            if (x.Index > y.Index) return 1;
            return 0;
		}
    }
}