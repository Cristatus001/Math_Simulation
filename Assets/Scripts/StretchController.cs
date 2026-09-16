using UnityEngine;

public class StretchController : MonoBehaviour
{
    public Transform topStretchHandle; // Reference to the top stretch collider (handle)
    public Transform bottomStretchHandle; // Reference to the bottom stretch collider (handle)
    public SpriteRenderer boundarySprite; // Reference to the boundary sprite for restricting stretching
    public float stretchSpeed = 0.5f; // Speed multiplier for controlling stretching

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
            // Check if the user tapped the top stretch handle
            if (topStretchHandle.GetComponent<Collider2D>().OverlapPoint(mousePosition))
            {
                isStretching = true;
                activeStretchHandle = topStretchHandle;
                initialMousePosition = mousePosition;
                initialScale = transform.localScale;
            }
            // Check if the user tapped the bottom stretch handle
            else if (bottomStretchHandle.GetComponent<Collider2D>().OverlapPoint(mousePosition))
            {
                isStretching = true;
                activeStretchHandle = bottomStretchHandle;
                initialMousePosition = mousePosition;
                initialScale = transform.localScale;
            }
        }

        // On mouse/touch hold
        if (Input.GetMouseButton(0) && isStretching)
        {
            Vector2 currentMousePosition = mousePosition;

            // Calculate the vertical stretching amount
            float verticalStretch = (currentMousePosition.y - initialMousePosition.y) * stretchSpeed;

            // Calculate the new scale while clamping it to the boundary
            float newHeight;
            if (activeStretchHandle == topStretchHandle)
            {
                newHeight = Mathf.Clamp(
                    initialScale.y + verticalStretch,
                    boundaryBounds.min.y - transform.position.y,
                    boundaryBounds.max.y - transform.position.y
                );
            }
            else if (activeStretchHandle == bottomStretchHandle)
            {
                newHeight = Mathf.Clamp(
                    initialScale.y - verticalStretch,
                    transform.position.y - boundaryBounds.max.y,
                    transform.position.y - boundaryBounds.min.y
                );
            }
            else
            {
                newHeight = initialScale.y; // Default to initial height if no valid handle
            }

            // Update the scale
            transform.localScale = new Vector3(initialScale.x, newHeight, initialScale.z);
        }

        // On mouse/touch release
        if (Input.GetMouseButtonUp(0))
        {
            isStretching = false;
            activeStretchHandle = null;
        }
    }
}
