#if UNITY_EDITOR
using RPG.Units.NPCs;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEditor;
using UnityEngine;

namespace RPG
{
    [ExecuteAlways]
    public class EditorPatrollingPoint : MonoBehaviour
    {
        public NPCInputComponent Target;
        public int Index;

		private void Update()
		{
            if (Physics.Raycast(transform.position, -transform.up, out var hit, 1000f))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
        }

		private void OnDrawGizmos()
		{
            Gizmos.color = Color.green;
            var position = transform.position;
            position.y = position.y + transform.localScale.y / 2f;
            Gizmos.DrawCube(position, transform.localScale);

            GUI.color = new Color(.9f, .4f, .4f);
            var pos = transform.position;
            pos.y = transform.localScale.y + .6f;

            Handles.Label(pos, Index.ToString(), Editor.EditorConstants.GizmosLabelStyle);
        }
	}

	[System.Serializable]
	public struct Point
	{
		public MeshFilter Object;
		public TextMeshPro Text;
	}

	[CustomEditor(typeof(EditorPatrollingPoint))]
    [CanEditMultipleObjects]
    public class EditorPatrollingPointEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("This object can only exist in the editor. After finishing work, you need to call the configuration of the <b>GameManager</b> through the context menu");
            EditorGUILayout.Separator();
            EditorGUILayout.Space(10f);
            base.OnInspectorGUI();
		}
    }
}
#endif
