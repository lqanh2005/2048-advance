using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int rows = 5;
    public int cols = 5;

    public RectTransform boardRoot;
    public RectTransform tilesParent;
    public Tile tilePrefab;

    private float cellSize;
    private bool[,] occ;
    [SerializeField] private Tile activeTile;

    private void Start()
    {
        InitBoard();
        SpawnLevelExample();
    }
    private void InitBoard()
    {
        float boardWidth = boardRoot.rect.width;
        float boardHeight = boardRoot.rect.height;
        cellSize = Mathf.Min(boardWidth / cols, boardHeight / rows);
        occ = new bool[rows, cols];
    }
    void SpawnLevelExample()
    {
        // Ví dụ tạo 1 tile 2x2 màu xanh, active
        activeTile = Instantiate(tilePrefab, tilesParent);
        activeTile.Setup(0, 0, 2, 2, cellSize, "2", Color.cyan);
        MarkOccupancy(activeTile, true);

        // Ví dụ tạo 1 tile 1x2 màu hồng, chướng ngại
        var blocker = Instantiate(tilePrefab, tilesParent);
        blocker.Setup(0, 2, 1, 2, cellSize, "4", Color.magenta);
        MarkOccupancy(blocker, true);
    }
    void MarkOccupancy(Tile t, bool value)
    {
        for (int r = t.row; r < t.row + t.height; r++)
            for (int c = t.col; c < t.col + t.width; c++)
                occ[r, c] = value;
    }
    private void Update()
    {
        if (activeTile == null) return;

        if (Input.GetKeyDown(KeyCode.A)) MoveActive(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.D)) MoveActive(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.W)) MoveActive(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.S)) MoveActive(Vector2Int.down);
    }
    void MoveActive(Vector2Int dir)
    {
        Debug.LogError("Move " + dir);
        // clear chỗ cũ
        MarkOccupancy(activeTile, false);

        int r = activeTile.row;
        int c = activeTile.col;

        while (true)
        {
            int rNext = r + dir.y;
            int cNext = c + dir.x;

            if (!InsideBoard(rNext, cNext, activeTile.width, activeTile.height)) break;
            if (!Free(rNext, cNext, activeTile.width, activeTile.height)) break;

            r = rNext;
            c = cNext;
        }

        activeTile.row = r;
        activeTile.col = c;
        activeTile.Refresh(cellSize);

        // set lại occ
        MarkOccupancy(activeTile, true);
    }
    bool InsideBoard(int row, int col, int w, int h)
    {
        return row >= 0 && col >= 0 && row + h <= rows && col + w <= cols;
    }

    bool Free(int row, int col, int w, int h)
    {
        for (int r = row; r < row + h; r++)
            for (int c = col; c < col + w; c++)
                if (occ[r, c]) return false;
        return true;
    }
}
