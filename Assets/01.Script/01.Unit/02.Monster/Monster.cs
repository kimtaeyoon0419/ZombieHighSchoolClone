//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Monster : MonoBehaviour
//{
//    public Transform player; // 플레이어의 Transform
//    public float pathUpdateDelay = 1f; // 경로를 재계산하는 주기

//    private List<Node> path;
//    private bool isPathFinding = false;

//    void Start()
//    {
//        StartCoroutine(UpdatePath()); // 주기적으로 경로를 업데이트하는 코루틴을 시작합니다.
//    }

//    // 주기적으로 경로를 업데이트하는 코루틴
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

//    // 경로를 찾는 함수 (A* 알고리즘)
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

//    // 경로를 역추적하여 path 리스트를 설정하는 함수
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

//    // 월드 좌표에서 노드를 가져오는 함수
//    Node NodeFromWorldPoint(Vector3 worldPosition)
//    {
//        int x = Mathf.RoundToInt(worldPosition.x - GameManager.instance.bottomLeft.x);
//        int y = Mathf.RoundToInt(worldPosition.y - GameManager.instance.bottomLeft.y);
//        return GameManager.grid[x, y];
//    }

//    // 현재 노드의 이웃 노드를 가져오는 함수
//    List<Node> GetNeighbors(Node node)
//    {
//        List<Node> neighbors = new List<Node>();

//        int[,] directions = new int[,]
//        {
//            {0, 1}, {1, 0}, {0, -1}, {-1, 0} // 상하좌우 방향
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

//    // 두 노드 간의 거리를 계산하는 함수
//    int GetDistance(Node nodeA, Node nodeB)
//    {
//        int distX = Mathf.Abs(nodeA.x - nodeB.x);
//        int distY = Mathf.Abs(nodeA.y - nodeB.y);
//        return distX + distY;
//    }

//    // 경로를 따라 이동하는 코루틴
//    IEnumerator FollowPath()
//    {
//        isPathFinding = true; // 경로 탐색 중으로 설정합니다.
//        foreach (Node node in path) // 경로의 각 노드에 대해
//        {
//            Vector3 targetPosition = new Vector3(node.x, node.y, 0); // 목표 위치를 설정합니다.
//            while (Vector3.Distance(transform.position, targetPosition) > 0.1f) // 좀비가 목표 위치에 도달할 때까지
//            {
//                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime); // 좀비를 목표 위치로 이동시킵니다.
//                yield return null; // 다음 프레임까지 기다립니다.
//            }
//        }
//        isPathFinding = false; // 경로 탐색이 완료되었음을 설정합니다.
//    }
//}
