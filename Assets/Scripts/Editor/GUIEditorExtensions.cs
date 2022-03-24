using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEngine;

namespace RPG.Editor
{
    public abstract class BaseCustomEditorWindow : EditorWindow
    {

        protected float GetDefaultSpace => position.width / 20f;

        public abstract void OnEnable();
        protected abstract void OnDisable();

        protected virtual void OnGUI()
        {
            GUIEditorExtensions.ReInit();
		}
	}

    public static class GUIEditorExtensions
    {
        public static GUIStyle ButtonStyleFontSize16;
        public static GUILayoutOption[] ButtonOptionMediumSize;
        public static GUILayoutOption[] ButtonOptionBigSize;
        public static GUIStyle SmallHeaderLabelStyle;
        public static Dictionary<ColorGUIType, Color> ColorGUI;
        public static System.Reflection.BindingFlags GetPrivateReflectionFlags;
        public static string test;
        public static GUIStyle GetHeaderLabelStyle(Rect windowRect)
        {
            return new GUIStyle("label")
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset((int)(windowRect.width / 2f - 50f), (int)(windowRect.width / 2f - 50f), 0, 0)
            };
        }

        public static void ReInit()
        {
            if(ButtonStyleFontSize16 == null)
            ButtonStyleFontSize16 = new GUIStyle("button")
            {
                fontSize = 16
            };
            if (ButtonOptionMediumSize == null)
                ButtonOptionMediumSize = new GUILayoutOption[]
            {
                GUILayout.Width(120f),
                GUILayout.Height(20f)
            };
            if (ButtonOptionBigSize == null)
                ButtonOptionBigSize = new GUILayoutOption[]
            {
                GUILayout.Width(200f),
                GUILayout.Height(30f)
            };
            if (SmallHeaderLabelStyle == null)
                SmallHeaderLabelStyle = new GUIStyle("label")
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            if (ColorGUI == null)
                ColorGUI = new Dictionary<ColorGUIType, Color>
            {
                { ColorGUIType.Default, Color.white },
                { ColorGUIType.Green, new Color(0.5f, 1f, 0.7f) },
                { ColorGUIType.Red, new Color(1f, 0.5f, 0.5f) },
                { ColorGUIType.Cyan, new Color(0f, 0.7f, 0.7f, 1f) },
                { ColorGUIType.Yellow, new Color(1f, 0.99f, 0.59f) },
                { ColorGUIType.Purple, new Color(0.85f, 0.6f, 1f) },
                { ColorGUIType.Turquoise, new Color(0.8f, 1f, 0.98f) },
            };
            GetPrivateReflectionFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        }

        public enum ColorGUIType : byte
        {
            Default,
            Green,
            Red,
            Cyan,
            Yellow,
            Purple,
            Turquoise,
        }
    }
}
