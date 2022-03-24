using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace RPG.UI
{
    public class GameObjectPool<T> : IEnumerable<T>, IReadOnlyCollection<T> where T : MonoBehaviour
    {
        private T _prefab;
        private LinkedList<T> _elements = new LinkedList<T>();

        public int Count => _elements.Count;

        public T GetOrCreateElement(out bool isNewElement)
        {
            var element = _elements.FirstOrDefault(t => !t.gameObject.activeSelf);

            if (element == null)
            {
                element = GameObject.Instantiate(_prefab);
                _elements.AddLast(element);

                isNewElement = true;
            }
            else
            {
                element.gameObject.SetActive(true);
                element.transform.SetAsLastSibling();

                isNewElement = false;
            }

            return element;
        }

        public void DisableAllElements()
        {
            foreach(var element in _elements)
            {
                element.gameObject.SetActive(false);
			}
		}

        public void OnDestroy()
        {
            DisableAllElements();
        }

		public GameObjectPool(T prefab)
        {
            _prefab = prefab;
        }
        public IEnumerator<T> GetEnumerator() => _elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();
    }
}
