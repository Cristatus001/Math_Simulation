using UnityEngine;

public class ImageInteraction : MonoBehaviour
{
    public Vector3 targetPosition; // The target position the image will move to
    public float moveSpeed = 2f;   // Speed at which the image will move
    public float scaleSpeed = 1f; // Speed at which the image scales down
    private bool isMoving = false;
    private bool hasReachedTarget = false;

    private Vector3 targetScale = new Vector3(0.2f, 0.2f, 0.2f);

    void Update()
    {
        // If the image is set to move, move it towards the target position
        if (isMoving && !hasReachedTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Check if the image has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                hasReachedTarget = true; // Mark as reached
                GetComponent<BoxCollider2D>().enabled = false; // Disable the Box Collider
            }
        }

        // If the target is reached, scale down the image
        if (hasReachedTarget)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);

            // Check if the scale is close enough to the target scale
            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0); // Rotate 90 degrees on the Z-axis
                hasReachedTarget = false; // Reset flag to avoid redundant execution
            }
        }
    }
   
    void OnMouseDown()
    {
        // Start moving when the image is tapped
        if (!isMoving)
        {
            isMoving = true;
        }
    }
}