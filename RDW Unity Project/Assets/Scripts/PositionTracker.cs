using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class PositionTracker : MonoBehaviour
{
    public InputActionReference markPositionActionReference; // Assign in Inspector
    private List<Vector3> markedPositions = new List<Vector3>(); // Stores the marked positions

    public Material lineMaterial; // Assign a material for the line in the Inspector
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
    }

    private void OnEnable()
    {
        markPositionActionReference.action.performed += MarkPosition;
    }

    private void OnDisable()
    {
        markPositionActionReference.action.performed -= MarkPosition;
    }

    private void MarkPosition(InputAction.CallbackContext context)
    {
        if (markedPositions.Count < 4)
        {
            Vector3 currentPlayerPosition = this.transform.position;
            markedPositions.Add(new Vector3(currentPlayerPosition.x, 0, currentPlayerPosition.z)); // Adjust Y to 0
            Debug.Log($"Marked Position: {markedPositions.Count}: {currentPlayerPosition}");

            if (markedPositions.Count == 4)
            {
                DrawLines();
                CalculateAndLogArea();
                markedPositions.Clear(); // Reset the list for next time
            }
        }
    }

    private void DrawLines()
    {
        lineRenderer.positionCount = 5; // 4 points + 1 to close the loop

        for (int i = 0; i < 4; i++)
        {
            lineRenderer.SetPosition(i, markedPositions[i]);
        }

        lineRenderer.SetPosition(4, markedPositions[0]); // Close the loop
    }

    private void CalculateAndLogArea()
    {
        // Assuming the points form a convex quadrilateral, split into two triangles for area calculation
        float area = CalculateTriangleArea(markedPositions[0], markedPositions[1], markedPositions[2]) +
                     CalculateTriangleArea(markedPositions[2], markedPositions[3], markedPositions[0]);

        Debug.Log($"Area of the shape: {area} square units");

        for (int i = 0; i < markedPositions.Count; i++)
        {
            Debug.Log($"Point {i + 1}: X = {markedPositions[i].x}, Z = {markedPositions[i].z}");
        }
    }

    private float CalculateTriangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
        // Heron's formula for area calculation uses only the lengths of sides,
        // so we ensure to calculate distances using only x and z coordinates by setting y to a constant value.

        // Create vectors representing the points on the XZ plane (Y value is ignored for distance calculation)
        Vector3 aXZ = new Vector3(a.x, 0, a.z);
        Vector3 bXZ = new Vector3(b.x, 0, b.z);
        Vector3 cXZ = new Vector3(c.x, 0, c.z);

        // Calculate the sides' lengths on the XZ plane
        float sideA = Vector3.Distance(aXZ, bXZ);
        float sideB = Vector3.Distance(bXZ, cXZ);
        float sideC = Vector3.Distance(cXZ, aXZ);

        // Calculate the semiperimeter
        float s = (sideA + sideB + sideC) / 2;

        // Calculate the area using Heron's formula
        float area = Mathf.Sqrt(s * (s - sideA) * (s - sideB) * (s - sideC));
        return area;
    }

}
