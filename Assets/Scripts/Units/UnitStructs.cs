using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace RPG.Units
{
    [Serializable]
    public class Container
    {
        private LinkedList<StatusData> _statuses = new LinkedList<StatusData>();

        [SerializeField]
        private float _defaultValue;

        public Range Range;
        public ContainerType Type;
        public float GetValue { get; private set; }

        public void AddStatus(StatusData statusData)
        {
            _statuses.AddLast(statusData);
            UpdateValue();
        }

        public void RemoveStatusByID(ulong id)
        {
            var status = _statuses.FirstOrDefault(t => t.Id == id);

            _statuses.Remove(status);
            UpdateValue();
        }

        public void UpdateValue()
        {
            switch (Type)
            {
                case ContainerType.Addition:
                    GetValue = _defaultValue + _statuses.Sum(t => t.Value);
                    break;
                case ContainerType.Deduction:
                    GetValue = _defaultValue - _statuses.Sum(t => t.Value);
                    break;
            }

            if (Range.IsInit) GetValue = Mathf.Clamp(GetValue, Range.Min, Range.Max);
        }

        public Container(float defaultValue)
        {
            _defaultValue = defaultValue;
            UpdateValue();
        }
        public Container() { }

        public static implicit operator float(Container c) => c.GetValue;

		public override string ToString() => GetValue.ToString();
    }

	public enum ContainerType : byte
    {
        Addition,
        Deduction,
        Multiplication,
        Division
    }

    [Serializable]
    public struct StatusData
    {
        public ulong Id;
        public float Value;

        public StatusData(ulong id, float value)
        {
            Id = id; Value = value;
		}

		public override string ToString()
		{
            return string.Concat("Id: ", Id, " :=> ", Value);
		}

		public override bool Equals(object obj)
		{
            if (!(obj is StatusData)) return false;
            var stack = (StatusData)obj;

            return stack.Id == Id && stack.Value == Value;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        public static implicit operator float(StatusData a) => a.Value;
        public static explicit operator StatusData(float a) => new StatusData(0, a);

        public static bool operator==(StatusData a, StatusData b)
        {
            return a.Id == b.Id && a.Value == b.Value;
		}

        public static bool operator!=(StatusData a, StatusData b)
        {
            return a.Id != b.Id || a.Value != b.Value;
		}
	}

    [Serializable]
    public readonly struct Range
    {
        public readonly bool IsInit;
        public readonly float Min;
        public readonly float Max;

        public Range(float min, float max)
        {
            Min = min;
            Max = max;
            IsInit = true;
		}
	}

    [Serializable]
    public class Status
    {
        public StatsType Type;
        public StatusData Data;

        public Status Clone()
        {
            return new Status() { Data = new StatusData(Data.Id, Data.Value), Type = Type };
		}
	}
}
