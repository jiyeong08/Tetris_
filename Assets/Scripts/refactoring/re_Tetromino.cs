using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class re_Tetromino : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tetrominoNode;
    [SerializeField] private Transform previewNode;
    
    [Header("Setting")]
    [Range(4, 40)]
    private static int boardWidth = 10;
    [Range(5, 20)]
    private static int boardHeight = 20;
    // private int offset_x = 0;
    private int halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
    private int halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);
    private int playableWidth = boardWidth + 1;
    
    private int indexVal = -1;
    [NonSerialized] public int index;
    
    private readonly Dictionary<int, Vector2[]> tetrominoShapes = new()
    {
        { 0, new[] { new Vector2(-2f, 0.0f), new Vector2(-1f, 0.0f), new Vector2(0f, 0.0f), new Vector2(1f, 0.0f) } },
        { 1, new[] { new Vector2(-1f, -1.0f), new Vector2(0f, -1.0f), new Vector2(1f, -1.0f), new Vector2(-1f, 0.0f) } },
        { 2, new[] { new Vector2(-1f, -1.0f), new Vector2(0f, -1.0f), new Vector2(1f, -1.0f), new Vector2(1f, 0.0f) } },
        { 3, new[] { new Vector2(-1f, -1f), new Vector2(0f, -1f), new Vector2(-1f, 0f), new Vector2(0f, 0f) } },
        { 4, new[] { new Vector2(-1f, -1f), new Vector2(0f, -1f), new Vector2(0f, 0f), new Vector2(1f, 0f) } },
        { 5, new[] { new Vector2(-1f, -1f), new Vector2(0f, -1f), new Vector2(1f, -1f), new Vector2(0f, 0f) } },
        { 6, new[] { new Vector2(-1f, 0f), new Vector2(0f, 0f), new Vector2(0f, -1f), new Vector2(1f, -1f) } },
    };
    
    private Tile createTile(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab);
        go.transform.parent = parent;
        go.transform.localPosition = position;
        
        var tile = go.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;
        
        return tile;
    }
    
    public void CreateTetromino(int players, int playerNo, int offset_x)
    {
        if (indexVal == -1)
        {
            index = Random.Range(0, 7);
        }
        else index = indexVal;
        
        Color32 color = Color.white;
        
        tetrominoNode.rotation = Quaternion.identity;
        tetrominoNode.position = new Vector2(offset_x, halfHeight);
        
        switch (index)
        {
            case 0: // I
                color = Color.cyan;
                foreach (var pos in tetrominoShapes[index]) createTile(tetrominoNode, pos, color);
                break;
            
            case 1: // J
                color = Color.blue;
                foreach (var pos in tetrominoShapes[index]) createTile(tetrominoNode, pos, color);
                break;
            
            case 2: // L
                color = Color.black;
                foreach (var pos in tetrominoShapes[index]) createTile(tetrominoNode, pos, color);
                break;
            
            case 3: // O
                color = Color.yellow;
                foreach (var pos in tetrominoShapes[index]) createTile(tetrominoNode, pos, color);
                break;
            
            case 4: // S
                color = Color.green;
                foreach (var pos in tetrominoShapes[index]) createTile(tetrominoNode, pos, color);
                break;
            
            case 5: // T
                color = Color.magenta;
                foreach (var pos in tetrominoShapes[index]) createTile(tetrominoNode, pos, color);
                break;
            
            case 6: // Z
                color = Color.red;
                foreach (var pos in tetrominoShapes[index]) createTile(tetrominoNode, pos, color);
                break;
        }
        CreatePreview(players, playerNo);
    }
    
    public void CreatePreview(int players, int playerNo)
    {
        foreach (Transform tile in previewNode)
        {
            Destroy(tile.gameObject);
        }
        previewNode.DetachChildren();
        
        var nextIndex = Random.Range(0, 7);
        while (nextIndex == indexVal)
        {
            nextIndex = Random.Range(0, 7);
        }
        indexVal = nextIndex;
        
        int previewX = halfWidth + 8;
        int previewY = halfHeight - 3;
        if (players == 2)
        {
            previewX = (2 * playerNo - 3) * (previewX + 2);
            previewY = previewY - 6;
        }
        else if (players == 3)
        {
            previewX = previewX + (12 * playerNo - 41);
            previewY = previewY - 18;
        }
        
        previewNode.position = new Vector2(previewX, previewY);
        Color32 color = Color.white;
        
        switch (indexVal)
        {
            case 0: // I
                color = Color.cyan;
                foreach (var pos in tetrominoShapes[indexVal]) createTile(previewNode, pos, color);
                break;
            
            case 1: // J
                color = Color.blue;
                foreach (var pos in tetrominoShapes[indexVal]) createTile(previewNode, pos, color);
                break;
            
            case 2: // L
                color = Color.black;
                foreach (var pos in tetrominoShapes[indexVal]) createTile(previewNode, pos, color);
                break;
            
            case 3: // O
                color = Color.yellow;
                foreach (var pos in tetrominoShapes[indexVal]) createTile(previewNode, pos, color);
                break;
            
            case 4: // S
                color = Color.green;
                foreach (var pos in tetrominoShapes[indexVal]) createTile(previewNode, pos, color);
                break;
            
            case 5: // T
                color = Color.magenta;
                foreach (var pos in tetrominoShapes[indexVal]) createTile(previewNode, pos, color);
                break;
            
            case 6: // Z
                color = Color.red;
                foreach (var pos in tetrominoShapes[indexVal]) createTile(previewNode, pos, color);
                break;
        }
    }
}
