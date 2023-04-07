using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public float jumpSpeed = 4.5f;
    public float fallSpeed = 1.45f; // Used to make the jump feel less "floaty"
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private Rigidbody rb;
    private int count;
    private int jumps_left;
    private float movementX;
    private float movementY;
    private Vector3 startingPosition;
    private bool onGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        jumps_left = 2;

        SetCountText();
        winTextObject.SetActive(false);
        startingPosition = transform.position;
    }
    
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 12)
        {
            winTextObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumps_left > 0)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);
            jumps_left--;
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        
        if (onGround)
        {  
            rb.AddForce(movement * speed);
        }
        else
        {
            rb.AddForce(movement * speed/4);
        }

        // Check if the player is falling (from jumping)
        // If so, increase the rate at which they fall 
        if (rb.velocity.y < 0.0f)
        {
            // Vector3.up is equivalent to 'new Vector3(0, 1, 0)
            rb.velocity += Vector3.up * Physics.gravity.y * fallSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            
            SetCountText();
        }
        else if (other.gameObject.CompareTag("OutOfBounds"))
        {
            // If the player goes out of bounds, simply reset them back to 
            // the starting position, and remove any velocity built up.
            transform.position = startingPosition;
            rb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            jumps_left = 2;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}
