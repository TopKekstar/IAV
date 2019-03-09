using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    LineRenderer lineRenderer;
    Vector3[] path;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void SetPath(Stack<Vector2Int> pathStack)
    {
        lineRenderer.positionCount = pathStack.Count;

        path = new Vector3[pathStack.Count];
        for (int i = 0; i < path.Length; i++)
        {
            Vector2Int vector2Int = pathStack.Pop();
            path[i] = new Vector3(vector2Int.x, vector2Int.y);
        }
        
        lineRenderer.SetPositions(path);        
    }

    public void SetColorLine(Color color)
    {
        lineRenderer.endColor = color;
        lineRenderer.startColor = color;
    }
}

