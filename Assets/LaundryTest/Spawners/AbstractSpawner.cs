using LaundryTest.Buildings.Blocks;
using UnityEngine;

namespace LaundryTest.Spawners
{
    public abstract class AbstractSpawner : MonoBehaviour
    {
        private BuildBlock _lastCopy;

        public BuildBlock Spawn()
        {
            if (_lastCopy != null)
            {
                SetInitialTransformations(_lastCopy);
                _lastCopy.gameObject.SetActive(true);
            }
            else
            {
                _lastCopy = CreateInstance();
            }

            return _lastCopy;
        }

        protected abstract BuildBlock CreateInstance();
        protected abstract void SetInitialTransformations(BuildBlock block);

        public void Apply()
        {
            _lastCopy = null;
        }

        public void Cancel()
        {
            _lastCopy.gameObject.SetActive(false);
        }
    }
}