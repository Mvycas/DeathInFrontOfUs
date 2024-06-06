using Crate;
using UnityEngine;
using UnityEngine.SceneManagement;  

namespace NavigationSystem
{
    public class ArrowIndicator : MonoBehaviour
    {
        private Transform player;       
        private Transform targetCrate;  

        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;  
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;  
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindPlayer();
        }

        void Start()
        {
            FindPlayer();
        }

        void Update()
        {
            if (player == null)
            {
                return;  
            }

            targetCrate = CrateManager.Instance.GetNearestUnopenedCrate(player.position).transform;
           
            if (targetCrate != null)
            {
                Vector3 targetDirection = targetCrate.position - player.position;
                targetDirection.y = 0f;  

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                float targetYRotation = targetRotation.eulerAngles.y;

                transform.rotation = Quaternion.Euler(90f, targetYRotation, 0f);
            }
        }

        private void FindPlayer()
        {
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject != null)
            {
                player = playerGameObject.transform;
            }
        }
    }
}
