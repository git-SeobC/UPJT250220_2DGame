using System;
using System.Collections;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public enum ANIMA_STATE
    {
        PlayerIdle,
        PlayerClear,
        PlayerGameOver,
        PlayerRun,
        PlayerJump
    }

    Animator animator;
    string current = "";     // 현재 진행 중인 애니메이션
    string previous = "";    // 기존 진행 중이던 애니메이션

    Rigidbody2D rbody;
    float axisH = 0.0f;
    public float speed = 3.0f;
    public float jump = 7.0f;
    public LayerMask groundLayer;
    bool goJump = false;
    bool onGround = false;

    public static string state = "playing"; // 현재의 상태(플레이 중)

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        state = "playing";
        normalSpeed = speed;
        normalJump = jump;
    }

    void Update()
    {
        if (state != "playing") return;

        // 일반적 2D 움직임 구현할 땐 GetAxisRaw (1칸단위 움직임)
        axisH = Input.GetAxisRaw("Horizontal"); // 수평 이동

        if (axisH > 0.0f)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if (axisH < 0.0f)
        {
            transform.localScale = new Vector2(-1, 1);
            // 벡터가 음수로 잡히게 되면 좌우 반전됨 (물체의 Transform의 Scale 값)
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

    }

    private void FixedUpdate()
    {
        if (state != "playing") return;

        // 지정한 두 점을 연결하는 가상의 선에 게임 오브젝트가 접촉하는지를 조사해 true 또는 false로 return 해주는 함수
        // up은 Vector 기준 (0,1,0)
        onGround = Physics2D.Linecast(transform.position, transform.position - (transform.up * 0.1f), groundLayer);

        // 지면 위에 있거나 또는 속도가 0이 아닌 경우
        if (onGround || axisH != 0)
        {
            rbody.linearVelocity = new Vector2(speed * axisH, rbody.linearVelocityY);
        }

        // 지면 위에 있는 상태에서 점프를 눌렀을 경우
        if (onGround && goJump)
        {
            Vector2 jumpPw = new Vector2(0, jump);
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);
            goJump = false;
        }

        if (onGround)
        {
            if (axisH == 0)
            {
                current = Enum.GetName(typeof(ANIMA_STATE), 0);
                // Enum.GetName(typeof(enum명), 값);
                // 해당 enum에 있는 그 값의 이름을 얻어오는 기능
            }
            else
            {
                current = Enum.GetName(typeof(ANIMA_STATE), 3);
            }
        }
        else
        {
            // 공중에 있는 경우
            current = Enum.GetName(typeof(ANIMA_STATE), 4);
        }


        if (current != previous)
        {
            previous = current;
            animator.Play(current);
        }
    }

    private void Jump()
    {
        goJump = true;
    }

    public enum ITEM_TYPE
    {
        Blue,
        Green,
        Red,
        White
    }

    private Coroutine itemCoroutine;
    private float normalSpeed;
    private float normalJump;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal") Goal();
        else if (collision.gameObject.tag == "Dead") GameoOver();
        else if (collision.gameObject.tag == "Item_Blue") // 아이템 동시에 여러개 먹으면 문제 있음
        {
            if (itemCoroutine != null) StopItemCoroutine();
            itemCoroutine = StartCoroutine(GetItemPower(ITEM_TYPE.Blue));
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Item_Green")
        {
            if (itemCoroutine != null) StopItemCoroutine();
            itemCoroutine = StartCoroutine(GetItemPower(ITEM_TYPE.Green));
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Item_Red")
        {
            if (itemCoroutine != null) StopItemCoroutine();
            itemCoroutine = StartCoroutine(GetItemPower(ITEM_TYPE.Red));
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Item_White")
        {
            if (itemCoroutine != null) StopItemCoroutine();
            itemCoroutine = StartCoroutine(GetItemPower(ITEM_TYPE.White));
            Destroy(collision.gameObject);
        }
    }

    private void StopItemCoroutine()
    {
        StopCoroutine(itemCoroutine);
        speed = normalSpeed;
        jump = normalJump; ;
    }

    IEnumerator GetItemPower(ITEM_TYPE item)
    {
        switch (item)
        {
            case ITEM_TYPE.Blue:
                speed = speed * 1.5f;
                yield return new WaitForSeconds(10);
                StopItemCoroutine();
                break;
            case ITEM_TYPE.Green:
                jump = jump * 1.5f;
                yield return new WaitForSeconds(10);
                StopItemCoroutine();
                break;
            case ITEM_TYPE.Red:
                speed = speed * 0.5f;
                yield return new WaitForSeconds(10);
                StopItemCoroutine();
                break;
            case ITEM_TYPE.White:
                speed = speed * 1.5f;
                jump = jump * 1.5f;
                yield return new WaitForSeconds(10);
                StopItemCoroutine();
                break;
        }
    }

    public void GameoOver()
    {
        Debug.Log("GAMEOVER");
        animator.Play(Enum.GetName(typeof(ANIMA_STATE), 2));
        state = "gameover";
        GameStop();
        GetComponent<CapsuleCollider2D>().enabled = false;  // 현재 플레이어가 가지고 있는 콜라이더를 비활성화 (충돌 발생 X)
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse); // 위로 살짝 뛰어오르는 연출을 위함
    }

    private void GameStop()
    {
        //var rbody = GetComponent<Rigidbody2D>();
        rbody.linearVelocity = new Vector2(0, 0); // 속력을 0으로 만들어서 움직임을 멈춤
    }

    private void Goal()
    {
        Debug.Log("GOAL");
        animator.Play(Enum.GetName(typeof(ANIMA_STATE), 1));
        state = "gameclear";
        GameStop();
    }
}








