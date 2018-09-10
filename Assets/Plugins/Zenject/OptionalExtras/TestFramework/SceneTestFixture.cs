#if UNITY_EDITOR

using System.Collections.Generic;
using Zenject.Internal;
using ModestTree;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using System.Collections;
using UnityEngine.TestTools;
using Assert = ModestTree.Assert;
using System.Linq;

// Ignore warning about using SceneManager.UnloadScene instead of SceneManager.UnloadSceneAsync
#pragma warning disable 618

namespace Zenject
{
    public abstract class SceneTestFixture
    {
        readonly List<DiContainer> _sceneContainers = new List<DiContainer>();

        bool _hasLoadedScene;
        DiContainer _sceneContainer;

        protected DiContainer SceneContainer
        {
            get { return _sceneContainer; }
        }

        protected IEnumerable<DiContainer> SceneContainers
        {
            get { return _sceneContainers; }
        }

        public IEnumerator LoadScene(string sceneName)
        {
            return LoadScenes(sceneName);
        }

        public IEnumerator LoadScenes(params string[] sceneNames)
        {
            Assert.That(!_hasLoadedScene, "Attempted to load scene twice!");
            _hasLoadedScene = true;

            // Clean up any leftovers from previous test
            ZenjectTestUtil.DestroyEverythingExceptTestRunner(false);

            Assert.That(SceneContainers.IsEmpty());

            for (int i = 0; i < sceneNames.Length; i++)
            {
                var sceneName = sceneNames[i];

                Assert.That(Application.CanStreamedLevelBeLoaded(sceneName),
                    "Cannot load scene '{0}' for test '{1}'.  The scenes used by SceneTestFixture derived classes must be added to the build settings for the test to work",
                    sceneName, GetType());

                Log.Info("Loading scene '{0}' for testing", sceneName);

                var loader = SceneManager.LoadSceneAsync(sceneName, i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);

                while (!loader.isDone)
                {
                    yield return null;
                }

                SceneContext sceneContext = null;

                if (ProjectContext.HasInstance)
                // ProjectContext might be null if scene does not have a scene context
                {
                    var scene = SceneManager.GetSceneByName(sceneName);

                    sceneContext = ProjectContext.Instance.Container.Resolve<SceneContextRegistry>()
                        .TryGetSceneContextForScene(scene);
                }

                _sceneContainers.Add(sceneContext.Container);
            }

            _sceneContainer = _sceneContainers.Where(x => x != null).Last();

            if (_sceneContainer != null)
            {
                _sceneContainer.Inject(this);
            }
        }

        [SetUp]
        public virtual void SetUp()
        {
            StaticContext.Clear();
            SetMemberDefaults();
        }

        void SetMemberDefaults()
        {
            _hasLoadedScene = false;
            _sceneContainer = null;
            _sceneContainers.Clear();
        }

        [TearDown]
        public virtual void Teardown()
        {
            ZenjectTestUtil.DestroyEverythingExceptTestRunner(true);
            StaticContext.Clear();
            SetMemberDefaults();
        }
    }
}

#endif
