using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    [Header("Forward")]
    public float forwardSpeed = 8f;
    public bool canMove = true;
    public Loading load;
    public PlayerDie playerDie;
    private bool loadingHandled = false;

    [Header("X2 Money")]
    public bool isX2Money = false, isStartX2Money = false;
    public float timeX2Money;
    public LixiCountShow show;

    [Header("Dynamic Speed")]
    public float baseSpeed = 8f;
    public float maxSpeed = 20f;
    public float speedIncreaseRate = 0.03f;
    private float distanceTravelled = 0f;

    [Header("Lane")]
    public float laneDistance = 2f;
    public float laneChangeSpeed = 10f;
    public int currentLane = 1;

    [Header("Jump")]
    public float jumpForce = 6f;
    public bool isGrounded;
    public bool isAutoJump = false;
    public float radiusBarrierCheck;

    [Header("Fall")]
    public float fallForce = 5f;
    private bool isFallingFast = false;

    [Header("Swipe")]
    public float swipeThreshold = 50f;
    private Vector2 touchStart;
    private bool isSwiping;

    public Rigidbody rb;

    [Header("Speed Boost")]
    public float timeBoost = 10f;
    public float boostMultiplier = 2f;
    public bool isSpeedBoosted = false;
    public GameObject speedEffect;

    [Header("UI")]
    public Image image;
    public GameObject speedShow;

    public Animator animator;

    Coroutine boostCoroutine;
    Coroutine boostUICoroutine;
    Coroutine animCoroutine;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip fallClip;
    public AudioClip changeLaneClip;
    public AudioClip boostClip;

    private bool lastPauseState = false;
    private bool ignoreInputThisFrame = false;

    void Start()
    {
        if (load == null)
            load = FindObjectOfType<Loading>();

        forwardSpeed = baseSpeed;
    }

    void Update()
    {
        bool isPaused = Setting.Instance != null && Setting.Instance.isPause;

        if (lastPauseState && !isPaused)
        {
            ignoreInputThisFrame = true;
            isSwiping = false;
            rb.velocity = Vector3.zero;
        }

        lastPauseState = isPaused;

        if (isPaused) return;

        if (ignoreInputThisFrame)
        {
            ignoreInputThisFrame = false;
            return;
        }

        if (load != null && load.isLoaded && !loadingHandled)
        {
            loadingHandled = true;
            StartCoroutine(WaitLoad());
            return;
        }

        if (!canMove) return;
        if (playerDie != null && playerDie.isDead) return;

        MoveForward();
        UpdateDynamicSpeed();

        if (isAutoJump)
            CheckRadiusBarrier();

        HandleSwipe();
        HandleKeyboard();
        MoveLane();
    }

    // ================= LOAD =================
    IEnumerator WaitLoad()
    {
        float cacheSpeed = forwardSpeed;
        canMove = false;
        forwardSpeed = 0f;

        yield return new WaitForSecondsRealtime(1f);

        canMove = true;
        forwardSpeed = cacheSpeed;
    }

    // ================= MOVE =================
    void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        distanceTravelled += forwardSpeed * Time.deltaTime;
    }

    void UpdateDynamicSpeed()
    {
        if (isSpeedBoosted) return;

        float targetSpeed = baseSpeed + distanceTravelled * speedIncreaseRate;
        forwardSpeed = Mathf.Min(targetSpeed, maxSpeed);
    }

    // ================= INPUT =================
    void HandleSwipe()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            HandleSwipeDirection((Vector2)Input.mousePosition - touchStart);
            isSwiping = false;
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                isSwiping = true;
            }

            if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                Vector2 delta = touch.position - touchStart;

                if (delta.magnitude >= swipeThreshold)
                {
                    HandleSwipeDirection(delta);
                    isSwiping = false;
                }
            }

            if (touch.phase == TouchPhase.Ended)
                isSwiping = false;
        }
