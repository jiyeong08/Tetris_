using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class re_GamePlay : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform backgroundNode;
    [SerializeField] private Transform boardNode;
    [SerializeField] private Transform tetrominoNode;
    [SerializeField] private Transform previewNode;
    
    [Header("Setting")]
    [Range(4, 40)]
    private int boardWidth = 10;
    [Range(5, 20)]
    private int boardHeight = 20;
    private float fallCycle;
    private int offset_x = 0;
    private int offset_y = 0;
    private int halfWidth;
    private int halfHeight;
    private float nextFallTime;

    private bool gameEnd;
    public event Action<int, int, int, string> GameEndOn;
    
    [SerializeField] private re_GamePlayUI playUI;
    [NonSerialized] public int scoreVal;
    [NonSerialized] public int levelVal;
    private int lineVal;
    
    private int indexVal;
    private int index;
    
    // player setting
    private int players;
    private int playerNo;
    private string upKey;
    private string downKey;
    private string leftKey;
    private string rightKey;
    private string dropKey;
    private string bombKey;
    private string changeKey;
    
    // Item
    private bool itemMode;
    [SerializeField] private GameObject items;
    private int bombItem;
    [SerializeField] private AudioSource bombEffect;
    private int changeItem;
    [SerializeField] private AudioSource changeEffect;
    
    public void GameStart(int players, int playerNo, bool itemMode)
    {
        clearBlock();
        
        gameEnd = false;
        
        fallCycle = 1.0f;
        scoreVal = 0;
        levelVal = 1;
        indexVal =- 1;
        
        this.players = players;
        this.playerNo = playerNo;

        switch (this.playerNo)
        {
            case 1:
                upKey = "w";
                downKey = "s";
                leftKey = "a";
                rightKey = "d";
                dropKey = "space";
                bombKey = "v";
                changeKey = "b";
                break;
            case 2:
                upKey = "up";
                downKey = "down";
                leftKey = "left";
                rightKey = "right";
                dropKey = "/";
                bombKey = ";";
                changeKey = "'";
                break;
        }
        
        this.itemMode = itemMode;
        if (this.itemMode)
        {
            items.SetActive(true);
            bombItem = 3;
            changeItem = 0;
            playUI.UpdateBomb(bombItem);
            playUI.UpdateChange(changeItem);
        }
        else items.SetActive(false);
        
        lineVal = levelVal * 2;
        playUI.UpdateLevel(levelVal);
        playUI.UpdateScore(scoreVal);
        playUI.UpdateLine(lineVal);
        
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);

        nextFallTime = Time.time + fallCycle;
        
        int playableWidth = boardWidth + 1; // 각 플레이어의 보드 가로 영역
        if (players == 2)
        {
            offset_x = (playerNo - 1) * (playableWidth + 2) - (playableWidth / 2 + 1);
        }
        else if (players == 3)
        {
            offset_x = (playerNo - 1) * (playableWidth + 2) - (playableWidth + 2);
            offset_y = 3;
        }

        // CreateBackground();
        // CreateBoardColumn();
        CreateTetromino();
    }
    
    Tile CreateTile(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab);
        go.transform.parent = parent;
        go.transform.localPosition = position;
        
        var tile = go.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;
        
        return tile;
    }
    
    void CreateBackground()
    {
        Color color = Color.gray;
        
        color.a = 0.5f;
        
        // 보드 영역
        for (int x = -halfWidth + offset_x; x < halfWidth + offset_x; ++x)
        {
            for (int y = halfHeight; y > -halfHeight + offset_y; --y)
            {
                CreateTile(backgroundNode, new Vector2(x, y), color, 0);
            }
        }

        color.a = 1.0f;

        // 테두리
        for (int y = halfHeight; y > -halfHeight + offset_y; --y) 
        { 
            CreateTile(backgroundNode, new Vector2(-halfWidth - 1 + offset_x, y), color, 0); 
            CreateTile(backgroundNode, new Vector2(halfWidth + offset_x, y), color, 0);
        }
        
        for (int x = -halfWidth - 1 + offset_x; x <= halfWidth + offset_x; ++x) 
        { 
            CreateTile(backgroundNode, new Vector2(x, -halfHeight + offset_y), color, 0);
        }
    }
    
    void CreateBoardColumn()
    {
        for (int i = 0; i < boardHeight - offset_y; ++i)
        {
            var col = new GameObject((boardHeight - i - 1).ToString());
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            col.transform.parent = boardNode;
        }
    }
    
    void CreateTetromino()
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
                CreateTile(tetrominoNode, new Vector2(-2f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), color);
                break;
            case 1: // J
                color = Color.blue;
                CreateTile(tetrominoNode, new Vector2(-1f, -1.0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, -1.0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, -1.0f), color);
                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), color);
                break;
            case 2: // L
                color = Color.black;
                CreateTile(tetrominoNode, new Vector2(-1f, -1.0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, -1.0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, -1.0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), color);
                break;
            case 3: // O
                color = Color.yellow;
                CreateTile(tetrominoNode, new Vector2(-1f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(-1f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                break;
            case 4: // S
                color = Color.green;
                CreateTile(tetrominoNode, new Vector2(-1f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), color);
                break;
            case 5: // T
                color = Color.magenta;
                CreateTile(tetrominoNode, new Vector2(-1f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(1f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                break;
            case 6: // Z
                color = Color.red;
                CreateTile(tetrominoNode, new Vector2(-1f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(1f, -1f), color);
                break;
        }
        CreatePreview();
    }
    
    void CreatePreview()
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
                CreateTile(previewNode, new Vector2(-2f, 0.0f), color);
                CreateTile(previewNode, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode, new Vector2(1f, 0.0f), color);
                break;
            case 1: // J
                color = Color.blue;
                CreateTile(previewNode, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode, new Vector2(1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(-1f, 1.0f), color);
                break;
            case 2: // L
                color = Color.black;
                CreateTile(previewNode, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode, new Vector2(1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(1f, 1.0f), color);
                break;
            case 3: // O
                color = Color.yellow;
                CreateTile(previewNode, new Vector2(-1f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(-1f, 1f), color);
                CreateTile(previewNode, new Vector2(0f, 1f), color);
                break;
            case 4: // S
                color = Color.green;
                CreateTile(previewNode, new Vector2(-1f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 1f), color);
                CreateTile(previewNode, new Vector2(1f, 1f), color);
                break;
            case 5: // T
                color = Color.magenta;
                CreateTile(previewNode, new Vector2(-1f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(1f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 1f), color);
                break;
            case 6: // Z
                color = Color.red;
                CreateTile(previewNode, new Vector2(-1f, 1f), color);
                CreateTile(previewNode, new Vector2(0f, 1f), color);
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(1f, 0f), color);
                break;
        }
    }

    void Update()
    {
        if (gameEnd) { }
        else
        {
            Vector3 moveDir = Vector3.zero;
            bool isRotate = false;
            if (playerNo == 1 || playerNo == 2)
            {
                if (Input.GetKeyDown(leftKey))
                {
                    moveDir.x = -1;
                }
                else if (Input.GetKeyDown(rightKey))
                {
                    moveDir.x = 1;
                }

                if (Input.GetKeyDown(upKey))
                {
                    if (index != 3) isRotate = true;
                }
                else if (Input.GetKeyDown(downKey))
                {
                    moveDir.y = -1;
                }

                if (Input.GetKeyDown(dropKey))
                {
                    while (MoveTetromino(Vector3.down, false))
                    {

                    }
                }
            }
            else if (playerNo == 3)
            {
                float mouseMove = Input.GetAxis("Mouse X");
                float wheelInput = Input.GetAxis("Mouse ScrollWheel");
                if (mouseMove < 0)
                {
                    moveDir.x = -1;
                }
                else if (mouseMove > 0)
                {
                    moveDir.x = 1;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (index != 3) isRotate = true;
                }
                else if (wheelInput < 0)
                {
                    moveDir.y = -1;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    while (MoveTetromino(Vector3.down, false))
                    {
                    }
                }
            }

            if (itemMode)
            {
                if (playerNo == 1 || playerNo == 2)
                {
                    if (Input.GetKeyDown(bombKey) && bombItem > 0)
                    {
                        DestroyLineByItem();
                    }

                    if (Input.GetKeyDown(changeKey) && changeItem > 0)
                    {
                        ChangeNextByItem();
                    }
                }
                else if (playerNo == 3)
                {
                    if (Input.GetKeyDown("-") && bombItem > 0)
                    {
                        DestroyLineByItem();
                    }

                    if (Input.GetKeyDown("+") && changeItem > 0)
                    {
                        ChangeNextByItem();
                    }
                }
            }
            
            if (Time.time > nextFallTime)
            {
                nextFallTime = Time.time + fallCycle;
                moveDir.y = -1;
                isRotate = false;
            }
            if (moveDir != Vector3.zero || isRotate)
            {
                MoveTetromino(moveDir, isRotate);
            }
        }
    }
    
    bool MoveTetromino(Vector3 moveDir, bool isRotate)
    {
        Vector3 oldPos = tetrominoNode.transform.position;
        Quaternion oldRot = tetrominoNode.transform.rotation;
        
        tetrominoNode.transform.position += moveDir;
        if (isRotate)
        {
            tetrominoNode.rotation *= Quaternion.Euler(0, 0, 90);
        }
        
        if (!CanMoveTo(tetrominoNode))
        {
            tetrominoNode.transform.position = oldPos;
            tetrominoNode.transform.rotation = oldRot;
            
            if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && isRotate == false)
            {
                AddToBoard(tetrominoNode);
                CheckBoardColumn();
                CreateTetromino();
                
                if (!CanMoveTo(tetrominoNode))
                {
                    gameEnd = true;
                    GameEndOn?.Invoke(playerNo, scoreVal, levelVal, "lose");
                }
            }
            return false;
        }
        return true;
    }
    
    void CheckBoardColumn()
    {
        bool isCleared = false;
        
        int linecount = 0;
        
        foreach (Transform column in boardNode)
        {
            if (column.childCount == boardWidth)
            {
                foreach (Transform tile in column)
                {
                    Destroy((tile.gameObject));
                }
                column.DetachChildren();
                isCleared = true;
                linecount++;
            }
        }
        if (linecount != 0)
        {
            scoreVal += linecount * linecount * 100;
            playUI.UpdateScore(scoreVal);
        }
        
        if (linecount != 0)
        {
            lineVal -= linecount;
            playUI.UpdateLine(lineVal);
            if (lineVal <= 0 && levelVal < 10)
            {
                levelVal += 1;
                bombItem += 1;
                changeItem += 1;
                lineVal = levelVal * 2;
                fallCycle = 0.1f * (11 - levelVal/2);
                
                playUI.UpdateLevel(levelVal);
                playUI.UpdateBomb(bombItem);
                playUI.UpdateChange(changeItem);
                playUI.UpdateLine(lineVal);
            }
            else if (lineVal <= 0 && levelVal == 10)
            {
                gameEnd = true;
                GameEndOn?.Invoke(playerNo, scoreVal, levelVal, "win");
            }

        }
        
        if (isCleared)
        {
            for (int i = 1 + offset_y; i < boardNode.childCount; ++i)
            {
                var column = boardNode.Find(i.ToString());
            
                if (column.childCount == 0)
                    continue;
            
                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0 + offset_y)
                {
                    if (boardNode.Find(j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }
                    j--;
                }
                
                if (emptyCol > 0)
                {
                    var targetColumn = boardNode.Find((i - emptyCol).ToString());

                    while (column.childCount > 0)
                    {
                        Transform tile = column.GetChild(0);
                        tile.parent = targetColumn;
                        tile.transform.position += new Vector3(0, -emptyCol, 0);
                    }
                    column.DetachChildren();
                }
            }
        }
        
    }
    
    void AddToBoard(Transform root)
    {
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);
            
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth - offset_x);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1 - offset_y);
            
            node.parent = boardNode.Find((y + offset_y).ToString());
            node.name = x.ToString();
        }
    }
    
    bool CanMoveTo(Transform root)
    {
        for (int i = 0; i < root.childCount; ++i)
        {
            var node = root.GetChild(i);
            
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth - offset_x);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1 - offset_y);
            
            if (x < 0 || x > boardWidth - 1)
                return false;
            if (y < 0)
                return false;
            
            var column = boardNode.Find((y + offset_y).ToString());
            
            if (column != null && column.Find(x.ToString()) != null)
                return false;
        }
        return true;
    }
    
    private void clearBlock()
    {
        foreach (Transform child in tetrominoNode)
        {
            // 기존 테트로미노 타일 삭제
            Destroy(child.gameObject);
        }
        foreach (Transform child in previewNode)
        {
            // 기존 미리보기 타일 삭제
            Destroy(child.gameObject);
        }
        
    }
    
    void DestroyLineByItem()
    {
        var bottom = boardNode.Find((0 + offset_y).ToString());
        foreach (Transform tile in bottom) 
        {
            Destroy((tile.gameObject));
        }
        bottom.DetachChildren();
        
        bombEffect.Play();
        bombItem -= 1;
        playUI.UpdateBomb(bombItem);

        scoreVal += 100;
        playUI.UpdateScore(scoreVal);

        lineVal -= 1;
        playUI.UpdateLine(lineVal);
        if (lineVal <= 0 && levelVal < 10)
        {
            levelVal += 1;
            bombItem += 1;
            changeItem += 1;
            lineVal = levelVal * 2;
            fallCycle = 0.1f * (11 - levelVal/2);
            
            playUI.UpdateLevel(levelVal);
            playUI.UpdateBomb(bombItem);
            playUI.UpdateChange(changeItem);
            playUI.UpdateLine(lineVal);
        }
        else if (lineVal <= 0 && levelVal == 10)
        {
            gameEnd = true;
            GameEndOn?.Invoke(playerNo, scoreVal, levelVal, "win");
        }
        
        for (int i = 1 + offset_y; i < boardNode.childCount; ++i)
        {
            var column = boardNode.Find(i.ToString());
            
            if (column.childCount == 0)
                continue;
            
            var targetColumn = boardNode.Find((i - 1).ToString());
            while (column.childCount > 0)
            {
                Transform tile = column.GetChild(0);
                tile.parent = targetColumn;
                tile.transform.position += new Vector3(0, -1, 0);
            }
            column.DetachChildren();
        }
    }
    
    void ChangeNextByItem()
    {
        CreatePreview();
        changeEffect.Play();
        changeItem -= 1;
        playUI.UpdateChange(changeItem);
    }
    
}
