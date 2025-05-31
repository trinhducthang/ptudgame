using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    
    AudioSource jumpSound;
    private new Camera camera;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);
    public bool falling => velocity.y < 0f && !grounded;

    //velocity: V?n t?c c?a nh�n v?t, ???c ?i?u ch?nh theo c? tr?c ngang (x) v� d?c (y).
    //grounded: Ki?m tra nh�n v?t c� ??ng tr�n m?t ??t hay kh�ng.
    //jumpForce: L?c nh?y, ???c t�nh to�n d?a tr�n chi?u cao nh?y t?i ?a v� th?i gian nh?y.
    //gravity: Tr?ng l?c, ???c t�nh to�n t? chi?u cao v� th?i gian nh?y.
    

    private void Awake() //g?i khi game object kh?i t?o, d�ng ?? g�n c�c th�nh ph?n c?n thi?t
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        jumpSound = GetComponent<AudioSource>();
        
        
    }

    private void OnEnable() //C?u h�nh l?i tr?ng th�i nh�n v?t khi game object ???c k�ch ho?t
    {
        rigidbody.isKinematic = false;
        collider.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void OnDisable() //T??ng t? nh? OnEnable nh?ng khi game object b? v� hi?u h�a.
    {
        rigidbody.isKinematic = true;
        collider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update()  //G?i m?i khung h�nh, x? l� logic di chuy?n ngang, ki?m tra tr?ng th�i nh�n v?t
    {
        HorizontalMovement();

        grounded = rigidbody.Raycast(Vector2.down);

        if (grounded) {
            GroundedMovement();
        }

        ApplyGravity();
    }

    private void FixedUpdate()  //X? l� v?t l�, c?p nh?t v? tr� nh�n v?t d?a tr�n v?n t?c trong m?i khung v?t l�
    {
        // move mario based on his velocity
        Vector2 position = rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        // clamp within the screen bounds
        Vector2 leftEdge = camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        rigidbody.MovePosition(position);
    }

    private void HorizontalMovement() //X? l� di chuy?n ngang v� l?t h??ng nh�n v?t

    {
        // accelerate / decelerate
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        // check if running into a wall
        if (rigidbody.Raycast(Vector2.right * velocity.x)) {
            velocity.x = 0f;
        }

        // flip sprite to face direction
        if (velocity.x > 0f) {
            transform.eulerAngles = Vector3.zero;
        } else if (velocity.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void GroundedMovement() //X? l� c�c h�nh ??ng khi nh�n v?t ??ng tr�n m?t ??t
    {
        // prevent gravity from infinitly building up
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        // perform jump
        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            jumping = true;
            jumpSound.Play();
            
        }
    }

    private void ApplyGravity()  //�p d?ng tr?ng l?c ?? nh�n v?t r?i t? nhi�n
    {
        // check if falling
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        // apply gravity and terminal velocity
        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)  //X? l� khi nh�n v?t va ch?m v?i c�c ??i t??ng kh�c
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // bounce off enemy head
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                velocity.y = jumpForce / 2f;
                jumping = true;
            }
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            // stop vertical movement if mario bonks his head
            if (transform.DotTest(collision.transform, Vector2.up)) {
                velocity.y = 0f;
            }
        }
    }

}
