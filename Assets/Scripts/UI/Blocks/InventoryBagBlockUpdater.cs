using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;

namespace RPG.UI.Blocks
{
    public partial class InventoryBagBlock
    {
		private float _speedFilling;
		private float4 _exitColorFloat;
		private float4 _enterColorFloat;

		public void SetIterfaceParams(Color exitColor, Color enterColor, float speedFilling)
		{
			_speedFilling = speedFilling;
			_exitColorFloat = exitColor.SimpleConvert();
			_enterColorFloat = enterColor.SimpleConvert();
		}

		private void Update()
		{
			var colors = new NativeArray<float4>(_pool.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			var fillings = new NativeArray<float>(_pool.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			var states = new NativeArray<bool>(_pool.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

			int i = 0;
			foreach (var element in _pool)
			{
				colors[i] = element.BackgroundColor.SimpleConvert();
				fillings[i] = element.EnterFill;
				states[i] = element.EnterCursor;
				i++;
			}

			var job = new RecolorJob
			{
				ExitColor = _exitColorFloat,
				EnterColor = _enterColorFloat,
				DeltaTime = TimeAssistant.UIDeltaTime,
				SpeedFilling = _speedFilling,
				ColorsArray = colors,
				FillingArray = fillings,
				StateArray = states
			};

			job.Schedule().Complete();

			i = 0;
			foreach (var element in _pool)
			{
				element.BackgroundColor = colors[i].SimpleConvert();
				element.EnterFill = fillings[i];
				i++;
			}

			colors.Dispose();
			fillings.Dispose();
			states.Dispose();
		}

		//[BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Medium)]
		private struct RecolorJob : IJob
		{
			[Unity.Collections.ReadOnly] public float4 ExitColor;
			[Unity.Collections.ReadOnly] public float4 EnterColor;
			[Unity.Collections.ReadOnly] public float DeltaTime;
			[Unity.Collections.ReadOnly] public float SpeedFilling;

			
			public NativeArray<float4> ColorsArray;
			public NativeArray<float> FillingArray;
			[Unity.Collections.ReadOnly] public NativeArray<bool> StateArray;

			public void Execute()
			{
				for(int i = 0; i < ColorsArray.Length; i++)
				{
					//При наведенном курсоре заполнение стремится к 1
					//При убранном курсоре заполнение стремится к 0
					if ((StateArray[i] && FillingArray[i] >= 1f) || (!StateArray[i] && FillingArray[i] <= 0f)) continue;

					FillingArray[i] += DeltaTime * (StateArray[i] ? SpeedFilling : -SpeedFilling);
					ColorsArray[i] = math.lerp(ExitColor, EnterColor, FillingArray[i]);
				}
			}
		}
	}
}
