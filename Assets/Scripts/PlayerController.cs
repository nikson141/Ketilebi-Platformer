using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private float _horizontalInput;
    private bool _isFacingRight;
    private bool _isJumping;
    private int _jumpCount; // Track the number of jumps

    private bool _isWallSliding;
    private float _wallSlidingSpeed = 3f;

    private bool _isWallJumping;
    private float _wallJumpingDir;
    private float _wallJumpingTime = 0.2f;
    private float _wallJumpingCounter;
    private float _wallJumpingDuration = 0.4f;
    private Vector2 _wallJumpingPower = new Vector2(8f, 16f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        _jumpCount = 0; // Initialize jump count
    }

    private void Update()
    {
        Movement();
        JumpPlayer();
        WallSlide();
        WallJump();

        if (!_isWallJumping)
        {
            FlipPlayer();
        }
    }

    public void Movement()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(_horizontalInput * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(_horizontalInput));
    }

    public void FlipPlayer()
    {
        if (_isFacingRight && _horizontalInput > 0f || !_isFacingRight && _horizontalInput < 0)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    public void JumpPlayer()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!_isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity before jumping
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                _isJumping = true;
                _jumpCount++;
            }
            else if (_jumpCount < 2) // Allow double jump
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity before jumping
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                _jumpCount++;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isJumping = false;
            _jumpCount = 0; // Reset jump count when grounded
        }
    }

    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (isWalled() && !_isJumping && rb.velocity.y < 0)
        {
            _isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -_wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            _isWallSliding = false;
        }
    }


    private void WallJump()
    {
        if (_isWallSliding)
        {
            _isWallJumping = false;
            _wallJumpingDir = -transform.localScale.x;
            _wallJumpingCounter = _wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            _wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && _wallJumpingCounter > 0f)
        {
            _isWallJumping = true;


            float adjustedJumpForce = jumpForce * 0.75f;
            rb.velocity = new Vector2(_wallJumpingDir * _wallJumpingPower.x, adjustedJumpForce);
            _wallJumpingCounter = 0f;

            if (transform.localScale.x != _wallJumpingDir)
            {
                _isFacingRight = !_isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), _wallJumpingDuration);
            Debug.Log("Player Jumping On Wall");
        }
    }


    private void StopWallJumping()
    {
        _isWallJumping = false;
    }
}