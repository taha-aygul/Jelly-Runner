using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBlobCollision : MonoBehaviour                  /// HER RB ISLEMINDE RIGIDBODY CEKIYOSUN YAPMA
{

    float _timeCounter;
    BlobStateController jelloController;
    BlobMovement blobMove;
    Rigidbody rb;
    Animator animator;
    [SerializeField] private GameObject damageEffect;

    private void Start()
    {
        animator = GetComponent<Animator>();
        blobMove = GetComponent<BlobMovement>();
        rb = GetComponent<Rigidbody>();
        jelloController = BlobStateController.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;

        if (other.CompareTag("SharpObstacle"))
        {
            DamageAnimation();
            jelloController.RecycleSmallBlobObject(gameObject);
        }
        else if (other.CompareTag("JumpZone"))
        {
            rb = GetComponent<Rigidbody>();
            Vector3 force = other.transform.up * jelloController.jumpForce;
            rb.AddForce(force, ForceMode.Impulse);
        }
        else if (other.CompareTag("Wall"))
        {
            DamageAnimation();
            jelloController.RecycleSmallBlobObject(gameObject);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("SlipperyCylinder"))
        {
            CapsuleCollider otherColl = collision.collider.GetComponent<CapsuleCollider>();
            otherColl.radius /= 2;
            _timeCounter += Time.deltaTime;

            // When bloba are sliding they cannot go faster
            if (_timeCounter > 1)
            {
                blobMove = GetComponent<BlobMovement>();
                blobMove.StopMovement();
            }

            // Calculate sliding power
            Vector3 slideDirection = new Vector3(collision.contacts[0].normal.x, 0, collision.contacts[0].normal.z).normalized;

            // Forces the power
            rb = GetComponent<Rigidbody>();
            rb.AddForce(slideDirection * jelloController.slideForce, ForceMode.Acceleration);
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("SlipperyCylinder"))
        {
            _timeCounter = 0;
            blobMove = GetComponent<BlobMovement>();
            blobMove.ContinueMovement();
        }

    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Finish") && !jelloController.FinishLineTriggered)
        {
            jelloController.EndGameTrigger();

        }
        else if (other.CompareTag("SmallBlob"))
        {
            other.tag = "Untagged";
            other.gameObject.layer = LayerMask.NameToLayer("Small Blob");
            other.GetComponent<Rigidbody>().isKinematic = false;
            other.isTrigger = false;
            jelloController.Grow(other.transform);
        }
        else if (other.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            ScoreManager.Instance.GainCoin(1);
        }
        else if (other.CompareTag("WindZone"))
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = new Vector3(0, 0, rb.velocity.z);
            Vector3 force = other.transform.up * jelloController.windForce;
            rb.AddForce(force, ForceMode.Impulse);
            blobMove.inControl = false;
        }
        else if (other.CompareTag("SharpObstacle"))
        {
            DamageAnimation();
            jelloController.RecycleSmallBlobObject(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            DamageAnimation();
            jelloController.RecycleSmallBlobObject(gameObject);
        }
        else if (other.CompareTag("Slides"))
        {
            animator.SetBool("isSliding", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WindZone"))
        {
            rb = GetComponent<Rigidbody>();
            blobMove.inControl = true;
        }
        else if (other.CompareTag("Slides"))
        {
            animator.SetBool("isSliding", false);
        }
    }
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void DamageAnimation()
    {

        Vector3 pos = transform.position + Vector3.up * transform.localScale.y / 8;
        GameObject effectGo = Instantiate(damageEffect, pos, Quaternion.identity);
        effectGo.transform.eulerAngles = new Vector3(90, 0, 0);
        /* Vector3 pos = transform.position - Vector3.up * transform.localScale.y / 2;
         Instantiate(damageEffect, pos, Quaternion.identity);
         RaycastHit hit;
         if (Physics.Raycast(transform.position, Vector3.down, out hit))
         {
             if (hit.collider.CompareTag("Ground"))
             {
                 GameObject effectGo = Instantiate(damageEffect, hit.point, Quaternion.identity);
                 effectGo.transform.eulerAngles = new Vector3(90, 0, 0);
             }
         }*/
    }
}
