using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPG.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _interface;
        [SerializeField]
        private RectTransform _loader;
        [SerializeField]
        private Image _progress;
        [SerializeField]
        private Image _fillProgress;

        [Space, SerializeField, Range(0.1f, 10f)]
        private float _loaderRotateSpeed = 10f;

        private IEnumerator LoadGame()
        {
            _interface.SetActive(false);
            _loader.gameObject.SetActive(true);
            _progress.gameObject.SetActive(true);

            var scene = SceneManager.LoadSceneAsync(1);
            scene.allowSceneActivation = false;

            var speed = new Vector3(0f, 0f, -_loaderRotateSpeed);

            while(scene.progress < 0.9f)
            {
                _loader.Rotate(speed * Time.deltaTime);
                _fillProgress.fillAmount = scene.progress;
                yield return null;
            }

            scene.allowSceneActivation = true;
        }

        public void NewGame_UnityEvent()
        {
            StartCoroutine(LoadGame());
		}
        public void OnSettings_UnityEvent()
        {
            
		}
        public void ExitGame_UnityEvent()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}