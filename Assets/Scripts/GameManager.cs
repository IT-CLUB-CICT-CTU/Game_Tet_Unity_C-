using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score")]
    public SecureInt score = new SecureInt(); // 🔒 anti-cheat score

    public LixiCountShow lixiCountShow;

    [Header("State")]
    public bool isGameStarted = false;
    private bool hasSaved = false;

    [Header("UI")]
    public GameObject[] UIMenu;
    public GameObject startPrefab;
    public GameObject load;
    private GameObject currentStartObj;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip mainMenu;
    public AudioClip inGame;
    public AudioClip ready;
    public AudioClip touchClip;
    public float delayTime = 1f;

    [Header("Camera & Anim")]
    public Animator animator;
    public CameraFollow cameraFollow;

    // ================= INIT =================
    void Awake()
    {
        Instance = this;
        // 🔒 Init SecureInt đúng lifecycle Unity
        score.Init(0);
    }

    void Start()
    {
        PlayMusicWithDelay(mainMenu, 2f);
    }

    void Update()
    {
        if (lixiCountShow == null)
            lixiCountShow = FindObjectOfType<LixiCountShow>();

        DetectTouchSound();
    }

    // ================= TOUCH =================
    void DetectTouchSound()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
            PlayTouchSound();
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
            PlayTouchSound();
#endif
    }

    // ================= MUSIC =================
    void PlayMusicWithDelay(AudioClip clip, float delay)
    {
        StartCoroutine(PlayMusicCoroutine(clip, delay));
    }

    IEnumerator PlayMusicCoroutine(AudioClip clip, float delay)
    {
        AudioListener.volume = 0f;
        yield return new WaitForSeconds(delay);
        AudioListener.volume = 1f;
        PlayMusic(clip);
    }

    void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.loop = clip != ready;
        audioSource.Play();
    }

    // ================= GAME FLOW =================
    public void Replay()
    {
        // 🔒 đổi key mỗi round
        score.ReKey();

        if (lixiCountShow != null)
            lixiCountShow.count.ReKey();

        OutToReplayAndSaveReward();
        PlayMusicCoroutine(inGame, 1.5f);

        hasSaved = false;
        isGameStarted = true;

        load.SetActive(true);
        SetUIMenu(false);

        StartCoroutine(WaitReplay());
    }

    IEnumerator WaitReplay()
    {
        if (UIMenu.Length > 5)
        {
            UIMenu[4].SetActive(false);
            UIMenu[5].SetActive(false);
        }

        yield return new WaitForSeconds(2.5f);

        currentStartObj = Instantiate(
            startPrefab,
            Vector3.zero,
            Quaternion.identity
        );
    }

    public void StartGame()
    {
        // 🔒 đổi key mỗi round
        score.ReKey();

        if (lixiCountShow != null)
            lixiCountShow.count.ReKey();

        hasSaved = false;
        isGameStarted = true;

        if (UIMenu.Length > 5)
        {
            UIMenu[4].SetActive(true);
            UIMenu[5].SetActive(true);
        }

        StartCoroutine(WaitLoad());
        PlayMusicWithDelay(inGame, 7.5f);
    }

    IEnumerator WaitLoad()
    {
        animator.enabled = true;
        animator.SetBool("isStart", true);

        PlayMusicWithDelay(ready, 0f);
        yield return new WaitForSeconds(4.5f);

        load.SetActive(true);

        if (UIMenu.Length > 5)
        {
            UIMenu[4].SetActive(false);
            UIMenu[5].SetActive(false);
        }

        yield return new WaitForSeconds(2f);

        currentStartObj = Instantiate(
            startPrefab,
            Vector3.zero,
            Quaternion.identity
        );
    }

    // ================= EXIT & SAVE =================
    public void OutToReplayAndSaveReward()
    {
        if (!isGameStarted || hasSaved) return;

        hasSaved = true;
        isGameStarted = false;

        cameraFollow.isOffsetMoving = false;

        if (currentStartObj != null)
            Destroy(currentStartObj);

        int reward = CalculateReward();
        SetScore(reward);

        Debug.Log($"[FINAL SCORE] = {score.Get()}");

        if (FirebaseManager.Instance != null &&
            FirebaseManager.Instance.IsReady)
        {
            FirebaseManager.Instance.SaveHighScore(score.Get()); // 🔒
        }

        CleanupScene();
        SetUIMenu(true);

        Time.timeScale = 1f;
        AudioListener.volume = 1f;
    }

    public void OutToMenuAndSaveReward()
    {
        if (!isGameStarted || hasSaved) return;

        hasSaved = true;
        isGameStarted = false;

        cameraFollow.isOffsetMoving = false;
        animator.SetBool("isStart", false);
        StartCoroutine(DisableAnimatorDelay());

        if (currentStartObj != null)
            Destroy(currentStartObj);

        int reward = CalculateReward();
        SetScore(reward);

        Debug.Log($"[FINAL SCORE] = {score.Get()}");

        if (FirebaseManager.Instance != null &&
            FirebaseManager.Instance.IsReady)
        {
            FirebaseManager.Instance.SaveHighScore(score.Get()); // 🔒
        }

        CleanupScene();
        SetUIMenu(true);

        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        PlayMusicWithDelay(mainMenu, delayTime);
    }

    IEnumerator DisableAnimatorDelay()
    {
        yield return new WaitForSeconds(0.5f);
        animator.enabled = false;
    }

    // ================= SCORE =================
    int CalculateReward()
    {
        if (lixiCountShow == null)
            return 0;

        int value = lixiCountShow.GetCount();

        // 🔒 basic anti-cheat validation
        if (value < 0 || value > 1_000_000)
        {
            Debug.LogWarning("CHEAT DETECTED");
            value = 0;
        }

        return value;
    }

    public void AddScore(int value)
    {
        score.Set(value); // 🔒 giữ flow cũ (set, không cộng)
    }

    public void SetScore(int value)
    {
        score.Set(value); // 🔒
    }

    public int GetScore()
    {
        return score.Get();
    }

    // ================= CLEAN =================
    void CleanupScene()
    {
        DestroyWithTag("MiniBoss");
        DestroyWithTag("BigBoss");
        DestroyWithTag("Lixi");
    }

    void DestroyWithTag(string tag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        foreach (var o in objs)
            Destroy(o);
    }

    void SetUIMenu(bool value)
    {
        foreach (GameObject ui in UIMenu)
            ui.SetActive(value);
    }

    // ================= UI =================
    void PlayTouchSound()
    {
        if (isGameStarted) return;
        if (touchClip == null || audioSource2 == null) return;

        audioSource2.PlayOneShot(touchClip);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
