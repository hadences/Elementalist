using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementComponent : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer playerSprite;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;

    InputAction moveAction;

    private Vector2 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find the references to the input actions
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        // read the move action val
        moveDirection = moveAction.ReadValue<Vector2>();
        
        // update sprite
        if(moveDirection.x > 0){
            playerSprite.flipX = false;
        }else if(moveDirection.x < 0){
            playerSprite.flipX = true;
        }
        
    }

    void FixedUpdate(){
        move(moveDirection);
    }

    /* move the player given the move input direction */
    private void move(Vector2 direction){
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }
}