#endif
    }

    void HandleSwipeDirection(Vector2 delta)
    {
        if (delta.magnitude < swipeThreshold) return;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
            {
                currentLane = Mathf.Min(2, currentLane + 1);
                if (isGrounded)
                    PlayTurnAnimation("isRight");
            }
            else
            {
                currentLane = Mathf.Max(0, currentLane - 1);
                if (isGrounded)
                    PlayTurnAnimation("isLeft");
            }

            audioSource?.PlayOneShot(changeLaneClip);
        }
        else
        {
            if (delta.y > 0)
                Jump();
            else
                FallDown();
        }
    }

    void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLane = Mathf.Max(0, currentLane - 1);
            audioSource?.PlayOneShot(changeLaneClip);
            if (isGrounded)
                PlayTurnAnimation("isLeft");
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLane = Mathf.Min(2, currentLane + 1);
            audioSource?.PlayOneShot(changeLaneClip);
            if (isGrounded)
                PlayTurnAnimation("isRight");
        }

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            FallDown();
        }
    }

    // ================= LANE =================
    void MoveLane()
    {
        Vector3 targetPos = transform.position;
        targetPos.x = (currentLane - 1) * laneDistance;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            laneChangeSpeed * Time.deltaTime
        );
    }

    // ================= ANIMATION CORE =================
    void PlayAnimation(string anim)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animator.SetBool("isJump", false);
        animator.SetBool("isDown", false);
        animator.SetBool("isLeft", false);
        animator.SetBool("isRight", false);

        animCoroutine = StartCoroutine(Animate(anim));
    }

    IEnumerator Animate(string anim)
    {
        animator.SetBool(anim, true);
        yield return new WaitForSeconds(1f);
        animator.SetBool(anim, false);
    }

    void PlayTurnAnimation(string anim)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animator.SetBool("isJump", false);
        animator.SetBool("isDown", false);
        animator.SetBool("isLeft", false);
        animator.SetBool("isRight", false);

        animCoroutine = StartCoroutine(AnimateTurn(anim));
    }

    IEnumerator AnimateTurn(string anim)
    {
        animator.SetBool(anim, true);
        yield return new WaitForSeconds(0.25f);
        animator.SetBool(anim, false);
    }

    // ================= JUMP =================
    public void Jump()
    {
        if (!isGrounded || isFallingFast)
            return;
        audioSource?.PlayOneShot(jumpClip);

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false;
        isFallingFast = false;

        PlayAnimation("isJump");
    }

    // ================= FALL =================
    void FallDown()
    {
        audioSource?.PlayOneShot(fallClip);

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.down * fallForce, ForceMode.Impulse);

        isFallingFast = true;

        PlayAnimation("isDown");
    }

    // ================= GROUND =================
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            isFallingFast = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
            isGrounded = false;
    }

    // ================= MONEY =================
    public void X2Money()
    {
        audioSource?.PlayOneShot(boostClip);

        if (!isStartX2Money)
        {
            if (show != null)
                show.UpdateCount(-500);
            isStartX2Money = true;
        }

        if (boostCoroutine != null)
            StopCoroutine(boostCoroutine);

        if (boostUICoroutine != null)
            StopCoroutine(boostUICoroutine);

        boostCoroutine = StartCoroutine(X2MoneyRoutine());
        boostUICoroutine = StartCoroutine(X2MoneyUIRoutine(timeBoost));
    }

    IEnumerator X2MoneyRoutine()
    {
        isX2Money = true;
        speedEffect.SetActive(true);

        yield return new WaitForSeconds(timeBoost);

        isX2Money = false;
        speedEffect.SetActive(false);
    }

    IEnumerator X2MoneyUIRoutine(float time)
    {
        speedShow.SetActive(true);
        float timeLeft = time;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            image.fillAmount = timeLeft / time;
            yield return null;
        }

        speedShow.SetActive(false);
        isStartX2Money = false;
    }

    // ================= AUTO JUMP =================
    void CheckRadiusBarrier()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position + Vector3.down * 0.5f,
            radiusBarrierCheck
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("ShortBarrier") && isGrounded)
                Jump();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + Vector3.down * 0.5f,
            radiusBarrierCheck
        );
    }
}
