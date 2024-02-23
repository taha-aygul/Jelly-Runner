using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBlobCollision : MonoBehaviour
{
    BlobStateController _blobController;
    private float _timeCounter;
    Animator animator;
    [SerializeField] private GameObject damageEffect;
    private void Start()
    {
        animator = GetComponent<Animator>();
        _blobController = BlobStateController.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;

        if (other.CompareTag("ScoreMultipler"))
        {
            transform.localScale = Vector3.zero;
            int multiplier = other.GetComponent<MultiplerBoxController>().multipler;
            GameManager.Instance.LevelSuccesful(multiplier);
        }
        else if (other.CompareTag("SharpObstacle"))
        {
            DamageAnimation();
            _blobController.GetDamage(1);
        }
        else if (other.CompareTag("Wall"))
        {
            other.tag = "Untagged";
        }
        else if (other.CompareTag("JumpZone"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 force = other.transform.up * _blobController.jumpForce;//new(0, jumpForce, 0);
            rb.AddForce(force, ForceMode.Impulse);
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        Collider other = collision.collider;
        if (other.CompareTag("SharpObstacle"))
        {
            _timeCounter += Time.deltaTime;
            if (_timeCounter >= 1)
            {
                DamageAnimation();
                _blobController.GetDamage(1);
                _timeCounter = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish") && !_blobController.FinishLineTriggered)
        {
            _blobController.EndGameTrigger();
        }
        else if (other.CompareTag("SmallBlob"))
        {
            other.tag = "Untagged";
            other.gameObject.layer = LayerMask.NameToLayer("Small Blob");
            other.GetComponent<Rigidbody>().isKinematic = false;
            other.isTrigger = false;
            _blobController.Grow(other.transform);
        }
        else if (other.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            ScoreManager.Instance.GainCoin(1);
        }
        else if (other.CompareTag("JumpBeforeWall"))
        {
            animator.SetBool("isJumping", true);
        }
        else if (other.CompareTag("SharpObstacle"))
        {
            DamageAnimation();
            _blobController.GetDamage(1);
        }
        else if (other.CompareTag("Slides"))
        {
            animator.SetBool("isSliding", true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("JumpBeforeWall"))
        {
            animator.SetBool("isJumping", false);
        }
        else if (other.CompareTag("Slides"))
        {
            animator.SetBool("isSliding", false);
        }
    }
    public void DamageAnimation()
    {
        Vector3 pos = transform.position + Vector3.up * transform.localScale.y / 8;
        GameObject effectGo = Instantiate(damageEffect, pos, Quaternion.identity);
        effectGo.transform.eulerAngles = new Vector3(90, 0, 0);
      
    }
}
