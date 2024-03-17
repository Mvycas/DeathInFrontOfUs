using UnityEngine;

namespace MovementSystem
{
    public class CameraController : MonoBehaviour
    {
    
        // This code is taken from "roll a ball" tutorials.
        // I mean it fits here, because I want same camera following behaviour
        // I will use perspective camera angled at 35 degrees. 
    
        public GameObject player;

        private Vector3 offset;
    
        // Start is called before the first frame update
        void Start()
        {
            offset = transform.position - player.transform.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = player.transform.position + offset;
        }
    }
}
