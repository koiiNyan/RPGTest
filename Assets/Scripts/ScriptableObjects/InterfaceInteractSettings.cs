using RPG.UI.Blocks;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewInterfaceInteractSettings", menuName = "Scriptable Objects/Interface Interact Settings", order = 102)]
    public class InterfaceInteractSettings : ScriptableObject
    {
        [Header("---Визуал ячеек инвентаря---")]
        [SerializeField]
        private Color _exitColor;
        [SerializeField]
        private Color _enterColor;
        [SerializeField, Range(0.01f, 3f)]
        private float _speedFilling = 2f;

        public Color ExitColor => _exitColor;
        public Color EnterColor => _enterColor;
        public float SpeedFilling => _speedFilling;
    }
}
