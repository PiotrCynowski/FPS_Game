using UnityEngine;

namespace Enemies
{
    public class EnemyTypeSoldier : Enemy
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float wanderDistance = 10f;
        [SerializeField] private float avoidDistance = 2f;
        [SerializeField] private LayerMask obstacleLayer;

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                targetPosition = GetRandomDestination();
            }

            Vector3 direction = (targetPosition - transform.position).normalized;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, avoidDistance, obstacleLayer))
            {
                direction = Vector3.Reflect(direction, hit.normal);
            }

            transform.position += direction * speed * Time.fixedDeltaTime;
        }

        private Vector3 GetRandomDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderDistance;
            randomDirection += startArenaPos;
            randomDirection.y = transform.position.y;
            return randomDirection;
        }
    }
}