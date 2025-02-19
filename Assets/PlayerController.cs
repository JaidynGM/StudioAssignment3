using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private inputManager inputManager;
    [SerializeField] private float speed;

    private Rigidbody rb;

    private void Start()
    {
        inputManager.OnMove.AddListener(MovePlayer);
        rb = GetComponent<Rigidbody>();
    }

    private void MovePlayer(Vector2 direction)
    {
        Vector3 moveDirection = new(direction.x, 0f, direction.y);
        rb.AddForce(speed * moveDirection);
    }
}
