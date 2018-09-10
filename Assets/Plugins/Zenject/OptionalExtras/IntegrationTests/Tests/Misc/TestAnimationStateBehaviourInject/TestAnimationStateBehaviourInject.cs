#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject.Tests;

namespace Zenject.Tests.TestAnimationStateBehaviourInject
{
    public class TestAnimationStateBehaviourInject : ZenjectIntegrationTestFixture
    {
        const string ResourcePrefix = "TestAnimationStateBehaviourInject/";

        [UnityTest]
        public IEnumerator Test1()
        {
            PreInstall();
            var prefab = FixtureUtil.GetPrefab(ResourcePrefix + "Foo");

            StateBehaviour1.OnStateEnterCalls = 0;

            Container.InstantiatePrefab(prefab);
            Container.BindInterfacesAndSelfTo<Foo>().AsSingle();
            PostInstall();

            yield return null;

            Assert.IsEqual(StateBehaviour1.OnStateEnterCalls, 1);
            yield break;
        }

        public class Foo : IInitializable
        {
            public Foo()
            {
            }

            public bool HasInitialized
            {
                get; private set;
            }

            public void Initialize()
            {
                HasInitialized = true;
            }
        }
    }
}

#endif
