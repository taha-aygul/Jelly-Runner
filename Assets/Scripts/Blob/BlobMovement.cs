using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobMovement : MonoBehaviour
{
    Rigidbody blobRb;
    Animator animator;
    float sideMovementSpeed;
    float movementSpeed;
    public float currentSideMovementSpeed;
    float currentMovementSpeed;
    BlobStateController blobController;
    public bool inControl = true;
    void Start()
    {
        blobController = BlobStateController.Instance;
        sideMovementSpeed = blobController.sideMovementSpeed;
        movementSpeed = blobController.movementSpeed;
        currentMovementSpeed = movementSpeed;
        currentSideMovementSpeed = sideMovementSpeed;
        blobRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void MoveForward()
    {
        animator.SetBool("isRunning", true);
        blobRb.velocity = new Vector3(blobRb.velocity.x, blobRb.velocity.y, currentMovementSpeed * Time.fixedDeltaTime);
    }
    public void MoveSideways(float inputDirection, Vector2 sideDistance)
    {
        // inControl variable affected by wind for small blobs and finishline for big blob
        if (!inControl)
            return;

        // Go controls edge situations 
        int go = 1;

        // If blob on the edges  new velocity of blob will be 0
        if (transform.position.x <= sideDistance.x && inputDirection < 0)
            go = 0;

        if (transform.position.x >= sideDistance.y && inputDirection > 0)
            go = 0;

        // Assigning the velocity 

        Vector3 newVelocity = new(go * inputDirection * sideMovementSpeed, blobRb.velocity.y, blobRb.velocity.z);
        blobRb.velocity = newVelocity;


    }
    public void StopMovement()
    {
        currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, 0, 0.5f);
        currentSideMovementSpeed = Mathf.Lerp(currentSideMovementSpeed, 0, 0.5f);
    }
    public void StopSideway()
    {
        currentSideMovementSpeed = 0;
    }
    public void ContinueMovement()
    {
        currentMovementSpeed = movementSpeed;
        currentSideMovementSpeed = sideMovementSpeed;
    }


}
