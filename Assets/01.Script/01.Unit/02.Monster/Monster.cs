//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Monster : MonoBehaviour
//{
//    public Transform player; // �÷��̾��� Transform
//    public float pathUpdateDelay = 1f; // ��θ� �����ϴ� �ֱ�

//    private List<Node> path;
//    private bool isPathFinding = false;

//    void Start()
//    {
//        StartCoroutine(UpdatePath()); // �ֱ������� ��θ� ������Ʈ�ϴ� �ڷ�ƾ�� �����մϴ�.
//    }

//    // �ֱ������� ��θ� ������Ʈ�ϴ� �ڷ�ƾ
//    IEnumerator UpdatePath()
//    {
//        while (true)
//        {
//            if (!isPathFinding)
//            {
//                FindPath(transform.position, player.position);
//                if (path != null && path.Count > 0)
//                    StartCoroutine(FollowPath());
//            }
//            yield return new WaitForSeconds(pathUpdateDelay);
//        }
//    }

//    // ��θ� ã�� �Լ� (A* �˰���)
//    void FindPath(Vector3 startPos, Vector3 targetPos)
//    {
//        Node startNode = NodeFromWorldPoint(startPos);
//        Node targetNode = NodeFromWorldPoint(targetPos);

//        List<Node> openList = new List<Node>();
//        HashSet<Node> closedList = new HashSet<Node>();
//        openList.Add(startNode);

//        while (openList.Count > 0)
//        {
//            Node currentNode = openList[0];
//            for (int i = 1; i < openList.Count; i++)
//            {
//                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
//                {
//                    currentNode = openList[i];
//                }
//            }

//            openList.Remove(currentNode);
//            closedList.Add(currentNode);

//            if (currentNode == targetNode)
//            {
//                RetracePath(startNode, targetNode);
//                return;
//            }

//            foreach (Node neighbor in GetNeighbors(currentNode))
//            {
//                if (neighbor.isWall || closedList.Contains(neighbor))
//                {
//                    continue;
//                }

//                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
//                if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
//                {
//                    neighbor.gCost = newMovementCostToNeighbor;
//                    neighbor.hCost = GetDistance(neighbor, targetNode);
//                    neighbor.parentNode = currentNode;

//                    if (!openList.Contains(neighbor))
//                        openList.Add(neighbor);
//                }
//            }
//        }
//    }

//    // ��θ� �������Ͽ� path ����Ʈ�� �����ϴ� �Լ�
//    void RetracePath(Node startNode, Node endNode)
//    {
//        List<Node> newPath = new List<Node>();
//        Node currentNode = endNode;

//        while (currentNode != startNode)
//        {
//            newPath.Add(currentNode);
//            currentNode = currentNode.parentNode;
//        }
//        newPath.Reverse();
//        path = newPath;
//    }

//    // ���� ��ǥ���� ��带 �������� �Լ�
//    Node NodeFromWorldPoint(Vector3 worldPosition)
//    {
//        int x = Mathf.RoundToInt(worldPosition.x - GameManager.instance.bottomLeft.x);
//        int y = Mathf.RoundToInt(worldPosition.y - GameManager.instance.bottomLeft.y);
//        return GameManager.grid[x, y];
//    }

//    // ���� ����� �̿� ��带 �������� �Լ�
//    List<Node> GetNeighbors(Node node)
//    {
//        List<Node> neighbors = new List<Node>();

//        int[,] directions = new int[,]
//        {
//            {0, 1}, {1, 0}, {0, -1}, {-1, 0} // �����¿� ����
//        };

//        for (int i = 0; i < directions.GetLength(0); i++)
//        {
//            int checkX = node.x + directions[i, 0];
//            int checkY = node.y + directions[i, 1];

//            if (checkX >= GameManager.instance.bottomLeft.x && checkX < GameManager.instance.topRight.x && checkY >= GameManager.instance.bottomLeft.y && checkY < GameManager.instance.topRight.y)
//            {
//                neighbors.Add(GameManager.grid[checkX - GameManager.instance.bottomLeft.x, checkY - GameManager.instance.bottomLeft.y]);
//            }
//        }

//        return neighbors;
//    }

//    // �� ��� ���� �Ÿ��� ����ϴ� �Լ�
//    int GetDistance(Node nodeA, Node nodeB)
//    {
//        int distX = Mathf.Abs(nodeA.x - nodeB.x);
//        int distY = Mathf.Abs(nodeA.y - nodeB.y);
//        return distX + distY;
//    }

//    // ��θ� ���� �̵��ϴ� �ڷ�ƾ
//    IEnumerator FollowPath()
//    {
//        isPathFinding = true; // ��� Ž�� ������ �����մϴ�.
//        foreach (Node node in path) // ����� �� ��忡 ����
//        {
//            Vector3 targetPosition = new Vector3(node.x, node.y, 0); // ��ǥ ��ġ�� �����մϴ�.
//            while (Vector3.Distance(transform.position, targetPosition) > 0.1f) // ���� ��ǥ ��ġ�� ������ ������
//            {
//                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime); // ���� ��ǥ ��ġ�� �̵���ŵ�ϴ�.
//                yield return null; // ���� �����ӱ��� ��ٸ��ϴ�.
//            }
//        }
//        isPathFinding = false; // ��� Ž���� �Ϸ�Ǿ����� �����մϴ�.
//    }
//}
