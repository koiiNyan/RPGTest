using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.Items
{
    [RequireComponent(typeof(MeshRenderer))]
    public class EquipmentComponent : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer _mesh;

        public bool Activity
        {
            get => enabled;
            set
            {
                enabled = value;
                _mesh.enabled = value;
			}
		}

        public EquipmentType Equipment { get; set; }
    }
}