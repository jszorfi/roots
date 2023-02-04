using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private SpriteAnimator Animator;

    private List<PathFinding.PathNode> m_NodesOnPath;
    private Vector2Int m_TargetCoordinates;
    private MapController mapController;

    public void StartMovingTo(Vector2Int destination)
    {
        m_TargetCoordinates = destination;
        StartCoroutine(MoveToDestination());
    }

    // Start is called before the first frame update
    void Start()
    {
        Animator = gameObject.GetComponent<SpriteAnimator>();
        mapController = FindObjectOfType<MapController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private static Vector3 CellCenterToPoint(Vector2Int cell)
    {
        return new Vector3(cell.x + 0.5f, cell.y + 0.5f, -1.0f);
    }

    private static Vector2Int PointToGridCell(Vector3 point)
    {
        return new Vector2Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y));
    }

    private IEnumerator MoveToDestination()
    {
        // Getting the walkable nodes from the map
        List<PathFinding.PathNode> nodes = mapController.getPathNodeList();

        // Calculating current position on the map
        Vector2Int currentPosition = PointToGridCell(transform.position);

        // Add back current position as it is technically impassable
        nodes.Add(new PathFinding.PathNode(currentPosition));

        // Finding a path from the current position to the target position
        m_NodesOnPath = PathFinding.FindPath(nodes, currentPosition, m_TargetCoordinates);
        Vector2Int lastTargetPosition = m_TargetCoordinates;

        // Finding a path to the destination
        while(Vector2Int.FloorToInt(transform.position) != m_TargetCoordinates)
        {
            // Checking whether we can not advance anymore
            if(m_NodesOnPath.Count == 0)
            {
                break;
            }

            // Advancing one step towards the destination
            Vector2Int nextCoordinates = m_NodesOnPath[0].position;
            Vector3 nextPosition = CellCenterToPoint(nextCoordinates);
            m_NodesOnPath.RemoveAt(0);

            // Updating animation according to movement direction
            if(nextCoordinates.x > Vector2Int.FloorToInt(transform.position).x)
            {
                Animator.SetAnimationByName("Walk Right");
            }
            else if(nextCoordinates.x < Vector2Int.FloorToInt(transform.position).x)
            {
                Animator.SetAnimationByName("Walk Left");
            }
            else if(nextCoordinates.y > Vector2Int.FloorToInt(transform.position).y)
            {
                Animator.SetAnimationByName("Walk Down");
            }
            else if(nextCoordinates.y < Vector2Int.FloorToInt(transform.position).y)
            {
                Animator.SetAnimationByName("Walk Down");
            }

            // Updating the position on the Level 
            while(Vector3.Distance(transform.position, nextPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, 1.5f * Time.deltaTime);
                yield return null;
            }

            // Waiting one second until the next step
            yield return new WaitForSeconds(0.01f);
        }

        // If we are not moving, play the Idle animation
        Animator.SetAnimationByName("Idle");
    }
}
