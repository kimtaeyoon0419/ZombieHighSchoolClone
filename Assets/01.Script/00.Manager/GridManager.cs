// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

[System.Serializable]
public class Node
{
    public bool isWall;           // �ش� ��尡 ������ ����
    public Node parentNode;       // ��θ� �����ϱ� ���� �θ� ���
    public int x, y;              // �׸��� �� ����� ��ǥ
    public int gCost, hCost;      // gCost: ���� ���κ����� �Ÿ�, hCost: ��ǥ �������� �޸���ƽ �Ÿ�
    public int fCost { get { return gCost + hCost; } } // fCost: gCost�� hCost�� �հ�

    // ������: ��带 �ʱ�ȭ�մϴ�.
    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall; // ������ ���� ����
        x = _x; // x ��ǥ ����
        y = _y; // y ��ǥ ����
    }
}

public class GridManager : MonoBehaviour
{
    public Vector2Int bottomLeft, topRight; // �׸����� ���ϴ� �� ���� ��ǥ
    public Transform player; // �÷��̾��� Transform
    public Transform zombie; // ������ Transform
    public float pathUpdateDelay = 1f; // ��θ� �����ϴ� �ֱ�

    private Node[,] grid; // �׸����� ��� �迭
    private List<Node> path; // ���� ���
    private int sizeX, sizeY; // �׸����� ���� �� ���� ũ��
    private bool isPathFinding = false; // ��� Ž�� �� ����

    private void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // �׸����� ũ�⸦ ����մϴ�.
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        grid = new Node[sizeX, sizeY]; // �׸��� �迭�� �����մϴ�.

        // �׸����� �� ���� �ʱ�ȭ�մϴ�.
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                // �ش� ��ǥ�� ���� �ִ��� Ȯ���մϴ�.
                bool isWall = Physics2D.OverlapCircle(new Vector2(x + bottomLeft.x, y + bottomLeft.y), 0.48f, LayerMask.GetMask("House")) != null;
                grid[x, y] = new Node(isWall, x + bottomLeft.x, y + bottomLeft.y); // ��带 �ʱ�ȭ�մϴ�.
            }
        }
    }
    // A* �˰����� ����Ͽ� ��θ� ã�� �Լ�
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = NodeFromWorldPoint(startPos); // ���� ��ġ�� ��带 ã���ϴ�.
        Node targetNode = NodeFromWorldPoint(targetPos); // ��ǥ ��ġ�� ��带 ã���ϴ�.

        List<Node> openList = new List<Node>(); // �����ִ� ����Ʈ (Ž���ؾ� �� ���)
        HashSet<Node> closedList = new HashSet<Node>(); // �����ִ� ����Ʈ (Ž�� �Ϸ�� ���)
        openList.Add(startNode); // ���� ��带 openList�� �߰��մϴ�.

        while (openList.Count > 0)
        {
            Node currentNode = openList[0]; // openList�� ù ��° ��带 ���� ���� �����մϴ�.
            for (int i = 1; i < openList.Count; i++)
            {
                // fCost�� �� ���ų� ������ hCost�� �� ���� ��带 �����մϴ�.
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i]; // fCost�� hCost�� ���� ���� ��带 ���� ���� �����մϴ�.
                }
            }

            openList.Remove(currentNode); // ���� ��带 openList���� �����մϴ�.
            closedList.Add(currentNode); // ���� ��带 closedList�� �߰��մϴ�.

            if (currentNode == targetNode) // ���� ��尡 ��ǥ ����̸�
            {
                RetracePath(startNode, targetNode); // ��θ� �����մϴ�.
                return; // �Լ� ����
            }

            // ���� ����� �̿� ��带 Ž���մϴ�.
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (neighbor.isWall || closedList.Contains(neighbor)) // �̿� ��尡 ���̰ų� �̹� Ž���� ����̸�
                {
                    continue; // ���� �̿� ���� �Ѿ�ϴ�.
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor); // ���� ��忡�� �̿� ������ �̵� ����� ����մϴ�.
                if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor)) // ���ο� �̵� ����� ���� ��뺸�� ���ų� �̿� ��尡 openList�� ������
                {
                    neighbor.gCost = newMovementCostToNeighbor; // gCost�� ������Ʈ�մϴ�.
                    neighbor.hCost = GetDistance(neighbor, targetNode); // hCost�� ������Ʈ�մϴ�.
                    neighbor.parentNode = currentNode; // �θ� ��带 ���� ���� �����մϴ�.

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor); // �̿� ��带 openList�� �߰��մϴ�.
                }
            }
        }
    }
    // ���� ��忡�� �� ������ ��θ� �����ϴ� �Լ�
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> newPath = new List<Node>(); // ���ο� ��� ����Ʈ�� �����մϴ�.
        Node currentNode = endNode; // ���� ��带 �� ���� �����մϴ�.

        // ���� ��忡 ������ ������ ��θ� �����մϴ�.
        while (currentNode != startNode)
        {
            newPath.Add(currentNode); // ���� ��带 ��� ����Ʈ�� �߰��մϴ�.
            currentNode = currentNode.parentNode; // ���� ��带 �θ� ���� �����մϴ�.
        }
        newPath.Reverse(); // ��θ� �������� �����մϴ�.
        path = newPath; // ���� ��θ� �����մϴ�.
    }

    // ���� ��ǥ�� �׸��� ���� ��ȯ�ϴ� �Լ�
    Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x - bottomLeft.x); // ���� ��ǥ�� x ���� �׸��� ��ǥ�� ��ȯ�մϴ�.
        int y = Mathf.RoundToInt(worldPosition.y - bottomLeft.y); // ���� ��ǥ�� y ���� �׸��� ��ǥ�� ��ȯ�մϴ�.
        return grid[x, y]; // ��ȯ�� �׸��� ��ǥ�� ��带 ��ȯ�մϴ�.
    }

    // Ư�� ����� �̿� ��带 ��ȯ�ϴ� �Լ�
    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>(); // �̿� ��� ����Ʈ�� �����մϴ�.

        int[,] directions = new int[,]
        {
            {0, 1}, {1, 0}, {0, -1}, {-1, 0} // �����¿� ����
        };

        // �� ������ Ž���մϴ�.
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int checkX = node.x + directions[i, 0]; // �̿� ����� x ��ǥ�� ����մϴ�.
            int checkY = node.y + directions[i, 1]; // �̿� ����� y ��ǥ�� ����մϴ�.

            // �̿� ��尡 �׸��� ���� ���� ������
            if (checkX >= bottomLeft.x && checkX < topRight.x && checkY >= bottomLeft.y && checkY < topRight.y)
            {
                neighbors.Add(grid[checkX - bottomLeft.x, checkY - bottomLeft.y]); // �̿� ��带 ����Ʈ�� �߰��մϴ�.
            }
        }

        return neighbors; // �̿� ��� ����Ʈ�� ��ȯ�մϴ�.
    }

    // �� ��� ���� �Ÿ��� ����ϴ� �Լ�
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.x - nodeB.x); // x ��ǥ ���� �Ÿ�
        int distY = Mathf.Abs(nodeA.y - nodeB.y); // y ��ǥ ���� �Ÿ�
        return distX + distY; // �� �Ÿ��� ���� ��ȯ�մϴ�.
    }
}
