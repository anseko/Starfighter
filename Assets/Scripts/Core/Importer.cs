using System;
using System.Collections;
using System.IO;
using System.Linq;
using Net.Core;
using UnityEngine;

namespace Core
{
    public static class Importer
    {
        public static StateData ImportAsteroids(string filepath)
        {
            var file = File.ReadAllText(filepath);
            return JsonUtility.FromJson<StateData>(file);
            
        }

        public static void ExportAsteroids(string filepath = "./asteroids.json")
        {
            var asteroids = GameObject.FindGameObjectsWithTag(Constants.AsteroidTag)
                .Select(obj => new Asteroid(obj.name.Replace("(Clone)",""), obj.transform)).ToArray();
            var worldState = new StateData()
            {
                worldState = asteroids
            };
            var textToWrite = JsonUtility.ToJson(worldState, true);
            File.WriteAllText(filepath, textToWrite);
        }

        public static void ExportAsteroids(StateData asteroids, string filepath = "./asteroids.json")
        {
            var textToWrite = JsonUtility.ToJson(asteroids);
            File.WriteAllText(filepath, textToWrite);
        }

        public static IEnumerator AddAsteroidsOnScene(StateData asteroids)
        {
            foreach (var asteroid in asteroids.worldState)
            {
                var goToInstantiate = Resources.Load(Constants.PathToPrefabs + asteroid.name);
                var instance =
                    GameObject.Instantiate(goToInstantiate, asteroid.position, asteroid.rotation) as
                        GameObject;
                instance.name = asteroid.name + Constants.Separator + Guid.NewGuid();
                instance.tag = Constants.AsteroidTag;
                instance.SetActive(true);
                yield return instance;
            }
            Debug.unityLogger.Log($"{asteroids.worldState.Length} asteroids are added");
        }
    }
}