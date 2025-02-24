using UnityEngine;

enum NodeState
{
    Idle,
    Dragging,
    Completed
}
public class DraggableNode : MonoBehaviour
{
    TracingManager tracingManager;
    NodeState state = NodeState.Idle;

    public void Complete()
    {
        state = NodeState.Completed;
    }

    private void Start()
    {
        tracingManager = FindFirstObjectByType<TracingManager>();
    }

    private void Update()
    {
        if (state == NodeState.Completed)
        {
            return;
        }
        // TODO: add clicking on node to start dragging

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            var nextPos = tracingManager.GetNextPos(transform.position, mousePosition);
            transform.position = nextPos;
        }
    }
}
