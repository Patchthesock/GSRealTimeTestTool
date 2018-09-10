using UnityEngine;
using Zenject;

namespace Zenject.Tests.AutoInjecter
{
    public class Gorp : MonoBehaviour
    {
        [Inject]
        public DiContainer Container;
    }
}

