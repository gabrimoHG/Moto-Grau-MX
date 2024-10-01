using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIHoverDetector : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    void Update()
    {
        DetectUIElement();
    }

    void DetectUIElement()
    {
        // Set up the PointerEventData with the current mouse position
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        // Create a list to hold the Raycast results
        List<RaycastResult> results = new List<RaycastResult>();

        // Perform the raycast using the GraphicRaycaster and pointer event data
        raycaster.Raycast(pointerEventData, results);

        // Check if any UI elements were detected
        if (results.Count > 0)
        {
            // Get the first detected UI element (closest to the camera)
            GameObject hoveredUIElement = results[0].gameObject;
            Debug.Log("Hovered UI element: " + hoveredUIElement.name);
        }
        else
        {
            Debug.Log("No UI element detected under the cursor.");
        }
    }
}
