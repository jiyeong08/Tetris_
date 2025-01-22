using System;
using UnityEngine;

public class re_GamePlay : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform backgroundNode;
    [SerializeField] private Transform boardNode;
    [SerializeField] private Transform tetrominoNode;
    [SerializeField] private Transform previewNode;
    [SerializeField] private re_GameMap gameMap;
    [SerializeField] private re_Tetromino tetrominos;
    
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
    public event Action<int, string> GameEndOn;
    
    [Header("UI")]
    [SerializeField] private re_GamePlayUI playUI;
    [NonSerialized] public int scoreVal;
    [NonSerialized] public int levelVal;
    private int lineVal;
    
    [Header("Player Setting")]
    private int players;
    private int playerNo;
    private re_GameKey gameKey;
    
    [Header("Item")]
    [SerializeField] private GameObject items;
    private bool itemMode;
    [SerializeField] private AudioSource bombEffect;
    private int bombItem;
    [SerializeField] private AudioSource changeEffect;
    private int changeItem;
    
    public void GameStart(int players, int playerNo, bool itemMode, re_GameKey gameKey)
    {
        this.players = players;
        this.playerNo = playerNo;
        this.itemMode = itemMode;
        this.gameKey = gameKey;
        
        gameEnd = false;
        
        scoreVal = 0;
        levelVal = 1;
        lineVal = levelVal * 2;
        playUI.UpdateLevel(levelVal);
        playUI.UpdateScore(scoreVal);
        playUI.UpdateLine(lineVal);
        
        if (this.itemMode)
        {
            items.SetActive(true);
            bombItem = 3;
            changeItem = 0;
            playUI.UpdateBomb(bombItem);
            playUI.UpdateChange(changeItem);
        }
        else items.SetActive(false);
        
        fallCycle = 1.0f;
        nextFallTime = Time.time + fallCycle;
        
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);
        
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
        
        clearBlock();
        gameMap.CreateMap(this.players, this.playerNo, offset_x, offset_y); // 맵 형성
        tetrominos.CreateTetromino(this.players, this.playerNo, offset_x); // 테트로미노 형성
    }

    void Update()
    {
        if (gameEnd) { }
        else
        {
            Vector3 moveDir = Vector3.zero;
            bool isRotate = false;
            
            if (gameKey.leftKey()) 
            { 
                moveDir.x = -1;
            }
            else if (gameKey.rightKey())
            {
                moveDir.x = 1;
            }

            if (gameKey.rotateKey())
            {
                if (tetrominos.index != 3) isRotate = true;
            }
            else if (gameKey.downKey())
            {
                moveDir.y = -1;
            }

            if (gameKey.dropKey())
            {
                while (moveTetromino(Vector3.down, false))
                {
                    
                }
                    
            }

            if (itemMode)
            {
                if (gameKey.bombKey() && bombItem > 0)
                { 
                    destroyLineByItem();
                }
                if (gameKey.changeKey() && changeItem > 0) 
                { 
                    changeNextByItem();
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
                moveTetromino(moveDir, isRotate);
            }
        }
    }
    
    private bool moveTetromino(Vector3 moveDir, bool isRotate)
    {
        Vector3 oldPos = tetrominoNode.transform.position;
        Quaternion oldRot = tetrominoNode.transform.rotation;
        
        tetrominoNode.transform.position += moveDir;
        if (isRotate)
        {
            tetrominoNode.rotation *= Quaternion.Euler(0, 0, 90);
        }
        
        if (!canMoveTo(tetrominoNode))
        {
            tetrominoNode.transform.position = oldPos;
            tetrominoNode.transform.rotation = oldRot;
            
            if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && isRotate == false)
            {
                addToBoard(tetrominoNode);
                checkBoardColumn();
                tetrominos.CreateTetromino(players, playerNo, offset_x);
                
                if (!canMoveTo(tetrominoNode))
                {
                    gameEnd = true;
                    GameEndOn?.Invoke(playerNo, "lose");
                }
            }
            return false;
        }
        return true;
    }
    
    private void checkBoardColumn()
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
                GameEndOn?.Invoke(playerNo, "win");
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
    
    private void addToBoard(Transform root)
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
    
    private bool canMoveTo(Transform root)
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
            Destroy(child.gameObject);
        }
        foreach (Transform child in previewNode)
        {
            Destroy(child.gameObject);
        }
    }
    
    private void destroyLineByItem()
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
            GameEndOn?.Invoke(playerNo, "win");
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
    
    private void changeNextByItem()
    {
        tetrominos.CreatePreview(players, playerNo);
        changeEffect.Play();
        changeItem -= 1;
        playUI.UpdateChange(changeItem);
    }
}
