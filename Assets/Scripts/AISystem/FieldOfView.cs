using System.Collections;
using UnityEngine;

namespace AISystem
{
    public class FieldOfView : MonoBehaviour
    {
        public float viewRadius;
        [Range(0, 360)] public float viewAngle;

        public LayerMask targetMask; 
        public LayerMask obstacleMask;

        private AIController aiController; 
        private bool playerVisibleLastCheck; 
        private Coroutine fovCoroutine;

        private void Awake()
        {
            aiController = GetComponent<AIController>();
        }

        private void OnEnable()
        {
            fovCoroutine = StartCoroutine(FindTargetsWithDelay(0.2f)); 
        }

        private void OnDisable()
        {
            if (fovCoroutine != null)
            {
                StopCoroutine(fovCoroutine); 
                fovCoroutine = null;
            }
        }

        IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        void FindVisibleTargets()
        {
            bool playerVisible = false;
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                    {
                        playerVisible = true;
                        aiController.SeePlayer(target.position);
                        break; 
                    }
                }
            }

            if (!playerVisible && playerVisibleLastCheck)
            {
                ClearVisibleTargets();
            }
            playerVisibleLastCheck = playerVisible;
        }
        public void ClearVisibleTargets()
        {
            aiController.LosePlayer();
        }
    }
}
