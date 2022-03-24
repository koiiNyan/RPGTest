using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Editor
{
#if UNITY_EDITOR
    public class LevelCheckComponent : MonoBehaviour
    {

        void Start()
        {
            var objects = GetComponentsInChildren<Transform>(true);
            var exceptions = new LinkedList<string>();
            
            foreach(var obj in objects)
            {
                var check1 = !obj.CompareTag(Constants.FloorTag);
                var check2 = obj.gameObject.layer != Constants.ObstacleLayerInt;

                if (check1 && check2) exceptions.AddLast($"<b>{obj.name}</b> does not have the correct tag and layer\n");
                else if(check1) exceptions.AddLast($"<b>{obj.name}</b> does not have the correct tag\n");
                else if (check2) exceptions.AddLast($"<b>{obj.name}</b> does not have the correct layer\n");
            }

            if(exceptions.Count > 0)
                EditorExtensions.LogError(exceptions, PriorityMessageType.Critical);
        }
    }
#endif
}
