using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPG.Editor
{
#if UNITY_EDITOR
    public class UnitParses : MonoBehaviour
    {
        public string FullJsonName;
        public TextAsset SaveData;

        [ContextMenu("Parse")]
        public void Parse()
        {
            var token = JToken.Parse(SaveData.text);
            Assistants.ConstructEntityExtensions.LoadGameObjectState(gameObject, token[FullJsonName] as JObject);
            DestroyImmediate(this);
        }
    }

    [CustomEditor(typeof(UnitParses))]
    public class UnitParserEditor : UnityEditor.Editor
    {
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

            EditorGUILayout.Space(20f);

            GUI.color = new Color(0.5f, 1f, 0.7f);
            if (GUILayout.Button("Parse"))
            {
                (target as UnitParses).Parse();
            }
            GUI.color = Color.white;
		}
	}
#endif
}
