using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class EnemyTypeTank : Enemy
    {
        public override int MaxHealth => 500;

        [SerializeField] private float speed = 2f;
        [SerializeField] private float wanderDistance = 5f;
        [SerializeField] private float waitTime = 2f;

        public override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(Wander());
        }


        private void OnDisable()
        {
            StopAllCoroutines();
        }


        private IEnumerator Wander()
        {
            targetPosition = transform.position;

            while (true)
            {
                targetPosition = GetRandomDestination();
                targetPosition.y = transform.position.y;

                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                    yield return null;
                }

                yield return new WaitForSeconds(waitTime);
            }
        }

        private Vector3 GetRandomDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderDistance;
            randomDirection += startArenaPos;
            return randomDirection;
        }
    }
}