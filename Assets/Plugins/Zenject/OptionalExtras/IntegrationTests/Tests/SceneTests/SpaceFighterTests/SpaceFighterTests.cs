#if UNITY_EDITOR

using System.Linq;
using ModestTree;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using System.Collections;
using UnityEngine.TestTools;
using Zenject.SpaceFighter;
using Assert = ModestTree.Assert;

namespace Zenject.SpaceFighter
{
    public class SpaceFighterTests : SceneTestFixture
    {
        const string SceneName = "SpaceFighter";

        [UnityTest]
        public IEnumerator TestEnemyStateChanges()
        {
            // Override settings to only spawn one enemy to test
            StaticContext.Container.BindInstance(
                new EnemySpawner.Settings()
                {
                    SpeedMin = 50,
                    SpeedMax = 50,
                    AccuracyMin = 1,
                    AccuracyMax = 1,
                    NumEnemiesIncreaseRate = 0,
                    NumEnemiesStartAmount = 1,
                });

            yield return LoadScene(SceneName);

            var enemy = SceneContainer.Resolve<EnemyRegistry>().Enemies.Single();

            // Should always start by chasing the player
            Assert.IsEqual(enemy.State, EnemyStates.Follow);

            // Wait a frame for AI logic to run
            yield return null;

            // Our player mock is always at position zero, so if we move the enemy there then the enemy
            // should immediately go into attack mode
            enemy.Position = Vector3.zero;

            // Wait a frame for AI logic to run
            yield return null;

            Assert.IsEqual(enemy.State, EnemyStates.Attack);

            enemy.Position = new Vector3(100, 100, 0);

            // Wait a frame for AI logic to run
            yield return null;

            // The enemy is very far away now, so it should return to searching for the player
            Assert.IsEqual(enemy.State, EnemyStates.Follow);
        }

        [UnityTest]
        public IEnumerator TestKillsPlayer()
        {
            // Override settings to only spawn one enemy to test
            StaticContext.Container.BindInstance(
                new EnemySpawner.Settings()
                {
                    SpeedMin = 50,
                    SpeedMax = 50,
                    AccuracyMin = 1,
                    AccuracyMax = 1,
                    NumEnemiesIncreaseRate = 0,
                    NumEnemiesStartAmount = 5,
                });

            yield return LoadScene(SceneName);

            var signalBus = SceneContainer.Resolve<SignalBus>();

            bool died = false;

            signalBus.Subscribe<PlayerDiedSignal>(() => died = true);

            var startTime = Time.realtimeSinceStartup;

            while (!died && Time.realtimeSinceStartup - startTime < 30.0f)
            {
                yield return null;
            }

            startTime = Time.realtimeSinceStartup;

            // Wait for death sequence to pass
            while (Time.realtimeSinceStartup - startTime < 2.0f)
            {
                yield return null;
            }

            Assert.That(died, "Enemy could not kill player in 30 seconds or less!");
        }
    }
}

#endif
