using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace RPG.UI.Elements
{
    [RequireComponent(typeof(Animation))]
    public class ConfirmElement : MonoBehaviour
    {
        private Coroutine _coroutineDelay;
        private Action<bool> _action;

        [SerializeField]
        private string _richTextStart;
        [SerializeField]
        private Animation _tremorClip;

        [Space, SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private TextMeshProUGUI _delay;
        [SerializeField]
        private TextMeshProUGUI _trueOptionText;
        [SerializeField]
        private TextMeshProUGUI _falseOptionText;

        public ResultHandler CreateAsyncWaiting(string text, OptionType trueOption, OptionType falseOption, int time = -1)
        {
            gameObject.SetActive(true);
            _text.text = text;

            _trueOptionText.text = StringHelper.GetOptionTypeLocalization(trueOption);
            _falseOptionText.text = StringHelper.GetOptionTypeLocalization(falseOption);
            if (time != -1)
            {
                _delay.enabled = true;
                _coroutineDelay = StartCoroutine(Delay(time));
            }
            else _delay.enabled = false;

            return ResultHandler.GetHandler(out _action);
        }

        private void Call(bool value)
        {
            if (_tremorClip.isPlaying) return;

            gameObject.SetActive(false);
            if (_coroutineDelay != null)
            {
                StopCoroutine(_coroutineDelay);
                _coroutineDelay = null;
            }

            _action.Invoke(value);
		}

        private IEnumerator Delay(int delay)
        {
            while(delay > 0)
            {
                _delay.text = string.Concat(StringHelper.ConfirmDelayText, _richTextStart, delay);
                delay--;
                yield return new WaitForSeconds(1f);
			}

            _coroutineDelay = null;
            Call(false);
		}

        public void OnTrue_EventUnity() => Call(true);
        public void OnFalse_EventUnity() => Call(false);
        public void OnMiss_eventUnity() => _tremorClip.Play();
    }
}
