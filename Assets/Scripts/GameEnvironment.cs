using System.Collections.Generic;
using UnityEngine;

namespace ObjectPoolingSystem
{
    public sealed class GameEnvironment
    {
        private static GameEnvironment instance;
        private List<GameObject> waypoints = new List<GameObject>();
        public List<GameObject> Waypoints
        {
            get { return waypoints; }
        }

        public static GameEnvironment Singleton
        {
            get
            {
                if (instance != null) return instance;
                instance = new GameEnvironment();
                instance.Waypoints.AddRange(
                    GameObject.FindGameObjectsWithTag("Waypoint"));
                return instance;
                }
            }
        }
    }
