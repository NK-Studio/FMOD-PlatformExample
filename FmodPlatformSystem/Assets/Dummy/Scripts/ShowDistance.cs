using UnityEngine;
using UnityEngine.UI;

namespace Dummy
{
    public class ShowDistance : MonoBehaviour
    {
        private Text _text;

        public Transform target01;
        public Transform target02;

        private void Awake()
        {
            TryGetComponent(out _text);
        }

        private void Update()
        {
            _text.text = $"Distance: {Vector3.Distance(target01.position, target02.position):N0}M";
        }
    }
}
