using System.Collections.Generic;
using UnityEngine;

namespace Crate
{
    public class CrateManager : MonoBehaviour
    {
        public static CrateManager Instance;
        public int numVials = 14; // No. of crates that has vials
        private List<crate> allCrates = new List<crate>();

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            allCrates.AddRange(FindObjectsOfType<crate>());
        
            Shuffle(allCrates);
        
            for (int i = 0; i < numVials; i++)
            {
                allCrates[i].containAntiViral = true; 
                Debug.Log($"Crate {allCrates[i].name} has been set to contain a vial."); 
            }
        }

        // fisher shuffle 
        private static void Shuffle<T>(List<T> list) 
        {
            System.Random random = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    
        public crate GetNearestUnopenedCrate(Vector3 playerPosition)
        {
            crate nearestCrate = null;
            float shortestDistance = Mathf.Infinity;

            foreach (crate currentCrate in allCrates)
            {
                if (!currentCrate.wasSearched) 
                {
                    float distanceToCrate = Vector3.Distance(playerPosition, currentCrate.transform.position);
                    if (distanceToCrate < shortestDistance)
                    {
                        shortestDistance = distanceToCrate;
                        nearestCrate = currentCrate;
                    }
                }
            }

            return nearestCrate;
        }
    }
}