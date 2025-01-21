using UnityEngine;

// 보드 상태 저장 및 행 삭제 로직
public class Board
{
    public int width;
    private int height;
    private Color[,] grid;

    public Board(int width, int height)
    {
        this.width = width;
        this.height = height;
        grid = new Color[width, height];
    }

    public bool AddToBoard(Vector2[] positions, Color color)
    {
        foreach (var pos in positions)
        {
            if (IsValidPosition(pos))
                grid[(int)pos.x, (int)pos.y] = color;
            else
                return false;
        }
        return false;
    }

    public bool IsRowComplete(int row)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, row] == Color.clear) return false;
        }
        return true;
    }

    public void ClearRow(int row)
    {
        for (int y = row; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = grid[x, y + 1];
            }
        }
        for (int x = 0; x < width; x++) grid[x, height - 1] = Color.clear;
    }

    private bool IsValidPosition(Vector2 position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }


    public bool IsOccupied(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;

        return grid[x, y] != Color.clear;
    }
}
