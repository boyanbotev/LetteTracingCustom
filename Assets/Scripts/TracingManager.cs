using System.ComponentModel;
using UnityEngine;

enum TracingState
{
    Idle,
    Tracing,
    Completed
}
public class TracingManager : MonoBehaviour
{
    [SerializeField] Transform points;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float speed = 1f;
    [SerializeField] DraggableNode node;
    int pointIndex = 0;
    TracingState state = TracingState.Idle;
    float maxDragDistance = 1f;
    float touchPointDistance = 0.2f;
    float minLineRendererPointSpacing = 0.3f;

    public Vector2 GetNextPos(Vector2 currentPos, Vector2 mousePos)
    {
        if (Vector2.Distance(currentPos, mousePos) > maxDragDistance)
        {
            return currentPos;
        }

        if (Vector2.Distance(currentPos, GetNextPoint()) < touchPointDistance)
        {
            pointIndex++;
            if (pointIndex == points.childCount - 1)
            {
                Complete();
                return currentPos;
            }
        }

        var nextPoint = GetNextPoint();
        var currentPoint = points.GetChild(pointIndex).position;

        var dirToNextPoint = new Vector2(nextPoint.x - currentPoint.x, nextPoint.y - currentPoint.y);
        var nearestPosOnLine = NearestPointOnLine(currentPoint, dirToNextPoint, mousePos);

        if (Vector2.Distance(nearestPosOnLine, nextPoint) >= Vector2.Distance(currentPos, nextPoint)) {
            return currentPos;
        }

        Vector2 pos = Vector2.MoveTowards(currentPos, nearestPosOnLine, speed * Time.deltaTime);
        UpdateLineRenderer(pos); // TODO: emit event here
        return pos;
    }

    // TODO: move this to another class
    void UpdateLineRenderer(Vector2 pos)
    {
        if (lineRenderer.positionCount > 0 && Vector2.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), pos) < minLineRendererPointSpacing)
        {
            return;
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }

    void Complete()
    {
        state = TracingState.Completed;
        Debug.Log("Completed");
        node.Complete();
    }

    private Vector2 GetNextPoint()
    {
        return points.GetChild(pointIndex + 1).position;
    }

    private Vector2 NearestPointOnLine(Vector2 linePnt, Vector2 lineDir, Vector2 point)
    {
        lineDir.Normalize();
        var v = point - linePnt;
        var d = Vector2.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }
}
