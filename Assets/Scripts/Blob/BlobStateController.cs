using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class BlobStateController : MonoBehaviour
{
    BlobInputHandler _blobInput;
    GameManager _gameManager;

    // Pools for blob Spawn
    [Header("Blob Pools")]

    [SerializeField] private Transform bigBlobPool;
    [SerializeField] private Transform smallBlobPool;

    [Space(5)]
    [Header("Blob Properties")]

    public float movementSpeed = 5f;
    public float sideMovementSpeed = 1;
    [SerializeField] private int blobHealth = 3;
    [SerializeField] private float blobScaleMultiplier = 1;
    [Range(0, 1)] [SerializeField] float stateChangeTime = 0.2f;
    [SerializeField] float spawnRadius = 2;

    [Space(5)]
    [Header("Blob Forces")]

    public float jumpForce = 10;
    public float slideForce = 20;
    public float windForce = 10;

    [Header("Ground Properties")]

    [SerializeField] private Vector2 groundSideDistanceMaxMin;

    bool onStateChange = false;     // Boolean for not changing state again during a state change
    public bool FinishLineTriggered { get; private set; } = false;  // Boolean checks is level on the finish line

    public BlobState blobState;
    public enum BlobState
    {
        Big,
        Small
    }

    public static BlobStateController Instance;
    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Awake()
    {
        MakeSingleton();
    }


    void Start()
    {
        _gameManager = GameManager.Instance;
        _blobInput = BlobInputHandler.Instance;
        blobState = BlobState.Big;
    }

    void Update()
    {
        if (!_gameManager.IsGamePlaying || transform.childCount == 0)
            return;

        if (FinishLineTriggered && transform.GetChild(0).localScale.x < blobScaleMultiplier)
        {
            _gameManager.LevelSuccesful(0);
            Destroy(transform.GetChild(0).gameObject);
        }

    }

    private void FixedUpdate()      // Sadece 1 kez ele geçirince veya þekil deðiþtirince hýzý verebilirsin ve sað sol için Drag yapýldýðýnda fonksiyonu çaðýrabilirsin
    {
        if (!_gameManager.IsGamePlaying || transform.childCount == 0)
            return;
        MoveBlobs();
    }
    public void ChangeForm()
    {
        // Game not started or already changing state
        if (!_gameManager.IsGamePlaying || onStateChange)
            return;

        // End of level if state is big return
        if (FinishLineTriggered && blobState == BlobState.Big)
            return;

        // If health is too small so blob cannot change form to big 
        if (blobHealth <= 1 && blobState == BlobState.Small && !FinishLineTriggered)
            return;

        if (blobState == BlobState.Big)
        {
            StartCoroutine(nameof(SpawnSmallBlobs));
            blobState = BlobState.Small;

        }
        else if (blobState == BlobState.Small)
        {
            StartCoroutine(nameof(SpawnBigBlob));
            blobState = BlobState.Big;
        }
        // ...Any other states
    }

    IEnumerator SpawnBigBlob()

    {
        onStateChange = true;

        Vector3 spawnPosition = GetAverageBlobPosition();

        float rayMesafe = 10f;
        if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, rayMesafe))
        {
            spawnPosition = hit.point + new Vector3(0f, 1f, 0f); // Spawn a lil bit up
        }

        // Fill Small Blob array
        GameObject[] smallBlobs = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            smallBlobs[i] = transform.GetChild(i).gameObject;
        }


        // Small Blobs goes to the average position
        foreach (var smallBlob in smallBlobs)
        {
            smallBlob.transform.DOMoveX(spawnPosition.x, stateChangeTime);
            smallBlob.transform.DOMoveZ(spawnPosition.z, stateChangeTime);  // This causes slowdown when state changing
        }

        // Waiting until state changes
        yield return new WaitForSeconds(stateChangeTime);

        // Again getting spawn position 
        spawnPosition = GetAverageBlobPosition();

        // Recycling Small Blobs
        foreach (var smallBlob in smallBlobs)
        {
            smallBlob.transform.parent = smallBlobPool;
            smallBlob.SetActive(false);
        }

        // Spawning Big Blob
        GameObject Blob = bigBlobPool.GetChild(0).gameObject;
        Blob.SetActive(true);
        Blob.transform.localScale = Vector3.one;
        Blob.transform.parent = transform;
        Blob.transform.position = spawnPosition;


        if (!FinishLineTriggered)
        {
            Blob.transform.DOScale(blobScaleMultiplier * blobHealth, 1f);
        }
        else
        {
            // If reached the finish position this part works
            var sequence = DOTween.Sequence();
            sequence.Append(Blob.transform.DOScale(blobScaleMultiplier * blobHealth, 0.2f));
            sequence.Append(Blob.transform.DOScale(0, blobHealth * 2));
            sequence.Play();
            Blob.GetComponent<BigBlobMovement>().ActivateTrail();
            Blob.GetComponent<BlobMovement>().inControl = true;
        }

        onStateChange = false;
        // End of state change
    }


    IEnumerator SpawnSmallBlobs()
    {
        onStateChange = true;

        // Pool process
        GameObject bigBlob = transform.GetChild(0).gameObject;

        // Getting Big Blob smaller
        bigBlob.transform.DOScale(blobScaleMultiplier, stateChangeTime);
        
        yield return new WaitForSeconds(stateChangeTime);

        Vector3 spawnPosition = GetAverageBlobPosition();
        Vector3 spawnPositionTemp = GetAverageBlobPosition();
        // Recycling Big Blob
        bigBlob.transform.parent = bigBlobPool;
        bigBlob.SetActive(false);
       

        // Spawning Small Blobs
        for (int i = 0; i < blobHealth; i++)
        {
            // Blob must not get a random position if blob count (blobHealth) is greater than 1
            if (blobHealth > 1)
            {
                Vector3 randomUnit = Random.onUnitSphere;
                // Spawn position must be above the ground,
                if (randomUnit.y < 0)
                {
                    randomUnit.y = -randomUnit.y;
                }
                spawnPosition += randomUnit * spawnRadius;
            }

            spawnPosition.x = Mathf.Clamp(spawnPosition.x, groundSideDistanceMaxMin.x, groundSideDistanceMaxMin.y);

            // Pool process
            GameObject Blob = smallBlobPool.GetChild(0).gameObject;

            // Calculating optimal spawn position 
            float rayDist = 3f;
            if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, rayDist))
            {
                spawnPosition = hit.point + new Vector3(0f, Blob.transform.localScale.y / 2, 0f);
            }
            else
            {
                spawnPosition = new Vector3(spawnPosition.x, spawnPositionTemp.y, spawnPosition.z);
            }

            // Spawning
            Blob.SetActive(true);
            Blob.transform.position = spawnPosition;
            Blob.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Blob.transform.parent = transform;
        }

        onStateChange = false;
        // End of state change

    }

    void MoveBlobs()
    {
        // Moving each blob
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Blob = transform.GetChild(i);
            Blob.GetComponent<BlobMovement>().MoveForward();
        }
    }

    public void StopSidewayMovementBlobs(bool choice)
    {
        // Moving each blob
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Blob = transform.GetChild(i);
            if (choice)
            {
                Blob.GetComponent<BlobMovement>().StopSideway();
            }
            else
            {
                Blob.GetComponent<BlobMovement>().ContinueMovement();
            }
        }
    }
    public void MoveBlobsSideways(float direction)
    {
        // Moving each blob
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Blob = transform.GetChild(i);
            Blob.GetComponent<BlobMovement>().MoveSideways(direction, groundSideDistanceMaxMin);
        }
    }

    public void RecycleSmallBlobObject(GameObject recycle)
    {
        recycle.transform.parent = smallBlobPool;
        recycle.SetActive(false);
        GetDamage(1);
    }
    public void GetDamage(int damageAmount)
    {
        blobHealth -= damageAmount;

        // All blobs died
        if (blobHealth <= 0)
        {
            _gameManager.RestartGame();
        }
        else if (blobHealth == 1)
        {
            //If blob health is 1 the blobState cannot be Big
            ChangeForm();
        }
        if (blobState == BlobState.Big)
        {
            transform.GetChild(0).DOScale(blobScaleMultiplier * blobHealth, 1f);
        }
    }


    public void Grow(Transform gameObjectToGrow)
    {
        blobHealth++;

        if (blobState == BlobState.Big)
        {
            transform.GetChild(0).DOScale(blobScaleMultiplier * blobHealth, 1f);
            gameObjectToGrow.parent = smallBlobPool;
            gameObjectToGrow.gameObject.SetActive(false);
        }
        else if (blobState == BlobState.Small)
        {
            gameObjectToGrow.parent = transform;
            gameObjectToGrow.GetComponent<Rigidbody>().useGravity = true;
            gameObjectToGrow.GetComponent<Rigidbody>().isKinematic = false;
        }
    }


    public Vector3 GetAverageBlobPosition()
    {

        Transform[] BlobTransforms = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            BlobTransforms[i] = transform.GetChild(i);
        }

        Vector3 averageBlobPosition = Vector3.zero;

        foreach (Transform BlobTransform in BlobTransforms)
        {
            averageBlobPosition += BlobTransform.position;
        }

        if (BlobTransforms.Length > 0)
        {
            averageBlobPosition /= BlobTransforms.Length;
        }
        return averageBlobPosition;


    }

    public void EndGameTrigger()
    {
        FinishLineTriggered = true;
        stateChangeTime = 0.1f;

        // Changing state to big
        if (blobState == BlobState.Big)
        {
            Transform Blob = transform.GetChild(0);
            var sequence = DOTween.Sequence();
            sequence.Append(Blob.DOScale(blobScaleMultiplier * blobHealth, stateChangeTime));
            sequence.Append(Blob.DOScale(0, blobHealth * 2));
            sequence.Play();
            Blob.GetComponent<BigBlobMovement>().ActivateTrail();
            Blob.GetComponent<BlobMovement>().inControl = true;
        }
        else
        {
            ChangeForm();
        }
    }

    public bool IsInBigState()
    {
        return blobState == BlobState.Big;
    }
}


