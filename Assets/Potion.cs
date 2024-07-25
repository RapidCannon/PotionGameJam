using UnityEngine;

public class Potion : MonoBehaviour
{
    // Serialized field for the target object
    [SerializeField] private Transform targetObject;

    // Variable to store the initial position
    private Vector3 initialPosition;

    // Variables to track dragging state and offset
    private bool isDragging = false;
    private Vector3 offset;

    // Reference to the PotionPour script
    [SerializeField] private PotionPour potionPour;

    // Angle threshold for triggering the pour
    [SerializeField] private float tiltThreshold = 110f;

    // Cooldown time in seconds to prevent rapid toggling
    [SerializeField] private float cooldownTime = 0.1f;

    private float lastChangeTime = 0f;

    private void Start()
    {
        // Store the initial position
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Start dragging when the mouse button is pressed
        if (Input.GetMouseButtonDown(0) && IsMouseOverPotion())
        {
            StartDragging();
        }

        // Stop dragging when the mouse button is released
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }

        // Continue dragging while the mouse button is held down
        if (isDragging)
        {
            DragPotion();
        }
    }

    private bool IsMouseOverPotion()
    {
        // Check if the mouse is over the potion using a Raycast
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = GetComponent<Collider2D>();
        return collider.OverlapPoint(mousePosition);
    }

    private void StartDragging()
    {
        // Calculate the offset between the potion and the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the potion stays on the same plane
        offset = transform.position - mousePosition;

        isDragging = true;
    }

    private void StopDragging()
    {
        isDragging = false;
        // Snap the potion back to the initial position
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity; // Reset the rotation when not dragging
    }

    private void DragPotion()
    {
        // Move the potion with the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the potion stays on the same plane
        transform.position = mousePosition + offset;

        // Tilt towards the target object
        if (targetObject != null)
        {
            Vector3 direction = targetObject.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Check the tilt angle
            float tiltAngle = Vector3.Angle(Vector3.up, transform.up);

            // Check if cooldown period has passed
            if (Time.time - lastChangeTime >= cooldownTime)
            {
                // Start particle system if tilt angle exceeds threshold
                if (tiltAngle > tiltThreshold)
                {
                    if (!potionPour.pourParticleSystem.isPlaying)
                    {
                        potionPour.pourParticleSystem.Play();
                        lastChangeTime = Time.time; // Update the last change time
                    }
                }
                else
                {
                    if (potionPour.pourParticleSystem.isPlaying)
                    {
                        potionPour.pourParticleSystem.Stop();
                        lastChangeTime = Time.time; // Update the last change time
                    }
                }
            }
        }
    }
}
