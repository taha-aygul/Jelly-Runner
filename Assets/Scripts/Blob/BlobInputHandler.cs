using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlobInputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    [Range(0, 2)][SerializeField] private float blobChangerTime;
    public float Direction { get; private set; }   // Direction of drag (-0.5f , 0.5f) 
    private Vector2 _touchPosition;                // Touch position         
    private BlobStateController _blobStateController;
    private bool _firstTouch;
    private GameManager _gameManager;
    private UIManager _UIManager;
    public static BlobInputHandler Instance;
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
        Direction = 0;
    }
    private void Start()
    {
        _blobStateController = BlobStateController.Instance;
        _gameManager = GameManager.Instance;
        _UIManager = UIManager.Instance;
    }

    public void OnDrag(PointerEventData eventData)
    {
        StopAllCoroutines();
        _blobStateController.StopSidewayMovementBlobs(false);

        if (!_firstTouch)
        {
            _firstTouch = true;
            _gameManager.StartGame();
            _UIManager.CloseTutorial();
        }

        float screenWidth = Screen.width;
        float normalizedDragDistance = (eventData.position.x - _touchPosition.x) / screenWidth;

        // Direction will be used for blob Movement on x axis
        Direction = normalizedDragDistance;

        _blobStateController.MoveBlobsSideways(Direction);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Blob State Changes

        if (GameManager.Instance.IsGameFinished)
        {
            LevelManager.Instance.NextLevel();
        }
        StopAllCoroutines();
        StartCoroutine(nameof(HoldCounter));

        _touchPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Dokunma sonunda
        StopAllCoroutines();
        Direction = 0f; // Sürükleme yönünü sýfýrla

        if (!_blobStateController.IsInBigState())
        {
            _blobStateController.ChangeForm();
        }
        _blobStateController.StopSidewayMovementBlobs(true);
    }
    IEnumerator HoldCounter()
    {
        yield return new WaitForSeconds(blobChangerTime);
        _blobStateController.ChangeForm();

    }





}
