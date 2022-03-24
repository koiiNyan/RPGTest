using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Editor.Generator
{
    public static class ConstantsGerenator
    {
        private static readonly string _inputActionAssetPath = "Assets/Scripts/Units/Players/PlayerControls.inputactions";
        private static readonly string _inputActionConstGerenatePath = "Scripts/Generated/InputActionConstants.cs";
        private static readonly string _baseNamespace = "RPG";


        [InitializeOnLoadMethod]
        private static void GenerateInputActionConstants()
        {
            GetAllActionInputsName(out var array, out var contants);

            if (array == null || contants == null) return;

            var str = new StringBuilder();
            foreach(var c in contants)
            {
                str.Append($"\n\t\t public static string {c} => \"{c}\";");
			}

            var path = string.Concat(Application.dataPath, "/", _inputActionConstGerenatePath);
            var code = string.Concat(
            $"namespace {_baseNamespace} \n{{",
            $"\n\tpublic static partial class Constants\n\t{{",
            $"\n\t\tpublic static string[] GetInputActionMapName => new string[]\n\t\t{{",
            array,
            $"\n\t\t}};\n",
            str,
            $"\t\t",
            $"\n\t}}\n}}");


            System.IO.File.WriteAllText(path, code);
        }

        private static void GetAllActionInputsName(out string array, out string[] constants, string map = "Unit")
        {
            var controls = AssetDatabase.LoadAssetAtPath(_inputActionAssetPath, typeof(InputActionAsset)) as InputActionAsset;
            if (controls == null)
            {
                array = null;
                constants = null;
                return;
            }

            var actions = controls.FindActionMap(map).actions;
            var str = new StringBuilder();
            constants = new string[actions.Count];
            for (int i = 0; i < actions.Count; i++)
            {
                str.Append(string.Concat("\n\t\t\t\"", actions[i].name, "\","));
                constants[i] = actions[i].name;
            }

            array = str.ToString();
        }
    }
}