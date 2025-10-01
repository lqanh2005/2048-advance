// SafeLevelGenerator.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Dir { Left, Right, Up, Down }

[Serializable]
public class TileData
{
    public string id;
    public int value;
    public int row, col;           // anchor (góc trên–trái)
    public Vector2Int[] baseMask;  // polyomino gốc (đơn vị 1x1)
    public int sx = 1, sy = 1;     // scale theo lưới (tính bằng số ô)
}

[Serializable]
public class LevelData
{
    public int rows, cols;
    public int goalValue;
    public List<TileData> tiles = new List<TileData>();
    public List<string> solutionMoves = new List<string>();
    public string notes;
}

static class PolyUtil
{
    public static Vector2Int[] Inflate(Vector2Int[] baseMask, int sx, int sy)
    {
        var cells = new List<Vector2Int>(baseMask.Length * sx * sy);
        foreach (var p in baseMask)
            for (int dy = 0; dy < sy; dy++)
                for (int dx = 0; dx < sx; dx++)
                    cells.Add(new Vector2Int(p.x * sx + dx, p.y * sy + dy));
        return cells.ToArray();
    }
    public static (int dr, int dc) Delta(Dir d) =>
        d == Dir.Left ? (0, -1) :
        d == Dir.Right ? (0, 1) :
        d == Dir.Up ? (-1, 0) : (1, 0);
    public static Dir Opp(Dir d) =>
        d == Dir.Left ? Dir.Right :
        d == Dir.Right ? Dir.Left :
        d == Dir.Up ? Dir.Down : Dir.Up;
}

class Board
{
    public readonly int rows, cols;
    public TileData[,] occ;
    public List<TileData> tiles = new List<TileData>();
    public Board(int r, int c) { rows = r; cols = c; occ = new TileData[r, c]; }

    bool InBounds(int r, int c) => r >= 0 && c >= 0 && r < rows && c < cols;

    public bool CanPlace(TileData t, int row, int col)
    {
        var mask = PolyUtil.Inflate(t.baseMask, t.sx, t.sy);
        foreach (var off in mask)
        {
            int R = row + off.y, C = col + off.x;
            if (!InBounds(R, C)) return false;
            var at = occ[R, C];
            if (at != null && !ReferenceEquals(at, t)) return false;
        }
        return true;
    }
    public void Unoccupy(TileData t)
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (occ[r, c] == t) occ[r, c] = null;
    }
    public void Occupy(TileData t, int row, int col)
    {
        Unoccupy(t);
        var mask = PolyUtil.Inflate(t.baseMask, t.sx, t.sy);
        foreach (var off in mask) occ[row + off.y, col + off.x] = t;
        t.row = row; t.col = col;
    }
    public (int r, int c) SlideMax(TileData t, Dir d)
    {
        var (dr, dc) = PolyUtil.Delta(d);
        Unoccupy(t);
        int r = t.row, c = t.col;
        while (CanPlace(t, r + dr, c + dc)) { r += dr; c += dc; }
        Occupy(t, r, c);
        return (r, c);
    }
}

public static class SafeLevelGenerator
{
    public static LevelData GenerateSafeLevel(
        int rows, int cols,
        Vector2Int[] baseMask, int sx, int sy,
        int targetValue, int steps, int seed)
    {
        var rng = new System.Random(seed);
        var board = new Board(rows, cols);

        var target = new TileData
        {
            id = "T",
            value = targetValue,
            baseMask = (Vector2Int[])baseMask.Clone(),
            sx = sx,
            sy = sy
        };

        // đặt target gần giữa
        if (!TryPlacePreferCenter(board, target))
            throw new Exception("Không đặt được tile target (board quá nhỏ?)");

        var forwardMoves = new List<string>();

        for (int i = 0; i < steps; i++)
        {
            Dir forwardDir = (Dir)rng.Next(0, 4);
            Dir backDir = PolyUtil.Opp(forwardDir);
            forwardMoves.Add(forwardDir.ToString());

            if ((target.value & 1) == 1)
                throw new Exception($"Step {i}: Target value lẻ ({target.value}), không tách được.");
            int childV = target.value / 2;

            var A = new TileData
            {
                id = $"A{i}",
                value = childV,
                baseMask = (Vector2Int[])target.baseMask.Clone(),
                sx = target.sx,
                sy = target.sy
            };
            var B = new TileData
            {
                id = $"B{i}",
                value = childV,
                baseMask = (Vector2Int[])target.baseMask.Clone(),
                sx = target.sx,
                sy = target.sy
            };

            board.Unoccupy(target);
            if (!board.CanPlace(A, target.row, target.col))
                throw new Exception($"Step {i}: Không đặt A vào vị trí target.");
            board.Occupy(A, target.row, target.col);

            var (dr, dc) = PolyUtil.Delta(backDir);
            int br = A.row + dr, bc = A.col + dc;

            if (!board.CanPlace(B, br, bc))
            {
                bool found = false; int rr = A.row, cc = A.col;
                for (int step = 1; step < Math.Max(rows, cols); step++)
                {
                    rr += dr; cc += dc;
                    if (!board.CanPlace(B, rr, cc)) continue;
                    br = rr; bc = cc; found = true; break;
                }
                if (!found) throw new Exception($"Step {i}: Không tìm được chỗ đặt B theo hướng back.");
            }
            board.Occupy(B, br, bc);

            board.SlideMax(A, backDir);
            board.SlideMax(B, backDir);

            target = A; // tiếp tục split A lần sau
        }

        var level = new LevelData
        {
            rows = rows,
            cols = cols,
            goalValue = targetValue,
            notes = $"seed={seed}, steps={steps}"
        };

        var exported = new HashSet<TileData>();
        for (int r = 0; r < board.rows; r++)
            for (int c = 0; c < board.cols; c++)
            {
                var t = board.occ[r, c];
                if (t != null && exported.Add(t))
                    level.tiles.Add(CloneForExport(t));
            }

        forwardMoves.Reverse();
        level.solutionMoves = forwardMoves;
        return level;
    }

    static bool TryPlacePreferCenter(Board b, TileData t)
    {
        var list = new List<(int r, int c)>();
        for (int r = 0; r < b.rows; r++)
            for (int c = 0; c < b.cols; c++)
                list.Add((r, c));
        float cr = (b.rows - 1) * 0.5f, cc = (b.cols - 1) * 0.5f;
        list.Sort((p, q) => {
            float dp = (p.r - cr) * (p.r - cr) + (p.c - cc) * (p.c - cc);
            float dq = (q.r - cr) * (q.r - cr) + (q.c - cc) * (q.c - cc);
            return dp.CompareTo(dq);
        });
        foreach (var pos in list)
        {
            if (b.CanPlace(t, pos.r, pos.c)) { b.Occupy(t, pos.r, pos.c); return true; }
        }
        return false;
    }

    static TileData CloneForExport(TileData t) => new TileData
    {
        id = t.id,
        value = t.value,
        row = t.row,
        col = t.col,
        baseMask = (Vector2Int[])t.baseMask.Clone(),
        sx = t.sx,
        sy = t.sy
    };
}
