﻿#if UNITY_EDITOR

using System;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject.Tests.Bindings.FromSubContainerPrefabResource;

namespace Zenject.Tests.Bindings
{
    public class TestFromSubContainerPrefabResource : ZenjectIntegrationTestFixture
    {
        const string PathPrefix = "TestFromSubContainerPrefabResource/";
        const string FooResourcePath = PathPrefix + "FooSubContainer";

        void CommonInstall()
        {
            Container.Settings = new ZenjectSettings(ValidationErrorResponses.Throw);
        }

        [UnityTest]
        public IEnumerator TestTransientError()
        {
            PreInstall();
            CommonInstall();

            // Validation should detect that it doesn't exist
            Container.Bind<Foo>().FromSubContainerResolve().ByNewPrefabResource(PathPrefix + "asdfasdfas").AsTransient().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfSingle()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Foo>().FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfTransient()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Foo>().FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfCached()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Foo>().FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfSingleMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfCachedMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfTransientMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(2);
            FixtureUtil.AssertComponentCount<Foo>(2);
            FixtureUtil.AssertComponentCount<Bar>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteSingle()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteTransient()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve()
                .ByNewPrefabResource(FooResourcePath).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteCached()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteSingleMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(IFoo), typeof(Bar)).To(typeof(Foo), typeof(Bar))
                .FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteCachedMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfIdentifiersFails()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Gorp>().FromSubContainerResolve().ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfIdentifiers()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Gorp>().FromSubContainerResolve("gorp").ByNewPrefabResource(FooResourcePath).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }
    }
}

#endif
