using UnityEngine;
using Zenject;

namespace Services
{
    public class PrefabBuilder
    {
        public PrefabBuilder(DiContainer container)
        {
            _container = container;
        }

        /***
         * <summary>Instantiate will construct and return an in game GameObject from a Prefab GameObject</summary>
         * <param name="model">The prefab Gameobject to construct in game and return</param>
        ***/
        public GameObject Instantiate(GameObject model)
        {
            return _container.InstantiatePrefab(model);
        }

        private readonly DiContainer _container;
    }
}