using UnityEngine;

public class MapMovingPlatform : MonoBehaviour, ICanParentPlayer
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float distance = 5f;
    [SerializeField] private TypeOfMovement movement;

    private float direction = 1f;   
    private Vector3 startPosition;
    private enum TypeOfMovement
    {
      Horizontal,
      Vertical,
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        switch (movement)
        {
            case TypeOfMovement.Horizontal:
                transform.position += Vector3.right * speed * direction * Time.fixedDeltaTime;
                if (Mathf.Abs(transform.position.x - startPosition.x) >= distance)
                {
                    direction *= -1f;
                }
                break;

            case TypeOfMovement.Vertical:
                transform.position += Vector3.up * speed * direction * Time.fixedDeltaTime;
                if (Mathf.Abs(transform.position.y - startPosition.y) >= distance)
                {
                    direction *= -1f;
                }
                break;

            default:
                Debug.LogError("Unknown TypeOfMovement: " + movement);
                break;
        }
    }
}