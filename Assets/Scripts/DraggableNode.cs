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
        gameObject.SetActive(false);
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

            var nextPos = tracingManager.GetNextPos(transform.position, mousePosition);
            transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
        }
    }
}
