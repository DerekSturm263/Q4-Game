using UnityEngine;
using Types;

namespace SingletonBehaviours
{
    public abstract class SpawnableController<T> : SingletonBehaviour<SpawnableController<T>>
    {
        [SerializeField] protected GameObject _template;

        protected GameObject _templateInstance;
        protected T _t;

        protected abstract bool TakeAwayFocus();
        protected virtual bool CanSpawn => true;
        protected int _activeCount;

        public void Spawn(T t)
        {
            if (!CanSpawn)
                return;

            ++_activeCount;

            if (TakeAwayFocus())
                EventSystemController.Instance.Enable();

            Transform parent = GameObject.FindWithTag("Popup Canvas").transform;
            _templateInstance = Instantiate(_template, parent);

            _t = t;
            SetUp(_t);
        }

        protected virtual void SetUp(T t) { }

        public void Despawn()
        {
            CleanUp(_t);

            _templateInstance.GetComponent<Animator>().SetTrigger("Exit");

            if (TakeAwayFocus())
                EventSystemController.Instance.Disable();

            --_activeCount;
        }

        protected virtual void CleanUp(T t) { }
    }
}
