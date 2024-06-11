// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

[System.Serializable]
public class Node
{
    public bool isWall;           // 해당 노드가 벽인지 여부
    public Node parentNode;       // 경로를 추적하기 위한 부모 노드
    public int x, y;              // 그리드 내 노드의 좌표
    public int gCost, hCost;      // gCost: 시작 노드로부터의 거리, hCost: 목표 노드까지의 휴리스틱 거리
    public int fCost { get { return gCost + hCost; } } // fCost: gCost와 hCost의 합계

    // 생성자: 노드를 초기화합니다.
    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall; // 벽인지 여부 설정
        x = _x; // x 좌표 설정
        y = _y; // y 좌표 설정
    }
}

public class GridManager : MonoBehaviour
{
    public Vector2Int bottomLeft, topRight; // 그리드의 좌하단 및 우상단 좌표
    public Transform player; // 플레이어의 Transform
    public Transform zombie; // 좀비의 Transform
    public float pathUpdateDelay = 1f; // 경로를 재계산하는 주기

    private Node[,] grid; // 그리드의 노드 배열
    private List<Node> path; // 계산된 경로
    private int sizeX, sizeY; // 그리드의 가로 및 세로 크기
    private bool isPathFinding = false; // 경로 탐색 중 여부

    private void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // 그리드의 크기를 계산합니다.
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        grid = new Node[sizeX, sizeY]; // 그리드 배열을 생성합니다.

        // 그리드의 각 셀을 초기화합니다.
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                // 해당 좌표에 벽이 있는지 확인합니다.
                bool isWall = Physics2D.OverlapCircle(new Vector2(x + bottomLeft.x, y + bottomLeft.y), 0.48f, LayerMask.GetMask("House")) != null;
                grid[x, y] = new Node(isWall, x + bottomLeft.x, y + bottomLeft.y); // 노드를 초기화합니다.
            }
        }
    }
    // A* 알고리즘을 사용하여 경로를 찾는 함수
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = NodeFromWorldPoint(startPos); // 시작 위치의 노드를 찾습니다.
        Node targetNode = NodeFromWorldPoint(targetPos); // 목표 위치의 노드를 찾습니다.

        List<Node> openList = new List<Node>(); // 열려있는 리스트 (탐색해야 할 노드)
        HashSet<Node> closedList = new HashSet<Node>(); // 닫혀있는 리스트 (탐색 완료된 노드)
        openList.Add(startNode); // 시작 노드를 openList에 추가합니다.

        while (openList.Count > 0)
        {
            Node currentNode = openList[0]; // openList의 첫 번째 노드를 현재 노드로 설정합니다.
            for (int i = 1; i < openList.Count; i++)
            {
                // fCost가 더 낮거나 같으면 hCost가 더 낮은 노드를 선택합니다.
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i]; // fCost와 hCost가 가장 낮은 노드를 현재 노드로 설정합니다.
                }
            }

            openList.Remove(currentNode); // 현재 노드를 openList에서 제거합니다.
            closedList.Add(currentNode); // 현재 노드를 closedList에 추가합니다.

            if (currentNode == targetNode) // 현재 노드가 목표 노드이면
            {
                RetracePath(startNode, targetNode); // 경로를 추적합니다.
                return; // 함수 종료
            }

            // 현재 노드의 이웃 노드를 탐색합니다.
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (neighbor.isWall || closedList.Contains(neighbor)) // 이웃 노드가 벽이거나 이미 탐색된 노드이면
                {
                    continue; // 다음 이웃 노드로 넘어갑니다.
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor); // 현재 노드에서 이웃 노드로의 이동 비용을 계산합니다.
                if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor)) // 새로운 이동 비용이 기존 비용보다 낮거나 이웃 노드가 openList에 없으면
                {
                    neighbor.gCost = newMovementCostToNeighbor; // gCost를 업데이트합니다.
                    neighbor.hCost = GetDistance(neighbor, targetNode); // hCost를 업데이트합니다.
                    neighbor.parentNode = currentNode; // 부모 노드를 현재 노드로 설정합니다.

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor); // 이웃 노드를 openList에 추가합니다.
                }
            }
        }
    }
    // 시작 노드에서 끝 노드까지 경로를 추적하는 함수
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> newPath = new List<Node>(); // 새로운 경로 리스트를 생성합니다.
        Node currentNode = endNode; // 현재 노드를 끝 노드로 설정합니다.

        // 시작 노드에 도달할 때까지 경로를 추적합니다.
        while (currentNode != startNode)
        {
            newPath.Add(currentNode); // 현재 노드를 경로 리스트에 추가합니다.
            currentNode = currentNode.parentNode; // 현재 노드를 부모 노드로 변경합니다.
        }
        newPath.Reverse(); // 경로를 역순으로 변경합니다.
        path = newPath; // 계산된 경로를 저장합니다.
    }

    // 월드 좌표를 그리드 노드로 변환하는 함수
    Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x - bottomLeft.x); // 월드 좌표의 x 값을 그리드 좌표로 변환합니다.
        int y = Mathf.RoundToInt(worldPosition.y - bottomLeft.y); // 월드 좌표의 y 값을 그리드 좌표로 변환합니다.
        return grid[x, y]; // 변환된 그리드 좌표의 노드를 반환합니다.
    }

    // 특정 노드의 이웃 노드를 반환하는 함수
    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>(); // 이웃 노드 리스트를 생성합니다.

        int[,] directions = new int[,]
        {
            {0, 1}, {1, 0}, {0, -1}, {-1, 0} // 상하좌우 방향
        };

        // 네 방향을 탐색합니다.
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int checkX = node.x + directions[i, 0]; // 이웃 노드의 x 좌표를 계산합니다.
            int checkY = node.y + directions[i, 1]; // 이웃 노드의 y 좌표를 계산합니다.

            // 이웃 노드가 그리드 범위 내에 있으면
            if (checkX >= bottomLeft.x && checkX < topRight.x && checkY >= bottomLeft.y && checkY < topRight.y)
            {
                neighbors.Add(grid[checkX - bottomLeft.x, checkY - bottomLeft.y]); // 이웃 노드를 리스트에 추가합니다.
            }
        }

        return neighbors; // 이웃 노드 리스트를 반환합니다.
    }

    // 두 노드 간의 거리를 계산하는 함수
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.x - nodeB.x); // x 좌표 간의 거리
        int distY = Mathf.Abs(nodeA.y - nodeB.y); // y 좌표 간의 거리
        return distX + distY; // 두 거리의 합을 반환합니다.
    }
}
