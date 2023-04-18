using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{

    Rigidbody2D rb;
    public float speed = 5f;
    public float jumpHeight = 13f;
    public Transform groundCheck;
    bool isGrounded;
    int curHp;
    int maxHp = 3;
    int minHp = 0;
    int coins = 0;
    bool isHit = false;
    public bool key = false;
    bool canTp = true;
    public bool inWater = false;
    bool isClimbing = false;
    bool canHit = true;
    Animator anim;
    public float moveInput;
    public Joystick joystick;

    public Main main;



    public void OnJumpButtonDown()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        curHp = maxHp;
    }


    void Update()
    {
        float verticalMove = joystick.Vertical;
        if (inWater && !isClimbing)
        {
            anim.SetInteger("State", 4);
            isGrounded = true;
            if (moveInput != 0 )
            {
                Flip();
            }
            if (verticalMove >= 0.5 && isGrounded)
            {
                rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
            }
        }
        else 
        {

        CheckGround();
        if (verticalMove >= 0.5 && isGrounded && !isClimbing)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
        }

            if (moveInput == 0 && isGrounded && !isClimbing)
            {
                anim.SetInteger("State", 1);
            }

            else
            {
                if (isGrounded && !isClimbing)
                {
                    Flip();
                    anim.SetInteger("State", 2);
                }
            }
        }
    }

    void FixedUpdate()
    {
        moveInput = joystick.Horizontal;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

    }//Input.GetAxis("Horizontal")

    void Flip()
    {
        if (moveInput > 0)
            transform.localRotation = Quaternion.Euler(0, 0, 0);

        if (moveInput < 0)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    void CheckGround()
    {
        Collider2D[] playerOnTheGround = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
        isGrounded = playerOnTheGround.Length > 1;
        if (!isGrounded && !isClimbing) 
        {
            anim.SetInteger("State", 3);
        }
    }

    public void RecountHp(int deltaHp)
    {
        if (deltaHp < 0 && canHit)
        {
            curHp = curHp + deltaHp;
            StopCoroutine(OnHit());
            isHit = true;
            StartCoroutine(OnHit());
        }

        else if (deltaHp > 0 && canHit)
        {
             curHp = curHp + deltaHp;
        }


        if (curHp <= minHp)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            Invoke("Lose", 1.5f);

        }
        IEnumerator OnHit()
        {
            if (isHit)
            {
                GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g - 0.04f, GetComponent<SpriteRenderer>().color.b - 0.02f);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g + 0.04f, GetComponent<SpriteRenderer>().color.b + 0.02f);
            }

            if (GetComponent<SpriteRenderer>().color.g == 1f)
            {
                StopCoroutine(OnHit());
            }
            if (GetComponent<SpriteRenderer>().color.g <= 0)
            {
                isHit = false;
            }
            yield return new WaitForSeconds(0.02f);

            StartCoroutine(OnHit());
        }
    }

    void Lose()
    {
        main.GetComponent<Main>().Lose();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Key")
        {
            Destroy(collision.gameObject);
            key = true;
        }
        if (collision.gameObject.tag == "deadZone")
        {
            Application.LoadLevel(0);
        }
        if (collision.gameObject.tag == "Door")
        {
            if (collision.gameObject.GetComponent<Door>().isOpen && canTp)
            {
                collision.gameObject.GetComponent<Door>().Teleport(gameObject);
                canTp = false;
                StartCoroutine(TPwait());
            }

            else if (key)
            {
                collision.gameObject.GetComponent<Door>().Unlock();
            }
        }

        if (collision.gameObject.tag == "BlueGem")
        {
            Destroy(collision.gameObject);
            StartCoroutine(NoHit());
        }

        if (collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            coins++;
            print("Кол-во монет:" + coins);
        }

        if (collision.gameObject.tag == "Heart" && curHp < maxHp)
        {
            Destroy(collision.gameObject);
            RecountHp(1);
            print("Текущая жизнь" + curHp);
        }
        
        if (collision.gameObject.tag == "Mushroom")
        {
            Destroy(collision.gameObject);
            RecountHp(-1);
        }

    }
    IEnumerator TPwait()
    {
        yield return new WaitForSeconds(1f);
        canTp = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Ladder")
        {
            isClimbing = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if (Input.GetAxis("Vertical") == 0)
            {
                anim.SetInteger("State", 5);
            }
            else
            {
                anim.SetInteger("State", 6);
                transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 0.5f * Time.deltaTime);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            isClimbing = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trampoline")
        {
            StartCoroutine(TrampolineAnim(collision.gameObject.GetComponentInParent<Animator>()));
        }
    }
        IEnumerator TrampolineAnim(Animator an)
        {
            an.SetBool("isJump", true);
            yield return new WaitForSeconds(0.3f);
            an.SetBool("isJump", false);
        }
        IEnumerator NoHit()
        {
            canHit = false;
            print("Неуязвимость активирована!");
            yield return new WaitForSeconds(5f);
            canHit = true;
            print("Неуязвимость деактивирована!");
        }
}
