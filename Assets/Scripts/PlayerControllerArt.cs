using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerControllerArt : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float movementSpeed;
    [SerializeField] private int acceletarion = 50;
    [SerializeField] private int deceleration = 50;

    [Header("Ground Parameters")]
    [SerializeField, Range(0, 1)] private float MinGroundNormalY = .65f;
    [SerializeField] private Transform raycastPoint;
    [SerializeField] private float stepSmooth = .1f;

    [Header("Jumping")] 
    [SerializeField] private float jumpimgForce;
    [SerializeField] private float additionalGravityForce;
    [SerializeField] private bool sealInput;

    [Header("Debug unview later")]
    [SerializeField] private float input;
    [SerializeField] private bool onGround;
    [SerializeField] private bool onSlope;

    [SerializeField] private bool isJumping = true;
    private Vector3 inputDirection;
    private Rigidbody playerRb;
    private CapsuleCollider playerCollider;
    private Vector3 groundNormal;
    Animator animator;


    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        CheckGround();

        input = Input.GetAxisRaw("Horizontal");
        inputDirection = new Vector3(input, 0, 0);

        if (!Input.GetKeyDown(KeyCode.W) || isJumping) return;
        
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0, 0);
        playerRb.AddForce(Vector3.up * jumpimgForce, ForceMode.Impulse);
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(.1f);
        isJumping = true;
    }

    void FixedUpdate()
    {
        //CheckGround();
        CheckForwardDirection();
        AddGravity();
        MoveCharacter();
    }

    private void AddGravity()
    {
        if (!onGround && playerRb.velocity.y <= 0)
            playerRb.velocity += additionalGravityForce * Time.deltaTime * Physics.gravity;
    }

    private void MoveCharacter()
    {
        if (sealInput && isJumping) return;
        if (input == 0 || !onGround)
        {
            animator.SetBool("isWalk", false);
        }
        if (input != 0 && onGround)
        {
            animator.SetBool("isWalk", true);
        }
        var targetSpeed = input * movementSpeed;
        var deltaSpeed = targetSpeed - playerRb.velocity.x;
        var directionAlongGround = Vector3.ProjectOnPlane(Vector3.right, groundNormal);
        var accelRate = input == 0 ? deceleration : acceletarion;
        playerRb.AddForce(directionAlongGround * (accelRate * deltaSpeed), ForceMode.Force);
        transform.LookAt(transform.position + inputDirection);
    }

    private void CheckGround()
    {
        onGround = Physics.Raycast(transform.position, Vector3.down, out var collision, .1f);
        groundNormal = collision.normal;
        if (!onGround)
        {
            StartCoroutine(CoyoteTime());
            animator.SetBool("isJump", true);
        }

        if (onGround)
        {
            isJumping = false;
            animator.SetBool("isJump", false);
        }  
            
        onSlope = onGround && groundNormal.y != 1;
        playerRb.useGravity = !onSlope;
    }

    private void CheckForwardDirection()
    {
        var isHittingLower = Physics.Raycast(transform.position, transform.forward, out var collisionLower,
            playerCollider.radius);
        //no obstacle on lower raycast
        if (!isHittingLower) return;

        var isHittingUpper = Physics.Raycast(raycastPoint.position, transform.forward, out var collisionUpper,
            playerCollider.radius);
        //no obstacle on upper raysact
        if (!isHittingUpper)
        {
            //not on slope and lower obstacle = 90 degrees => lift character a bit. Im assume where is no ladders on slopes
            if (!onSlope && input != 0 && collisionLower.normal.y == 0)
                playerRb.position += new Vector3(0, stepSmooth, 0);
            return;
        }
        // upper obstacle is not walkable slope or wall
        if (Mathf.Abs(collisionUpper.normal.y) < MinGroundNormalY)
            input = 0;
    }
}