using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    /*
     * Prefab
     * 여러 컴포넌트로 이미 구성이 완성된 재사용 가능한 게임 오브젝트
     * 하나의 게임 오브젝트를 여러번 사용해야 할 경우 프리팹을 사용하면 여러곳에 손쉽게 적용할 수 있음
     */
    
    /*
     * Awake
     * 오브젝트가 생성된 직후 1번만 실행
     * 인스펙터 창에서 스크립트 요소를 비활성화 해도 실행됨
     * 스크립트와 초기화 사이의 모든 레퍼런스 설정에 이용됨
     */
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null)
        {
            // SpriteRenserer가 없을 경우 오류 상황을 출력
            Debug.LogError("You need to SpriteRenderer for Block");
        }
    }
    // 그래픽의 스프라이트를 위한 클래스
    SpriteRenderer spriteRenderer;

    public Color color
    {
        // Set : 멤버 변수에 값을 할당
        set
        {
            // Color : 스프라이트의 색상을 지정
            spriteRenderer.color = value;
        }
        // Get : 멤버 변수에 값을 반환
        get
        {
            return spriteRenderer.color;
        }
    }

    public int sortingOrder
    {
        set
        {
            // SortingOrder : 스프라이트 표시 순서를 결정하기 위한 속성
            spriteRenderer.sortingOrder = value;
        }
        get
        {
            return spriteRenderer.sortingOrder;
        }
    }
}
