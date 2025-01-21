using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoloPlay : MonoBehaviour
{
    // 필요 소스 불러오기
    [Header("Source")]
    /*
     * GameObject
     * Prefab 오브젝트를 불러와서 관리 가능
     */
    [SerializeField] private GameObject tilePrefab;
    /*
     * Transform
     * 게임 오브젝트가 가지는 기본 컴포넌트
     * 위치, 회전, 크기 정보를 담고 있으며, 부모관계를 설정, 관리할 수 있음
     */
    [SerializeField] private Transform backgroundNode;
    [SerializeField] private Transform boardNode;
    [SerializeField] private Transform tetrominoNode;
    [SerializeField] private Transform previewNode;
    
    /*
     * [Header("Title")]
     * 그룹 이름을 지어 구분짓기를 가능하게 함
     * 유니티 에디터 변수들이 많아질 경우 관리하기 편리
     */
    [Header("Setting")]
    /*
     * [Range(a,b)]
     * 사용자에게 입력받을 값의 범위를 정해주어 오류를 방지하는 기능
     */
    [Range(4, 40)]
    private int boardWidth = 10;
    // 높이 설정
    [Range(5, 20)]
    private int boardHeight = 20;
    // 떨어지는 속도
    private float fallCycle;
    // 위치 보정
    private float offset_x = 0f;
    private float offset_y = 0f;
    
    private int halfWidth;
    private int halfHeight;

    private float nextFallTime;

    private bool gameOver;
    private bool gameClear;
    public event Action<int, int> GameOverOn;
    public event Action<int> GameClearOn;

    // UI 관련 변수
    [SerializeField] private UIManager gameplay;
    private int scoreVal;
    private int levelVal;
    private int lineVal;

    private int indexVal;
    private int index;

    // Item
    private bool itemMode;
    [SerializeField] private GameObject items;
    private int bombItem;
    [SerializeField] private AudioSource bombEffect;
    private int changeItem;
    [SerializeField] private AudioSource changeEffect;

    public void GameStart(bool itemMode)
    {
        gameClear = false;
        gameOver = false;
        
        fallCycle = 1.0f;
        
        scoreVal = 0;
        levelVal = 1;
        indexVal =- 1;

        this.itemMode = itemMode;
        if (this.itemMode)
        {
            items.SetActive(true);
            bombItem = 3;
            bombEffect.clip = Resources.Load<AudioClip>("Audio/bomb");
            changeItem = 0;
            changeEffect.clip = Resources.Load<AudioClip>("Audio/change");
            gameplay.UpdateBomb(bombItem);
            gameplay.UpdateChange(changeItem);
        }
        else items.SetActive(false);

        //게임 시작 시 text 설정
        lineVal = levelVal * 2;
        gameplay.UpdateScore(scoreVal);
        gameplay.UpdateLevel(levelVal);
        gameplay.UpdateLine(lineVal);
        
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);    // 너비의 중간값 설정
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);  // 높이의 중간값 설정

        nextFallTime = Time.time + fallCycle;   // 다음에 테트로미노가 떨어질 시간 설정

        CreateBackground(); // 배경 만들기
        CreateBoardColumn(); // 높이만큼 행 노드 만들어주기
        CreateTetromino();  // 테트로미노 만들기
    }
    
    // 타일 생성
    Tile CreateTile(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab);   // tilePrefab을 복제한 오브젝트 생성
        go.transform.parent = parent;   // 부모 지정
        go.transform.localPosition = position;  // 위치 지정
        
        var tile = go.GetComponent<Tile>(); // tilePrefab의 Tile 스크립트 불러오기
        tile.color = color;
        tile.sortingOrder = order;
        
        return tile;
    }

    // 배경 타일을 생성 -> 테두리 및 배경 타일 생성 시 위치 범위 잘 생각해주면서 for문 범위 설정
    void CreateBackground()
    {
        Color color = Color.gray;   // 원하는 배경 색 설정
        
        // 타일 보드
        color.a = 0.5f; // 테두리와 구분 위헤 투명도 바꾸기
        for (int x = -halfWidth; x < halfWidth; ++x)
        {
            for (int y = halfHeight; y > -halfHeight; --y)
            {
                CreateTile(backgroundNode, new Vector2(x, y), color, 0);
            }
        }
        
        // 좌우 테두리
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode, new Vector2(-halfWidth - 1, y), color, 0);
            CreateTile(backgroundNode, new Vector2(halfWidth, y), color, 0);
        }
        
        // 아래 테두리
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            CreateTile(backgroundNode, new Vector2(x, -halfHeight), color, 0);
        }
    }

    // 높이만틈 행 노드 만들어주기
    void CreateBoardColumn()
    {
        for (int i = 0; i < boardHeight; ++i)
        {
            // ToString을 이용하여 오브젝트 이름 설정
            var col = new GameObject((boardHeight - i - 1).ToString());
            // 위치 설정 -> 행 위치의 높이, 가로 중앙
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            // 보드 노드의 자식으로 설정
            col.transform.parent = boardNode;
        }
    }
    
    // 테트로미노 생성
    void CreateTetromino()
    {
        //제일 처음에 나오는 테트로미노인 경우(미리보기 값이 없는 경우)
        if (indexVal == -1)
        {
            index = Random.Range(0, 7); // 랜덤으로 0~6 사이의 값 생성
        }
        
        // 미리보기의 index 값이 0~6 사이의 값이 결정된 경우(preview의 값 가져오기)
        else index = indexVal;

        Color32 color = Color.white;
        
        // 회전 계산에 사용하기 위한 쿼터니언 클래스
        tetrominoNode.rotation = Quaternion.identity;
        // 테트로미노 생성 위치 (중앙 상단), 값 보정 적용
        tetrominoNode.position = new Vector2(offset_x, halfHeight + offset_y);

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
                CreateTile(tetrominoNode, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(1f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), color);
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

    /*
     * if
     * 직관적이므로 이해하기 쉬움
     * 입력하는 키의 종류가 늘어남에 따라 코드의 양이 늘어나며 중복이 심해짐
     *
     * switch
     * 대, 소문자 구분이 가능
     * 코드의 양 많음
     *
     * Dictionary, delegate
     * 입력하는 키의 종류가 늘어나도 코드의 증가 없음
     * 코드가 복잡하고 활용이 어려움
     */
    // Update is called once per frame
    void Update()
    {
        //게임 오버 처리
        if (gameOver || gameClear) { }
        // 게임 처리
        else
        {
            // 초기화
            Vector3 moveDir = Vector3.zero; // 이동 여부 저장용
            bool isRotate = false;  // 회전 여부 저장용
        
            // 각 키에 따라 이동 여부 혹은 회전 여부 설정
            if (Input.GetKeyDown("left"))
            {
                moveDir.x = -1;
            }
            else if (Input.GetKeyDown("right"))
            {
                moveDir.x = 1;
            }

            if (Input.GetKeyDown("up"))
            {
                if(index != 3) isRotate = true;
            }
            else if (Input.GetKeyDown("down"))
            {
                moveDir.y = -1;
            }

            if (Input.GetKeyDown("space"))
            {
                // 테트로미노가 바닥에 닿을 때까지 아래로 이동
                while (MoveTetromino(Vector3.down, false))
                {
                
                }
            }

            if(itemMode)
            {
                if (Input.GetKeyDown("z"))
                {
                    if (bombItem > 0) DestroyLineByItem();
                }

                if (Input.GetKeyDown("x"))
                {
                    if (changeItem > 0) ChangeNextByItem();
                }
            }
        
            /*
             * Time.deltaTime
             * 프레임이 시작하고 끝나는 시간
             * 1초당 프레임을 규격화하기 위해 사용
             *
             * Time.time
             * 선언된 시점에서 카운트가 시작되고 그 시간값이 저장(초단위)
             * 시간을 측정하기 위해 사용
             */
            // 아래로 떨어지는 경우는 강제로 이동
            if (Time.time > nextFallTime)
            {
                nextFallTime = Time.time + fallCycle;   // 다음 떨어질 시간 재설정
                moveDir.y = -1; // 아래로 한 칸 이동
                isRotate = false;
            }
        
            // 아무런 키 입력이 없을 경우 Tetromino 움직이지 않게 하기
            if (moveDir != Vector3.zero || isRotate)
            {
                MoveTetromino(moveDir, isRotate);
            }
        }
    }

    // 이동이 가능하면 true, 불가능하면 false를 return
    bool MoveTetromino(Vector3 moveDir, bool isRotate)
    {
        // 이동 or 회전 불가 시 돌아가기 위한 값 저장
        Vector3 oldPos = tetrominoNode.transform.position;
        Quaternion oldRot = tetrominoNode.transform.rotation;
        
        //이동 & 회전
        tetrominoNode.transform.position += moveDir;
        if (isRotate)
        {
            // 현재 테트로미노 노드에 90도 회전을 더해줌
            tetrominoNode.rotation *= Quaternion.Euler(0, 0, 90);
        }
        
        // 이동 불가 시 이전 위치, 회전으로 돌아가기
        if (!CanMoveTo(tetrominoNode))
        {
            tetrominoNode.transform.position = oldPos;
            tetrominoNode.transform.rotation = oldRot;
            
            // 이동 불가하고 현재 아래로 떨어지고 있는 상황 = 바닥에 닿았다는 의미
            if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && isRotate == false)
            {
                AddToBoard(tetrominoNode);
                CheckBoardColumn();
                CreateTetromino();
                
                // 테트로미노 새로 추가 직후 이동 가능 확인
                if (!CanMoveTo(tetrominoNode))
                {
                    clearMap();
                    gameOver = true;
                    GameOverOn?.Invoke(scoreVal, levelVal);
                }
            }
            
            return false;
        }
        
        return true;
    }

    /*
     * foreach
     * 끝을 지정해주는 다른 반복문과 달리, 인자로 들어온 item 내부 인덱스를 처음부터 끝까지 알아서 순환해주는 반복문
     */
    // 보드에 완성된 행이 있으면 삭제
    void CheckBoardColumn()
    {
        bool isCleared = false;
        
        // 한번에 사라진 행 개수 확인용
        int linecount = 0;
        
        // 완성된 행 = 행의 자식 개수가 가로 크기 인지 비교
        foreach (Transform column in boardNode)
        {
            if (column.childCount == boardWidth)
            {
                // 행의 모든 자식을 삭제
                foreach (Transform tile in column)
                {
                    Destroy((tile.gameObject));
                }
                // 행의 모든 자식들과의 연결 끊기
                column.DetachChildren();
                isCleared = true;
                linecount++;    // 완성된 행 하나당 linecount 증가
            }
        }
        
        // 완성된 행이 있을 경우 점수 증가 + 남은 라인 감소
        if (linecount != 0)
        {
            scoreVal += linecount * linecount * 100;
            gameplay.UpdateScore(scoreVal);
            
            lineVal -= linecount;
            gameplay.UpdateLine(lineVal);
            // 레벨업까지 필요 라인 도달 경우 (최대 레벨 10으로 한정)
            if (lineVal <= 0 && levelVal < 10)
            {
                levelVal += 1;  // 레벨 증가
                bombItem += 1;
                changeItem += 1;
                lineVal = levelVal * 2;   // 남은 라인 갱신
                fallCycle = 0.1f * (11 - levelVal); // 속도 증가
                
                gameplay.UpdateLevel(levelVal);
                gameplay.UpdateBomb(bombItem);
                gameplay.UpdateChange(changeItem);
                gameplay.UpdateLine(lineVal);
            }
            else if (lineVal <= 0 && levelVal == 10)
            {
                clearMap();
                gameClear = true;
                GameClearOn?.Invoke(scoreVal);
            }
            
        }
        
        // 비어 있는 행이 존재하면 아래로 내리기
        if (isCleared)
        {
            // 가장 바닥 행은 내릴 필요가 없으므로 index 1부터 for문 시작
            for (int i = 1; i < boardNode.childCount; ++i)
            {
                var column = boardNode.Find(i.ToString());
            
                // 이미 비어 있는 행은 무시
                if (column.childCount == 0)
                    continue;
            
                // 현재 행 아래쪽에 빈 행이 존재하는지 확인, 빈 행만큼 emptyCol 증가
                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0)
                {
                    if (boardNode.Find(j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }
                    j--;
                }
                
                // 현재 행 아래쪽에 빈 행 존재 시 아래로 내림
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

    // 테트로미노를 보드에 추가
    void AddToBoard(Transform root) // tetrominoNode를 매개변수 root로 가져오기
    {
        // 테트로미노가 바닥에 닿을 시 테트로미노의 자녀 오브젝트들의 부모 노드를 Board 노드로 바꾸어 오브젝트들이 이동할 수 있게 함 + 자녀 오브젝트들의 이름을 유니티 좌표계 기준 x 위치로 만들어 관리하기 쉽게 함
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);
            
            // 유니티 좌표계에서 테트리스 좌표계로 변환
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);
            
            // 부모노드 : 행 노드(y 위치), 오브젝트 이름 : x 위치
            node.parent = boardNode.Find(y.ToString());
            node.name = x.ToString();
        }
    }

    // 이동 가능한지 체크 후 True or False 반환하는 메소드
    bool CanMoveTo(Transform root)  // tetrominoNode를 매개변수 root로 가져오기
    {
        // tetrominoNode의 자식 타일을 모두 검사
        for (int i = 0; i < root.childCount; ++i)
        {
            var node = root.GetChild(i);
            
            // 유니티 좌표계에서 테트리스 좌표계로 변환
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);
            
            // 이동 가능한 좌표인지 확인 후 반환
            if (x < 0 || x > boardWidth - 1)
                return false;
            if (y < 0)
                return false;
            
            // 이미 다른 타일이 있는지 확인
            var column = boardNode.Find(y.ToString());
            
            if (column != null && column.Find(x.ToString()) != null)
                return false;
        }
        return true;
    }

    void CreatePreview()
    {
        // 이미 있는 미리보기 삭제하기
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
        // indexVal = Random.Range(0, 7);
        
        Color32 color = Color.white;
        
        // 미리보기 테트로미노 생성 위치 (우측 상단)
        previewNode.position = new Vector2(halfWidth + 8, halfHeight - 3);

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
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(1f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 1f), color);
                CreateTile(previewNode, new Vector2(1f, 1f), color);
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

    private void clearMap()
    {
        Debug.Log("clearmap start");
        foreach (Transform child in boardNode)
        {
            // 기존 타일 삭제
            Destroy(child.gameObject);
        }
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
        foreach (Transform child in backgroundNode)
        {
            // 기존 배경 타일 삭제
            Destroy(child.gameObject);
        }
        Debug.Log("clearmap finish");
    }
    
    void DestroyLineByItem()
    {
        // 가장 아래 줄 제거
        var bottom = boardNode.Find("0");
        foreach (Transform tile in bottom)
        {
            Destroy((tile.gameObject));
        }
        bottom.DetachChildren();
        
        
        // bomb item 업데이트
        bombEffect.Play();
        bombItem -= 1;
        gameplay.UpdateBomb(bombItem);
        
        
        // 점수, 레벨, 라인 업데이트
        scoreVal += 100;
        gameplay.UpdateScore(scoreVal);
        lineVal -= 1;
        gameplay.UpdateLine(lineVal);
        if (lineVal <= 0 && levelVal < 10)
        {
            levelVal += 1;
            changeItem += 1;
            lineVal = levelVal * 2;
            fallCycle = 0.1f * (11 - levelVal);
            
            gameplay.UpdateLevel(levelVal);
            gameplay.UpdateChange(changeItem);
            gameplay.UpdateLine(lineVal);
        }
        else if (lineVal <= 0 && levelVal == 10)
        {
            clearMap();
            gameClear = true;
            GameClearOn?.Invoke(scoreVal);
        }
        
        
        // 1줄 씩 아래로 내리기
        for (int i = 1; i < boardNode.childCount; ++i)
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
        gameplay.UpdateChange(changeItem);
    }
}
