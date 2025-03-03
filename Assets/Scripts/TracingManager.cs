using System.Collections;
using UnityEngine;
using System;

enum TracingState
{
    Idle,
    InitialTrace,
    Tracing,
    Completed
}
public class TracingManager : MonoBehaviour
{
    public static event Action OnComplete;
    [SerializeField] Transform points;
    [SerializeField] float speed = 1f;
    [SerializeField] float initialTraceSpeed = 1f;
    [SerializeField] DraggableNode node;
    int pointIndex = 0;
    TracingState state = TracingState.InitialTrace;
    float touchPointDistance = 0.01f;
    LineManager lineManager;

    void Awake()
    {
        lineManager = FindFirstObjectByType<LineManager>();
        node.gameObject.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(InitialTraceRoutine());
    }

    public Vector2 GetNextPos(Vector2 currentPos, Vector2 mousePos)
    {
        if (IsNearTarget(currentPos))
        {
            pointIndex++;
            if (pointIndex == points.childCount - 1)
            {
                Complete();
                return currentPos;
            }
        }

        var pos = CalculatePos(currentPos, mousePos);
        lineManager.UpdateLines(pos);
        return pos;
    }

    Vector2 CalculatePos(Vector2 currentPos, Vector2 mousePos)
    {
        var nextPoint = GetNextPoint();
        var currentPoint = points.GetChild(pointIndex).position;
        var dirToNextPoint = new Vector2(nextPoint.x - currentPoint.x, nextPoint.y - currentPoint.y);
        var nearestPosOnLine = NearestPointOnLine(currentPoint, dirToNextPoint, mousePos);

        var clampedPos = new Vector2(
           Mathf.Clamp(nearestPosOnLine.x, Mathf.Min(currentPos.x, nextPoint.x), Mathf.Max(currentPos.x, nextPoint.x)),
           Mathf.Clamp(nearestPosOnLine.y, Mathf.Min(currentPos.y, nextPoint.y), Mathf.Max(currentPos.y, nextPoint.y))
        );

        return Vector2.MoveTowards(currentPos, clampedPos, speed * Time.deltaTime);
    }

    bool IsNearTarget(Vector2 currentPos)
    {
        return Vector2.Distance(currentPos, GetNextPoint()) < touchPointDistance;
    }

    void Complete()
    {
        if (state == TracingState.InitialTrace)
        {
            return;
        }

        state = TracingState.Completed;
        Debug.Log("Completed");
        node.Complete();
        OnComplete?.Invoke();
    }

    private void Reset()
    {
        pointIndex = 0;
        lineManager.Reset();
        state = TracingState.Idle;
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

    IEnumerator InitialTraceRoutine()
    {
        var pos = points.GetChild(pointIndex).position;

        while (pointIndex < points.childCount - 1)
        {
            var nextPoint = GetNextPoint();
            var dir = new Vector2(nextPoint.x - pos.x, nextPoint.y - pos.y);
            var nextPos = new Vector2(pos.x + dir.x, pos.y + dir.y);
            pos = GetNextPos(pos, nextPos);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        Reset();
        node.gameObject.SetActive(true);
    }
}
