using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 4.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private DynamicJoystick joystick;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        joystick = FindAnyObjectByType<DynamicJoystick>();
    }

    void FixedUpdate()
    {
        Vector2 input = joystick != null ? joystick.Direction : Vector2.zero;

        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);

        if (input.x != 0)
            sr.flipX = input.x < 0;
    }
}