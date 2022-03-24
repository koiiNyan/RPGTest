using RPG;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAssistant : Time
{
    private static float _gameCoef = 1f;
    private static float _uiCoef = 1f;

    public static event SimpleHandle<float> OnGameScaleChangeEventHandler;
    public static event SimpleHandle<float> OnUIScaleChangeEventHandler;

    public static float GameDeltaTime => deltaTime * _gameCoef;

    public static float UIDeltaTime => deltaTime * _uiCoef;

    public static void SetGameDeltaTime(float value)
    {
        _gameCoef = value;
        OnGameScaleChangeEventHandler?.Invoke(value);
    }

    public static void SetUIDeltaTime(float value)
    {
        _uiCoef = value;
        OnUIScaleChangeEventHandler?.Invoke(value);
    }
}
