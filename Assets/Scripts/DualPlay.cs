using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DualPlay : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform backgroundNode1;
    [SerializeField] private Transform backgroundNode2;
    [SerializeField] private Transform boardNode1;
    [SerializeField] private Transform boardNode2;
    [SerializeField] private Transform tetrominoNode1;
    [SerializeField] private Transform tetrominoNode2;
    [SerializeField] private Transform previewNode1;
    [SerializeField] private Transform previewNode2;
    
    [Header("Setting")]
    [Range(4, 40)]
    private int boardWidth = 10;
    [Range(5, 20)]
    private int boardHeight = 20;
    private float fallCycle1;
    private float fallCycle2;
    private float offset_x = 0f;
    private float offset_y = 0f;
    
    private int halfWidth;
    private int halfHeight;

    private float nextFallTime1;
    private float nextFallTime2;

    private bool gameEnd;
    public event Action<string, int, int, int, int> GameEndOn;
    
    // [SerializeField] private UIManager gameplay;
    [SerializeField] private UIManager player1;
    [SerializeField] private UIManager player2;
    private int scoreVal1;
    private int levelVal1;
    private int lineVal1;
    private int scoreVal2;
    private int levelVal2;
    private int lineVal2;
    
    private int indexVal1;
    private int index1;
    private int indexVal2;
    private int index2;
    
    // Item
    private bool itemMode;
    [SerializeField] private GameObject items;
    private int bombItem1;
    private int bombItem2;
    [SerializeField] private AudioSource bombEffect;
    private int changeItem1;
    private int changeItem2;
    [SerializeField] private AudioSource changeEffect;
    
    public void GameStart(bool itemMode)
    {
        gameEnd = false;
        
        fallCycle1 = 1.0f;
        scoreVal1 = 0;
        levelVal1 = 1;
        indexVal1 =- 1;
        fallCycle2 = 1.0f;
        scoreVal2 = 0;
        levelVal2 = 1;
        indexVal2 =- 1;
        
        this.itemMode = itemMode;
        if (this.itemMode)
        {
            items.SetActive(true);
            bombItem1 = 3;
            bombItem2 = 3;
            bombEffect.clip = Resources.Load<AudioClip>("Audio/bomb");
            changeItem1 = 0;
            changeItem2 = 0;
            changeEffect.clip = Resources.Load<AudioClip>("Audio/change");
            player1.UpdateBomb(bombItem1);
            player2.UpdateBomb(bombItem2);
            player1.UpdateChange(changeItem1);
            player2.UpdateChange(changeItem2);
        }
        else items.SetActive(false);
        
        lineVal1 = levelVal1 * 2;
        player1.UpdateLevel(levelVal1);
        player1.UpdateScore(scoreVal1);
        player1.UpdateLine(lineVal1);
        lineVal2 = levelVal2 * 2;
        player2.UpdateLevel(levelVal2);
        player2.UpdateScore(scoreVal2);
        player2.UpdateLine(lineVal2);
        
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);

        nextFallTime1 = Time.time + fallCycle1;
        nextFallTime2 = Time.time + fallCycle2;

        CreateBackground();
        
        for (int i = 0; i < boardHeight; ++i)
        {
            // Player 1
            var col1 = new GameObject((boardHeight - i - 1).ToString());
            col1.transform.position = new Vector3(0, halfHeight - i, 0);
            col1.transform.parent = boardNode1;
            // Player 2
            var col2 = new GameObject((boardHeight - i - 1).ToString());
            col2.transform.position = new Vector3(0, halfHeight - i, 0);
            col2.transform.parent = boardNode2;
        }
        
        CreateTetromino1();
        CreateTetromino2();
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
        // Player 1
        for (int x = -halfWidth - 6; x < halfWidth - 6; ++x)
        {
            for (int y = halfHeight; y > -halfHeight; --y)
            {
                CreateTile(backgroundNode1, new Vector2(x, y), color, 0);
            }
        }
        // Player2
        for (int x = -halfWidth + 7; x < halfWidth + 7; ++x)
        {
            for (int y = halfHeight; y > -halfHeight; --y)
            {
                CreateTile(backgroundNode2, new Vector2(x, y), color, 0);
            }
        }
        
        color.a = 1.0f;
        // Player 1
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode1, new Vector2(-halfWidth - 7, y), color, 0);
            CreateTile(backgroundNode1, new Vector2(halfWidth - 6, y), color, 0);
        }
        // Player 2
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode2, new Vector2(-halfWidth + 6, y), color, 0);
            CreateTile(backgroundNode2, new Vector2(halfWidth + 7, y), color, 0);
        }
        
        // Player 1
        for (int x = -halfWidth - 7; x <= halfWidth - 6; ++x)
        {
            CreateTile(backgroundNode1, new Vector2(x, -halfHeight), color, 0);
        }
        // Player 2
        for (int x = -halfWidth + 6; x <= halfWidth + 7; ++x)
        {
            CreateTile(backgroundNode2, new Vector2(x, -halfHeight), color, 0);
        }
        
    }
    
    // Player 1
    void CreateTetromino1()
    {
        if (indexVal1 == -1)
        {
            index1 = Random.Range(0, 7);
        }
        else index1 = indexVal1;

        Color32 color = Color.white;
        
        tetrominoNode1.rotation = Quaternion.identity;
        tetrominoNode1.position = new Vector2(offset_x - 6, halfHeight + offset_y);
        
        switch (index1)
        {
            case 0: // I
                color = Color.cyan;
                CreateTile(tetrominoNode1, new Vector2(-2f, 0.0f), color);
                CreateTile(tetrominoNode1, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, 0.0f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, 0.0f), color);
                break;
            case 1: // J
                color = Color.blue;
                CreateTile(tetrominoNode1, new Vector2(-1f, -1.0f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, -1.0f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, -1.0f), color);
                CreateTile(tetrominoNode1, new Vector2(-1f, 0.0f), color);
                break;
            case 2: // L
                color = Color.black;
                CreateTile(tetrominoNode1, new Vector2(-1f, -1.0f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, -1.0f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, -1.0f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, 0.0f), color);
                break;
            case 3: // O
                color = Color.yellow;
                CreateTile(tetrominoNode1, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, -1f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, 0f), color);
                break;
            case 4: // S
                color = Color.green;
                CreateTile(tetrominoNode1, new Vector2(-1f, -1f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, 0f), color);
                break;
            case 5: // T
                color = Color.magenta;
                CreateTile(tetrominoNode1, new Vector2(-1f, -1f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, -1f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, 0f), color);
                break;
            case 6: // Z
                color = Color.red;
                CreateTile(tetrominoNode1, new Vector2(-1f, 0f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode1, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode1, new Vector2(1f, -1f), color);
                break;
        }
        CreatePreview1();
    }
    // Player 2
    void CreateTetromino2()
    {
        if (indexVal2 == -1)
        {
            index2 = Random.Range(0, 7);
        }
        else index2 = indexVal2;

        Color32 color = Color.white;
        
        tetrominoNode2.rotation = Quaternion.identity;
        tetrominoNode2.position = new Vector2(offset_x + 6, halfHeight + offset_y);
        
        switch (index2)
        {
            case 0: // I
                color = Color.cyan;
                CreateTile(tetrominoNode2, new Vector2(-2f, 0.0f), color);
                CreateTile(tetrominoNode2, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, 0.0f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, 0.0f), color);
                break;
            case 1: // J
                color = Color.blue;
                CreateTile(tetrominoNode2, new Vector2(-1f, -1.0f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, -1.0f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, -1.0f), color);
                CreateTile(tetrominoNode2, new Vector2(-1f, 0.0f), color);
                break;
            case 2: // L
                color = Color.black;
                CreateTile(tetrominoNode2, new Vector2(-1f, -1.0f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, -1.0f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, -1.0f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, 0.0f), color);
                break;
            case 3: // O
                color = Color.yellow;
                CreateTile(tetrominoNode2, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, -1f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, 0f), color);
                break;
            case 4: // S
                color = Color.green;
                CreateTile(tetrominoNode2, new Vector2(-1f, -1f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, 0f), color);
                break;
            case 5: // T
                color = Color.magenta;
                CreateTile(tetrominoNode2, new Vector2(-1f, -1f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, -1f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, 0f), color);
                break;
            case 6: // Z
                color = Color.red;
                CreateTile(tetrominoNode2, new Vector2(-1f, 0f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode2, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode2, new Vector2(1f, -1f), color);
                break;
        }
        CreatePreview2();
    }
    
    // Player 1
    void CreatePreview1()
    {
        foreach (Transform tile in previewNode1)
        {
            Destroy(tile.gameObject);
        }
        previewNode1.DetachChildren();
        
        var nextIndex = Random.Range(0, 7);
        while (nextIndex == indexVal1)
        {
            nextIndex = Random.Range(0, 7);
        }
        indexVal1 = nextIndex;
        
        previewNode1.position = new Vector2(-(halfWidth + 10), halfHeight - 9);
        Color32 color = Color.white;

        switch (indexVal1)
        {
            case 0: // I
                color = Color.cyan;
                CreateTile(previewNode1, new Vector2(-2f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(1f, 0.0f), color);
                break;
            case 1: // J
                color = Color.blue;
                CreateTile(previewNode1, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(1f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(-1f, 1.0f), color);
                break;
            case 2: // L
                color = Color.black;
                CreateTile(previewNode1, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(1f, 0.0f), color);
                CreateTile(previewNode1, new Vector2(1f, 1.0f), color);
                break;
            case 3: // O
                color = Color.yellow;
                CreateTile(previewNode1, new Vector2(0f, 0f), color);
                CreateTile(previewNode1, new Vector2(1f, 0f), color);
                CreateTile(previewNode1, new Vector2(0f, 1f), color);
                CreateTile(previewNode1, new Vector2(1f, 1f), color);
                break;
            case 4: // S
                color = Color.green;
                CreateTile(previewNode1, new Vector2(-1f, 0f), color);
                CreateTile(previewNode1, new Vector2(0f, 0f), color);
                CreateTile(previewNode1, new Vector2(0f, 1f), color);
                CreateTile(previewNode1, new Vector2(1f, 1f), color);
                break;
            case 5: // T
                color = Color.magenta;
                CreateTile(previewNode1, new Vector2(-1f, 0f), color);
                CreateTile(previewNode1, new Vector2(0f, 0f), color);
                CreateTile(previewNode1, new Vector2(1f, 0f), color);
                CreateTile(previewNode1, new Vector2(0f, 1f), color);
                break;
            case 6: // Z
                color = Color.red;
                CreateTile(previewNode1, new Vector2(-1f, 1f), color);
                CreateTile(previewNode1, new Vector2(0f, 1f), color);
                CreateTile(previewNode1, new Vector2(0f, 0f), color);
                CreateTile(previewNode1, new Vector2(1f, 0f), color);
                break;
        }
    }
    // Player 2
    void CreatePreview2()
    {
        foreach (Transform tile in previewNode2)
        {
            Destroy(tile.gameObject);
        }
        previewNode2.DetachChildren();
        
        var nextIndex = Random.Range(0, 7);
        while (nextIndex == indexVal2)
        {
            nextIndex = Random.Range(0, 7);
        }
        indexVal2 = nextIndex;
        
        previewNode2.position = new Vector2(halfWidth + 10, halfHeight - 9);
        Color32 color = Color.white;
        
        switch (indexVal2)
        {
            case 0: // I
                color = Color.cyan;
                CreateTile(previewNode2, new Vector2(-2f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(1f, 0.0f), color);
                break;
            case 1: // J
                color = Color.blue;
                CreateTile(previewNode2, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(1f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(-1f, 1.0f), color);
                break;
            case 2: // L
                color = Color.black;
                CreateTile(previewNode2, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(1f, 0.0f), color);
                CreateTile(previewNode2, new Vector2(1f, 1.0f), color);
                break;
            case 3: // O
                color = Color.yellow;
                CreateTile(previewNode2, new Vector2(0f, 0f), color);
                CreateTile(previewNode2, new Vector2(1f, 0f), color);
                CreateTile(previewNode2, new Vector2(0f, 1f), color);
                CreateTile(previewNode2, new Vector2(1f, 1f), color);
                break;
            case 4: // S
                color = Color.green;
                CreateTile(previewNode2, new Vector2(-1f, 0f), color);
                CreateTile(previewNode2, new Vector2(0f, 0f), color);
                CreateTile(previewNode2, new Vector2(0f, 1f), color);
                CreateTile(previewNode2, new Vector2(1f, 1f), color);
                break;
            case 5: // T
                color = Color.magenta;
                CreateTile(previewNode2, new Vector2(-1f, 0f), color);
                CreateTile(previewNode2, new Vector2(0f, 0f), color);
                CreateTile(previewNode2, new Vector2(1f, 0f), color);
                CreateTile(previewNode2, new Vector2(0f, 1f), color);
                break;
            case 6: // Z
                color = Color.red;
                CreateTile(previewNode2, new Vector2(-1f, 1f), color);
                CreateTile(previewNode2, new Vector2(0f, 1f), color);
                CreateTile(previewNode2, new Vector2(0f, 0f), color);
                CreateTile(previewNode2, new Vector2(1f, 0f), color);
                break;
        }
    }

    void Update()
    {
        //게임 오버 처리
        if (gameEnd) { }
        // 게임 처리
        else
        {
            // Player 1
            Vector3 moveDir1 = Vector3.zero;
            bool isRotate1 = false;
            if (Input.GetKeyDown("a"))
            {
                moveDir1.x = -1;
            }
            else if (Input.GetKeyDown("d"))
            {
                moveDir1.x = 1;
            }
            if (Input.GetKeyDown("w"))
            {
                if(index1 != 3) isRotate1 = true;
            }
            else if (Input.GetKeyDown("s"))
            {
                moveDir1.y = -1;
            }
            if (Input.GetKeyDown("space"))
            {
                while (MoveTetromino1(Vector3.down, false))
                {
                
                }
            }

            if (itemMode)
            {
                if (Input.GetKeyDown("v"))
                {
                    if (bombItem1 > 0) DestroyLineByItem1();
                }

                if (Input.GetKeyDown("b"))
                {
                    if (changeItem1 > 0) ChangeNextByItem1();
                }
            }
            if (Time.time > nextFallTime1)
            {
                nextFallTime1 = Time.time + fallCycle1;
                moveDir1.y = -1;
                isRotate1 = false;
            }
            if (moveDir1 != Vector3.zero || isRotate1)
            {
                MoveTetromino1(moveDir1, isRotate1);
            }
            // Player 2
            Vector3 moveDir2 = Vector3.zero;
            bool isRotate2 = false;
            if (Input.GetKeyDown("left"))
            {
                moveDir2.x = -1;
            }
            else if (Input.GetKeyDown("right"))
            {
                moveDir2.x = 1;
            }
            if (Input.GetKeyDown("up"))
            {
                if(index2 != 3) isRotate2 = true;
            }
            else if (Input.GetKeyDown("down"))
            {
                moveDir2.y = -1;
            }
            if (Input.GetKeyDown("/"))
            {
                while (MoveTetromino2(Vector3.down, false))
                {
                }
            }
            if (itemMode)
            {
                if (Input.GetKeyDown(";"))
                {
                    if (bombItem2 > 0) DestroyLineByItem2();
                }

                if (Input.GetKeyDown("'"))
                {
                    if (changeItem2 > 0) ChangeNextByItem2();
                }
            }
            if (Time.time > nextFallTime2)
            {
                nextFallTime2 = Time.time + fallCycle2;
                moveDir2.y = -1;
                isRotate2 = false;
            }
            if (moveDir2 != Vector3.zero || isRotate2)
            {
                MoveTetromino2(moveDir2, isRotate2);
            }
        }
    }
    
    // Player 1
    bool MoveTetromino1(Vector3 moveDir, bool isRotate)
    {
        Vector3 oldPos = tetrominoNode1.transform.position;
        Quaternion oldRot = tetrominoNode1.transform.rotation;
        
        tetrominoNode1.transform.position += moveDir;
        if (isRotate)
        {
            tetrominoNode1.rotation *= Quaternion.Euler(0, 0, 90);
        }
        
        if (!CanMoveTo1(tetrominoNode1))
        {
            tetrominoNode1.transform.position = oldPos;
            tetrominoNode1.transform.rotation = oldRot;
            
            if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && isRotate == false)
            {
                AddToBoard1(tetrominoNode1);
                CheckBoardColumn1();
                CreateTetromino1();
                
                if (!CanMoveTo1(tetrominoNode1))
                {
                    clearMap();
                    gameEnd = true;
                    GameEndOn?.Invoke("player2", levelVal1, scoreVal1, levelVal2, scoreVal2);
                }
            }
            return false;
        }
        return true;
    }
    // Player 2
    bool MoveTetromino2(Vector3 moveDir, bool isRotate)
    {
        Vector3 oldPos = tetrominoNode2.transform.position;
        Quaternion oldRot = tetrominoNode2.transform.rotation;

        tetrominoNode2.transform.position += moveDir;
        if (isRotate)
        {
            tetrominoNode2.rotation *= Quaternion.Euler(0, 0, 90);
        }

        if (!CanMoveTo2(tetrominoNode2))
        {
            tetrominoNode2.transform.position = oldPos;
            tetrominoNode2.transform.rotation = oldRot;

            if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && isRotate == false)
            {
                AddToBoard2(tetrominoNode2);
                CheckBoardColumn2();
                CreateTetromino2();

                if (!CanMoveTo2(tetrominoNode2))
                {
                    clearMap();
                    gameEnd = true;
                    GameEndOn?.Invoke("player1", levelVal1, scoreVal1, levelVal2, scoreVal2);
                }
            }
            return false;
        }
        return true;
    }
    
    // Player 1
    void CheckBoardColumn1()
    {
        bool isCleared = false;
        
        int linecount = 0;
        
        foreach (Transform column in boardNode1)
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
            scoreVal1 += linecount * linecount * 100;
            player1.UpdateScore(scoreVal1);
        }
        
        if (linecount != 0)
        {
            lineVal1 -= linecount;
            player1.UpdateLine(lineVal1);
            if (lineVal1 <= 0 && levelVal1 < 10)
            {
                levelVal1 += 1;
                bombItem1 += 1;
                changeItem1 += 1;
                lineVal1 = levelVal1 * 2;
                fallCycle1 = 0.1f * (11 - levelVal1);
                
                player1.UpdateLevel(levelVal1);
                player1.UpdateBomb(bombItem1);
                player1.UpdateChange(changeItem1);
                player1.UpdateLine(lineVal1);
            }
            else if (lineVal1 <= 0 && levelVal1 == 10)
            {
                clearMap();
                gameEnd = true;
                GameEndOn?.Invoke("player1", levelVal1, scoreVal1, levelVal2, scoreVal2);
            }

        }
        
        if (isCleared)
        {
            for (int i = 1; i < boardNode1.childCount; ++i)
            {
                var column = boardNode1.Find(i.ToString());
            
                if (column.childCount == 0)
                    continue;
            
                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0)
                {
                    if (boardNode1.Find(j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }
                    j--;
                }
                
                if (emptyCol > 0)
                {
                    var targetColumn = boardNode1.Find((i - emptyCol).ToString());

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
    // Player 2
    void CheckBoardColumn2()
    {
        bool isCleared = false;

        int linecount = 0;

        foreach (Transform column in boardNode2)
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
            scoreVal2 += linecount * linecount * 100;
            player2.UpdateScore(scoreVal2);
        }

        if (linecount != 0)
        {
            lineVal2 -= linecount;
            player2.UpdateLine(lineVal2);
            if (lineVal2 <= 0 && levelVal2 < 10)
            {
                levelVal2 += 1;
                bombItem2 += 1;
                changeItem2 += 1;
                lineVal2 = levelVal2 * 2;
                fallCycle2 = 0.1f * (11 - levelVal2);
                
                player2.UpdateLevel(levelVal2);
                player2.UpdateBomb(bombItem2);
                player2.UpdateChange(changeItem2);
                player2.UpdateLine(lineVal2);
            }
            else if (lineVal2 <= 0 && levelVal2 == 10)
            {
                clearMap();
                gameEnd = true;
                GameEndOn?.Invoke("player2", levelVal1, scoreVal1, levelVal2, scoreVal2);
            }

        }

        if (isCleared)
        {
            for (int i = 1; i < boardNode2.childCount; ++i)
            {
                var column = boardNode2.Find(i.ToString());

                if (column.childCount == 0)
                    continue;

                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0)
                {
                    if (boardNode2.Find(j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }

                    j--;
                }

                if (emptyCol > 0)
                {
                    var targetColumn = boardNode2.Find((i - emptyCol).ToString());

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
    
    // Player 1
    void AddToBoard1(Transform root)
    {
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);
            
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth + 6);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);
            
            node.parent = boardNode1.Find(y.ToString());
            node.name = x.ToString();
        }
    }
    // Player 2
    void AddToBoard2(Transform root)
    {
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);

            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth - 7);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            node.parent = boardNode2.Find(y.ToString());
            node.name = x.ToString();
        }
    }
    
    // Player 1
    bool CanMoveTo1(Transform root)
    {
        for (int i = 0; i < root.childCount; ++i)
        {
            var node = root.GetChild(i);
            
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth + 6);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);
            
            if (x < 0 || x > boardWidth - 1)
                return false;
            if (y < 0)
                return false;
            
            var column = boardNode1.Find(y.ToString());
            
            if (column != null && column.Find(x.ToString()) != null)
                return false;
        }
        return true;
    }
    // Player 2
    bool CanMoveTo2(Transform root)
    {
        for (int i = 0; i < root.childCount; ++i)
        {
            var node = root.GetChild(i);

            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth - 7);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            if (x < 0 || x > boardWidth - 1)
                return false;
            if (y < 0)
                return false;

            var column = boardNode2.Find(y.ToString());

            if (column != null && column.Find(x.ToString()) != null)
                return false;
        }

        return true;
    }
    
    private void clearMap()
    {
        
        // Player 1
        foreach (Transform child in boardNode1)
        {
            // 기존 타일 삭제
            Destroy(child.gameObject);
        }
        foreach (Transform child in tetrominoNode1)
        {
            // 기존 테트로미노 타일 삭제
            Destroy(child.gameObject);
        }
        foreach (Transform child in previewNode1)
        {
            // 기존 미리보기 타일 삭제
            Destroy(child.gameObject);
        }
        foreach (Transform child in backgroundNode1)
        {
            // 기존 배경 타일 삭제
            Destroy(child.gameObject);
        }
        
        // Player 2
        foreach (Transform child in boardNode2)
        {
            // 기존 타일 삭제
            Destroy(child.gameObject);
        }
        foreach (Transform child in tetrominoNode2)
        {
            // 기존 테트로미노 타일 삭제
            Destroy(child.gameObject);
        }
        foreach (Transform child in previewNode2)
        {
            // 기존 미리보기 타일 삭제
            Destroy(child.gameObject);
        }
        foreach (Transform child in backgroundNode2)
        {
            // 기존 배경 타일 삭제
            Destroy(child.gameObject);
        }
        
    }
    
    void DestroyLineByItem1()
    {
        var bottom = boardNode1.Find("0");
        foreach (Transform tile in bottom) 
        {
            Destroy((tile.gameObject));
        }
        bottom.DetachChildren();
        
        bombEffect.Play();
        bombItem1 -= 1;
        player1.UpdateBomb(bombItem1);

        scoreVal1 += 100;
        player1.UpdateScore(scoreVal1);

        lineVal1 -= 1;
        player1.UpdateLine(lineVal1);
        if (lineVal1 <= 0 && levelVal1 < 10)
        {
            levelVal1 += 1;
            bombItem1 += 1;
            changeItem1 += 1;
            lineVal1 = levelVal1 * 2;
            fallCycle1 = 0.1f * (10 - levelVal1);
            
            player1.UpdateLevel(levelVal1);
            player1.UpdateBomb(bombItem1);
            player1.UpdateChange(changeItem1);
            player1.UpdateLine(lineVal1);
        }
        else if (lineVal1 <= 0 && levelVal1 == 10)
        {
            clearMap();
            gameEnd = true;
            GameEndOn?.Invoke("player1", levelVal1, scoreVal1, levelVal2, scoreVal2);
        }
        
        for (int i = 1; i < boardNode1.childCount; ++i)
        {
            var column = boardNode1.Find(i.ToString());
            
            if (column.childCount == 0)
                continue;
            
            var targetColumn = boardNode1.Find((i - 1).ToString());
            while (column.childCount > 0)
            {
                Transform tile = column.GetChild(0);
                tile.parent = targetColumn;
                tile.transform.position += new Vector3(0, -1, 0);
            }
            column.DetachChildren();
        }
    }
    
    void DestroyLineByItem2()
    {
        var bottom = boardNode2.Find("0");
        foreach (Transform tile in bottom) 
        {
            Destroy((tile.gameObject));
        }
        bottom.DetachChildren();
        
        bombEffect.Play();
        bombItem2 -= 1;
        player2.UpdateBomb(bombItem2);

        scoreVal2 += 100;
        player2.UpdateScore(scoreVal2);

        lineVal2 -= 1;
        player2.UpdateLine(lineVal2);
        if (lineVal2 <= 0 && levelVal2 < 10)
        {
            levelVal2 += 1;
            bombItem2 += 1;
            changeItem2 += 1;
            lineVal2 = levelVal2 * 2;
            fallCycle2 = 0.1f * (10 - levelVal2);
            
            player2.UpdateLevel(levelVal2);
            player2.UpdateBomb(bombItem2);
            player2.UpdateChange(changeItem2);
            player2.UpdateLine(lineVal2);
        }
        else if (lineVal2 <= 0 && levelVal2 == 10)
        {
            clearMap();
            gameEnd = true;
            GameEndOn?.Invoke("player2", levelVal1, scoreVal1, levelVal2, scoreVal2);
        }
        
        for (int i = 1; i < boardNode2.childCount; ++i)
        {
            var column = boardNode2.Find(i.ToString());
            
            if (column.childCount == 0)
                continue;
            
            var targetColumn = boardNode2.Find((i - 1).ToString());
            while (column.childCount > 0)
            {
                Transform tile = column.GetChild(0);
                tile.parent = targetColumn;
                tile.transform.position += new Vector3(0, -1, 0);
            }
            column.DetachChildren();
        }
    }
    
    void ChangeNextByItem1()
    {
        CreatePreview1();
        changeEffect.Play();
        changeItem1 -= 1;
        player1.UpdateChange(changeItem1);
    }
    
    void ChangeNextByItem2()
    {
        CreatePreview2();
        changeEffect.Play();
        changeItem2 -= 1;
        player2.UpdateChange(changeItem2);
    }
}
