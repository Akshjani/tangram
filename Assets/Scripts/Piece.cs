using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float dragHeight = 2f;
    [SerializeField] private float groundHeight = 0.1f;

    [Header("Rotation settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float rotationStep = 45f;

    [Header("Snap settings")]
    [SerializeField] private Target myTarget; // Reference specific target for this piece
    [SerializeField] private float angleSnapThreshold = 15f; //  rotation needs
    [SerializeField] private bool snapOnCollision = true; // Auto-snap correct target

    [Header("Visual settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color snapColor = Color.green;
    [SerializeField] private float hoverBrightness = 1.2f;

    private Camera mainCamera;
    private bool isDragging = false;
    private bool isRotating = false;
    private bool isHovering = false;
    private Vector3 dragOffset;
    private float currentRotation = 0f;
    private bool isSnapped = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Renderer pieceRenderer;
    private Material pieceMaterial;
    private Color originalColor;

    private bool isCollidingWithTarget = false;

    public static event System.Action<Piece> OnPieceSnapped;


    void Start()
    {
        mainCamera = Camera.main;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        currentRotation = transform.eulerAngles.y;

        pieceRenderer = GetComponent<Renderer>();
        if (pieceRenderer != null)
        {
            pieceMaterial = pieceRenderer.material;
            originalColor = pieceMaterial.color;
        }
    }

    void Update()
    {
        if (isSnapped) return;

        HandleInput();
        HandleHover();
    }

    void HandleHover()
    {
        if (isDragging || isRotating) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform == transform)
            {
                if (!isHovering)
                {
                    isHovering = true;
                    OnHoverEnter();
                }
            }
            else
            {
                if (isHovering)
                {
                    isHovering = false;
                    OnHoverExit();
                }
            }
        }
        else
        {
            if (isHovering)
            {
                isHovering = false;
                OnHoverExit();
            }
        }
    }

    void OnHoverEnter()
    {
        if (pieceMaterial != null && !isSnapped)
        {
            pieceMaterial.color = originalColor * hoverBrightness;
        }
    }

    void OnHoverExit()
    {
        if (pieceMaterial != null && !isSnapped)
        {
            pieceMaterial.color = originalColor;
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform == transform)
                {
                    StartDragging(hit.point);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                StopDragging();
            }
        }

        if (isDragging)
        {
            DragPiece();
        }

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform == transform)
                {
                    isRotating = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            if (isRotating)
            {
                isRotating = false;
                CheckSnapConditions();
            }
        }

        if (isRotating)
        {
            RotatePiece();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isHovering)
            {
                RotateByStep();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHovering)
            {
                RotateByStep(-1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isHovering)
            {
                ResetToInitialPosition();
            }
        }
    }

    void StartDragging(Vector3 hitPoint)
    {
        AudioManager.Instance?.PlayClick();
        isDragging = true;

        Vector3 piecePos = transform.position;
        piecePos.y = dragHeight;

        Vector3 mousePos = GetMouseWorldPosition(dragHeight);
        dragOffset = piecePos - mousePos;

        Vector3 newPos = transform.position;
        newPos.y = dragHeight;
        transform.position = newPos;
    }

    void StopDragging()
    {
        isDragging = false;

        Vector3 newPos = transform.position;
        newPos.y = groundHeight;
        transform.position = newPos;

        CheckSnapConditions();
    }

    void DragPiece()
    {
        Vector3 mousePos = GetMouseWorldPosition(dragHeight);
        Vector3 newPosition = mousePos + dragOffset;
        newPosition.y = dragHeight;
        transform.position = newPosition;
    }

    void RotatePiece()
    {
        float mouseX = Input.GetAxis("Mouse X");
        currentRotation += mouseX * rotationSpeed * Time.deltaTime;

        if (currentRotation >= 360f) currentRotation -= 360f;
        if (currentRotation < 0f) currentRotation += 360f;

        transform.rotation = Quaternion.Euler(0, currentRotation, 0);
    }

    void RotateByStep(int direction = 1)
    {
        currentRotation += rotationStep * direction;

        if (currentRotation >= 360f) currentRotation -= 360f;
        if (currentRotation < 0f) currentRotation += 360f;

        transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        AudioManager.Instance?.PlayClick();
        CheckSnapConditions();

    }

    Vector3 GetMouseWorldPosition(float height)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, height, 0));
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }

    // Called when piece collides with target
    void OnTriggerEnter(Collider other)
    {
        Target target = other.GetComponent<Target>();
        if (target != null && target == myTarget)
        {
            isCollidingWithTarget = true;
            Debug.Log($"{gameObject.name} entered target area");

            if (snapOnCollision && !isDragging && !isRotating)
            {
                CheckSnapConditions();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        Target target = other.GetComponent<Target>();
        if (target != null && target == myTarget)
        {
            isCollidingWithTarget = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Target target = other.GetComponent<Target>();
        if (target != null && target == myTarget)
        {
            isCollidingWithTarget = false;
            Debug.Log($"{gameObject.name} left target area");
        }
    }

    void CheckSnapConditions()
    {
        if (myTarget == null || !isCollidingWithTarget) return;

        // Check if rotation is correct
        float angleDifference = Quaternion.Angle(transform.rotation, myTarget.transform.rotation);

        Debug.Log($"{gameObject.name} - Colliding: YES, Angle diff: {angleDifference:F2}°");

        if (angleDifference < angleSnapThreshold)
        {
            SnapToTarget();
        }
        else
        {
            AudioManager.Instance?.PlayBump();
            Debug.Log($"{gameObject.name} - Rotation not correct. Needs {angleSnapThreshold}°, currently {angleDifference:F2}°");
        }
    }

    void SnapToTarget()
    {
        if (myTarget == null) return;

        // Snap to exact position and rotation
        transform.position = myTarget.transform.position;
        transform.rotation = myTarget.transform.rotation;
        currentRotation = myTarget.transform.eulerAngles.y;
        isSnapped = true;

        // Visual feedback
        if (pieceMaterial != null)
        {
            //pieceMaterial.color = snapColor;
        }

        Debug.Log($"{gameObject.name} SNAPPED CORRECTLY! ✓✓✓");

        // Disable collider to prevent further collision detection
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        OnPieceSnapped?.Invoke(this);
        AudioManager.Instance?.PlayPop();

    }

    public void ResetPiece()
    {
        isSnapped = false;
        isDragging = false;
        isRotating = false;
        isCollidingWithTarget = false;

        // enable collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        if (pieceMaterial != null)
        {
            pieceMaterial.color = originalColor;
        }
    }

    public void ResetToInitialPosition()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        currentRotation = initialRotation.eulerAngles.y;

        if (pieceMaterial != null && !isSnapped)
        {
            pieceMaterial.color = originalColor;
        }
    }

    public bool IsSnapped()
    {
        return isSnapped;
    }
}
