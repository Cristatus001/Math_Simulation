using UnityEngine;

public class PlankController : MonoBehaviour
{
    public Transform topPivot; // Reference to the top collider (pivot point)
    public Transform bottomPivot; // Reference to the bottom collider (pivot point)
    public Collider2D centralCollider; // Reference to the central collider
    public Transform topStretchHandle; // Reference to the top stretch collider (handle)
    public Transform bottomStretchHandle; // Reference to the bottom stretch collider (handle)
    public SpriteRenderer boundarySprite; // Reference to the boundary sprite for restricting movement and stretching
    public float stretchSpeed = 0.5f; // Speed multiplier for controlling stretching
    public float val; // Rotation speed multiplier

    private Transform currentPivot = null; // Active pivot point for rotation
    private bool isDragging = false; // Whether the plank is being dragged
    private Vector2 offset; // Offset between the mouse position and the plank's position
    private bool isStretching = false; // Whether the plank is being stretched
    private Transform activeStretchHandle = null; // The active handle being used for stretching
    private Vector2 initialMousePosition; // The initial mouse position when stretching starts
    private Vector3 initialScale; // The initial scale of the plank
    private Bounds boundaryBounds; // The bounds of the boundary sprite

    void Start()
    {
        // Get the bounds of the boundary sprite
        if (boundarySprite != null)
        {
            boundaryBounds = boundarySprite.bounds;
        }
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // On mouse/touch press
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the user tapped the central collider for dragging
            if (centralCollider.OverlapPoint(mousePosition))
            {
                isDragging = true;
                offset = (Vector2)transform.position - mousePosition;
            }
            // Check if the user tapped the top pivot for rotation
            else if (topPivot.GetComponent<Collider2D>().OverlapPoint(mousePosition))
            {
                isDragging = false;
                currentPivot = bottomPivot;
                val = -1000;
            }
            // Check if the user tapped the bottom pivot for rotation
            else if (bottomPivot.GetComponent<Collider2D>().OverlapPoint(mousePosition))
            {
                isDragging = false;
                currentPivot = topPivot;
                val = 1000;
            }
            // Check if the user tapped the top stretch handle
            else if (topStretchHandle.GetComponent<Collider2D>().OverlapPoint(mousePosition))
            {
                isStretching = true;
                activeStretchHandle = topStretchHandle;
                initialMousePosition = mousePosition;
                initialScale = transform.localScale;
                currentPivot = bottomPivot;  // Switch pivot to the bottom when stretching from top
            }
            // Check if the user tapped the bottom stretch handle
            else if (bottomStretchHandle.GetComponent<Collider2D>().OverlapPoint(mousePosition))
            {
                isStretching = true;
                activeStretchHandle = bottomStretchHandle;
                initialMousePosition = mousePosition;
                initialScale = transform.localScale;
                currentPivot = topPivot;  // Switch pivot to the top when stretching from bottom
            }
        }

        // On mouse/touch hold
        if (Input.GetMouseButton(0))
        {
            if (isDragging)
            {
                // Dragging: Move the plank
                Vector2 targetPosition = mousePosition + offset;
                targetPosition = ClampPositionWithinBoundary(targetPosition);
                transform.position = targetPosition;
            }
            else if (isStretching)
            {
                // Stretching: Adjust the scale based on which handle is being used
                Vector2 currentMousePosition = mousePosition;
                float verticalStretch = (currentMousePosition.y - initialMousePosition.y) * stretchSpeed;

                if (activeStretchHandle == topStretchHandle)
                {
                    // Stretch only the top side when the top stretch handle is used
                    float newHeight = Mathf.Clamp(
                        initialScale.y + verticalStretch,
                        boundaryBounds.min.y - transform.position.y,
                        boundaryBounds.max.y - transform.position.y
                    );

                    transform.localScale = new Vector3(initialScale.x, newHeight, initialScale.z);
                }
                else if (activeStretchHandle == bottomStretchHandle)
                {
                    // Stretch only the bottom side when the bottom stretch handle is used
                    float newHeight = Mathf.Clamp(
                        initialScale.y - verticalStretch,
                        transform.position.y - boundaryBounds.max.y,
                        transform.position.y - boundaryBounds.min.y
                    );

                    transform.localScale = new Vector3(initialScale.x, newHeight, initialScale.z);
                }
            }
            else
            {
                // Rotating: Rotate around the current pivot
                float rotationInput = Input.GetAxis("Mouse X");
                transform.RotateAround(currentPivot.position, Vector3.forward, rotationInput * val * Time.deltaTime);

                // Clamp position after rotation to stay within the boundary
                Vector3 clampedPosition = ClampPositionWithinBoundary(transform.position);
                transform.position = clampedPosition;
            }
        }

        // On mouse/touch release
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            isStretching = false;
            activeStretchHandle = null;
            currentPivot = null; // Reset the pivot when releasing
        }
    } 

    // Clamp the position of the plank to stay within the boundary
    // Clamp the position of the plank to stay within the boundary
    private Vector2 ClampPositionWithinBoundary(Vector2 position)
    {
        // Calculate the bounds of the plank including its scale
        Vector3 plankSize = GetComponent<SpriteRenderer>().bounds.size;
        Vector3 halfPlankSize = plankSize / 2;

        // Calculate min and max positions to keep the entire plank within the boundary
        float minX = boundaryBounds.min.x + halfPlankSize.x;
        float maxX = boundaryBounds.max.x - halfPlankSize.x;
        float minY = boundaryBounds.min.y + halfPlankSize.y;
        float maxY = boundaryBounds.max.y - halfPlankSize.y;

        // Clamp the position to stay inside the boundary
        float clampedX = Mathf.Clamp(position.x, minX, maxX);
        float clampedY = Mathf.Clamp(position.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
}
