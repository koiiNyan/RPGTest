using Newtonsoft.Json;

using RPG.Commands;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPG
{
    [Serializable]
    public struct EffectData
    {
        public string Id;
        public float Duration;
        public Sprite Sprite;

        public EffectData(string id, float duration, Sprite sprite)
        {
            Id = id; Duration = duration; Sprite = sprite;
        }
    }

    [Serializable]
    public struct BotInfoData
    {
        public BotPrefabType BotPrefabType;
        public Units.NPCs.NPCUnit Prefab;

	}

    [Serializable]
    public struct TransformData
    {
        [SerializeField, JsonProperty("xPos")]
        private float _xPos;
        [SerializeField, JsonProperty("yPos")]
        private float _yPos;
        [SerializeField, JsonProperty("zPos")]
        private float _zPos;

        [SerializeField, JsonProperty("xRot")]
        private float _xRot;
        [SerializeField, JsonProperty("yRot")]
        private float _yRot;
        [SerializeField, JsonProperty("zRot")]
        private float _zRot;

        [JsonIgnore]
        public CameraPositionType Type;
        [JsonIgnore]
        public Vector3 Position => new Vector3(_xPos, _yPos, _zPos);
        [JsonIgnore]
        public Vector3 Rotation => new Vector3(_xRot, _yRot, _zRot);

        public TransformData(Transform transform)
        {
            Type = CameraPositionType.Default;
            _xPos = transform.localPosition.x;
            _yPos = transform.localPosition.y;
            _zPos = transform.localPosition.z;
            _xRot = transform.localEulerAngles.x;
            _yRot = transform.localEulerAngles.y;
            _zRot = transform.localEulerAngles.z;
		}

        public void SetLocalTransformData(Transform transform)
        {
            transform.localPosition = Position;
            transform.localEulerAngles = Rotation;
		}
	}

    [Serializable]
    public struct ResolutionData
    {
        public int Width;
        public int Height;
        public int RefreshRate;

        public static implicit operator Resolution(ResolutionData data)
        {
            return new Resolution { width = data.Width, height = data.Height, refreshRate = data.RefreshRate };
		}

        public static implicit operator ResolutionData(Resolution data)
        {
            return new ResolutionData { Width = data.width, Height = data.height, RefreshRate = data.refreshRate };
        }
    }
}
