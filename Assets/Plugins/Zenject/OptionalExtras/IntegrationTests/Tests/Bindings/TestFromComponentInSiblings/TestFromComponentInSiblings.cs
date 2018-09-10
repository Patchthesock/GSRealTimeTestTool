#if UNITY_EDITOR

using System;
using System.Collections;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    public class TestFromComponentSibling : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator RunTestSingleMatch()
        {
            var foo = new GameObject().AddComponent<Foo>();

            var bar = foo.gameObject.AddComponent<Bar>();
            var qux1 = foo.gameObject.AddComponent<Qux>();
            foo.gameObject.AddComponent<Qux>();

            PreInstall();

            Container.Bind<Qux>().FromComponentSibling();
            Container.Bind<Bar>().FromComponentSibling();
            Container.Bind<IBar>().FromComponentSibling();

            PostInstall();

            Assert.IsEqual(foo.Bar, bar);
            Assert.IsEqual(foo.IBar, bar);
            Assert.IsEqual(foo.Qux.Count, 1);
            Assert.IsEqual(foo.Qux[0], qux1);
            Assert.IsEqual(qux1.OtherQux, qux1);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunTestMultiple()
        {
            var foo = new GameObject().AddComponent<Foo>();

            var bar = foo.gameObject.AddComponent<Bar>();
            var qux1 = foo.gameObject.AddComponent<Qux>();
            var qux2 = foo.gameObject.AddComponent<Qux>();

            PreInstall();

            Container.Bind<Qux>().FromComponentsSibling();
            Container.Bind<Bar>().FromComponentSibling();
            Container.Bind<IBar>().FromComponentSibling();

            PostInstall();

            Assert.IsEqual(foo.Bar, bar);
            Assert.IsEqual(foo.IBar, bar);
            Assert.IsEqual(foo.Qux[0], qux1);
            Assert.IsEqual(foo.Qux[1], qux2);

            // Should skip self
            Assert.IsEqual(foo.Qux[0].OtherQux, foo.Qux[1]);
            Assert.IsEqual(foo.Qux[1].OtherQux, foo.Qux[0]);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunTestMissingFailure()
        {
            new GameObject().AddComponent<Gorp>();

            PreInstall();

            Container.Bind<Bar>().FromComponentSibling();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator RunTestMissingSuccess()
        {
            var foo = new GameObject().AddComponent<Foo>();
            foo.gameObject.AddComponent<Bar>();

            PreInstall();

            Container.Bind<Qux>().FromComponentsSibling();
            Container.Bind<Bar>().FromComponentSibling();
            Container.Bind<IBar>().FromComponentSibling();

            PostInstall();

            Assert.That(foo.Qux.IsEmpty());
            yield break;
        }

        public class Qux : MonoBehaviour
        {
            [Inject]
            public Qux OtherQux;
        }

        public interface IBar
        {
        }

        public class Bar : MonoBehaviour, IBar
        {
        }

        public class Foo : MonoBehaviour
        {
            [Inject]
            public Bar Bar;

            [Inject]
            public IBar IBar;

            [Inject]
            public List<Qux> Qux;
        }

        public class Gorp : MonoBehaviour
        {
            [Inject]
            public Bar Bar;
        }
    }
}

#endif
