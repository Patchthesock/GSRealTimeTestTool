using UnityEngine;
using Zenject;

namespace Zenject.Tests.AutoInjecter
{
    public class Qux : MonoBehaviour
    {
        [Inject]
        public DiContainer Container;
    }
}

