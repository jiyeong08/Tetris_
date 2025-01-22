using UnityEngine;

public class re_GameMap : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform backgroundNode;
    [SerializeField] private Transform boardNode;
    
    [Header("Setting")]
    [Range(4, 40)]
    private static int boardWidth = 10;
    [Range(5, 20)]
    private static int boardHeight = 20;
    private int offset_x;
    private int offset_y;
    private int halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
    private int halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);

    public void CreateMap(int players, int playerNo, int offset_x, int offset_y)
    {
        this.offset_x = offset_x;
        this.offset_y = offset_y;
        
        clearMap();
        createBackground();
        createBoardColumn();
    }
    
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
    
    private void createBackground()
    {
        Color color = Color.gray;
        
        color.a = 0.5f;
        
        // 보드 영역
        for (int x = -halfWidth + offset_x; x < halfWidth + offset_x; ++x)
        {
            for (int y = halfHeight; y > -halfHeight + offset_y; --y)
            {
                createTile(backgroundNode, new Vector2(x, y), color, 0);
            }
        }

        color.a = 1.0f;

        // 테두리
        for (int y = halfHeight; y > -halfHeight + offset_y; --y) 
        { 
            createTile(backgroundNode, new Vector2(-halfWidth - 1 + offset_x, y), color, 0); 
            createTile(backgroundNode, new Vector2(halfWidth + offset_x, y), color, 0);
        }
        
        for (int x = -halfWidth - 1 + offset_x; x <= halfWidth + offset_x; ++x) 
        { 
            createTile(backgroundNode, new Vector2(x, -halfHeight + offset_y), color, 0);
        }
        
        Debug.Log("Created background finish");
    }
    
    private void createBoardColumn()
    {
        for (int i = 0; i < boardHeight - offset_y; ++i)
        {
            var col = new GameObject((boardHeight - i - 1).ToString());
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            col.transform.parent = boardNode;
        }
        
        Debug.Log("create board column finish");
    }
    
    private void clearMap()
    {
        
        foreach (Transform child in boardNode)
        {
            // 기존 타일 삭제
            Destroy(child.gameObject);
        }
        foreach (Transform child in backgroundNode)
        {
            // 기존 배경 타일 삭제
            Destroy(child.gameObject);
        }
        
    }
}
