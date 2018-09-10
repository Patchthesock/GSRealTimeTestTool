#if UNITY_EDITOR

using System;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    public class TestFromComponentInChildren : ZenjectIntegrationTestFixture
    {
        Root _root;
        Child _child1;
        Child _child2;
        Grandchild _grandchild;

        public void Setup1()
        {
            _root = new GameObject("root").AddComponent<Root>();

            _child1 = new GameObject("child1").AddComponent<Child>();
            _child1.transform.SetParent(_root.transform);

            _child2 = new GameObject("child2").AddComponent<Child>();
            _child2.transform.SetParent(_root.transform);

            _grandchild = new GameObject("grandchild").AddComponent<Grandchild>();
            _grandchild.transform.SetParent(_child1.transform);
        }

        [UnityTest]
        public IEnumerator RunMatchSingleChild()
        {
            Setup1();
            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();
            Container.Bind<Child>().FromComponentInChildren();

            PostInstall();

            Assert.IsEqual(_root.Grandchild, _grandchild);
            Assert.IsEqual(_root.Childs.Count, 1);
            Assert.IsEqual(_root.Childs[0], _child1);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchAllChildren()
        {
            Setup1();
            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();
            Container.Bind<Child>().FromComponentsInChildren();

            PostInstall();

            Assert.IsEqual(_root.Grandchild, _grandchild);
            Assert.IsEqual(_root.Childs.Count, 2);
            Assert.IsEqual(_root.Childs[0], _child1);
            Assert.IsEqual(_root.Childs[1], _child2);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMissingChildrenFailure()
        {
            new GameObject("root").AddComponent<Root>();

            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMissingChildrenSuccess()
        {
            var root = new GameObject("root").AddComponent<Root>();

            var grandchild = new GameObject("grandchild").AddComponent<Grandchild>();
            grandchild.transform.SetParent(root.transform);

            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptional()
        {
            var root = new GameObject("root").AddComponent<RootWithOptional>();

            PreInstall();

            Container.Bind<Child>().FromComponentInChildren();

            PostInstall();

            Assert.IsNull(root.Child);

            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptional2()
        {
            var root = new GameObject("root").AddComponent<Root>();

            var grandChild = new GameObject("grandchild").AddComponent<Grandchild>();
            grandChild.transform.SetParent(root.transform, false);

            PreInstall();

            Container.Bind<Grandchild>().FromComponentsInChildren();
            Container.Bind<Child>().FromComponentInChildren();

            PostInstall();

            // The FromComponentInChildren call should match nothing when optional like in
            // list bindings
            Assert.That(root.Childs.IsEmpty());

            yield break;
        }

        public class Root : MonoBehaviour
        {
            [Inject]
            public Grandchild Grandchild;

            [Inject]
            public List<Child> Childs;
        }

        public class Child : MonoBehaviour
        {
        }

        public class Grandchild : MonoBehaviour
        {
        }

        public class RootWithOptional : MonoBehaviour
        {
            [InjectOptional]
            public Child Child;
        }
    }
}

#endif
