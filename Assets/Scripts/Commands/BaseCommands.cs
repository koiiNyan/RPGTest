using Newtonsoft.Json;

using RPG.Units;
using UnityEngine;

namespace RPG.Commands
{
    public abstract class BaseCommand<T> where T : class
    {
        [JsonIgnore]
        public Unit Source { get; set; }

        public abstract void OnStart();
        public abstract void OnUpdate(float delta);
        public abstract void OnEnd();

        public abstract T Clone();

        public abstract string Serialize();
        public abstract void Deserialize(string code);
    }

    public abstract class BaseCommandEffect : BaseCommand<BaseCommandEffect>
    {
        private float _duration;

        [JsonIgnore]
        public Unit Target { get; set; }

        public float Duration
        {
            get => _duration;
            private set => _duration = CurrentDuration = value;
        }

        public float CurrentDuration { get; set; }

        public string ID { get; }

        [JsonIgnore]
        public Sprite Sprite { get; }

        public BaseCommandEffect(EffectData data)
        {
            Duration = data.Duration; ID = data.Id; Sprite = data.Sprite;
        }
    }

    public abstract class BaseCommandSkill : BaseCommandEffect
    {
        public string AnimationKey { get; }

        public BaseCommandSkill(string animationKey, EffectData data) : base(data)
        {
            AnimationKey = animationKey;
		}
    }
}
