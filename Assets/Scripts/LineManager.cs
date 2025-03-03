using UnityEngine;

public class LineManager : MonoBehaviour
{
    [SerializeField] LineRenderer[] lineRenderers;
    int lineRendererIndex = 0;
    float minLineRendererPointSpacing = 0.3f;
    float minAngle = 30;

    public void UpdateLines(Vector2 pos)
    {
        var lineRenderer = lineRenderers[lineRendererIndex];
        if (lineRenderer.positionCount > 0 && Vector2.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), pos) < minLineRendererPointSpacing)
        {
            return;
        }

        if (IsAngleTooAcute(pos))
        {
            lineRendererIndex++;
            lineRenderer = lineRenderers[lineRendererIndex];
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }

    public void Reset()
    {
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.positionCount = 0;
        }
        lineRendererIndex = 0;
    }

    bool IsAngleTooAcute(Vector2 pos)
    {
        var lineRenderer = lineRenderers[lineRendererIndex];
        if (lineRenderer.positionCount < 2)
        {
            return false;
        }

        var p0 = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
        var p1 = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        var p2 = new Vector3(pos.x, pos.y, 0);

        var v0 = p0 - p1;
        var v1 = p2 - p1;

        var angle = Vector2.Angle(v0, v1);
        return angle < minAngle;
    }
}
