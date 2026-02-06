using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

[System.Serializable]
public class MyRankInfo
{
    public string name;
    public int score;
    public int rank;
}

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    // ================= CONFIG =================
    public const int APP_VERSION = 3;

    // ================= UI =================
    [Header("UI")]
    public GameObject board;
    public TMP_InputField nameInput;
    public TMP_Text statusText;
    public TMP_Text namePlayer;
    public Button submitButton;

    FirebaseAuth auth;
    FirebaseFirestore db;

    public string UserId { get; private set; }
    public bool IsReady { get; private set; }

    bool isLoading = false;
    bool isVersionValid = true;

    // Guide
    public GuideManager guideManager;

    // ================= UNITY =================
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        InitFirebase();
    }

    // ================= HELPER =================
    void SetLoading(bool loading, string message = "")
    {
        isLoading = loading;
        submitButton.interactable = !loading && isVersionValid;

        if (!string.IsNullOrEmpty(message))
            statusText.text = message;
    }

    void ForceUpdateUI()
    {
        isVersionValid = false;
        statusText.text = "PLEASE UPDATE GAME";
        submitButton.interactable = false;
        board.SetActive(false);
    }

    // ================= INIT =================
    void InitFirebase()
    {
        SetLoading(true, "CONNECTING...");

        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.Result != DependencyStatus.Available)
                {
                    SetLoading(false, "FIREBASE ERROR");
                    return;
                }

                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;

                CheckAppVersion();
            });
    }

    // ================= VERSION CHECK =================
    async void CheckAppVersion()
    {
        try
        {
            var snap = await db.Collection("config")
                .Document("app")
                .GetSnapshotAsync();

            int minVersion = (int)snap.GetValue<long>("minVersion");

            if (APP_VERSION < minVersion)
            {
                ForceUpdateUI();
                return;
            }

            SignIn();
        }
        catch
        {
            SetLoading(false, "VERSION CHECK FAIL");
        }
    }

    // ================= AUTH =================
    void SignIn()
    {
        SetLoading(true, "SIGNING IN...");

        auth.SignInAnonymouslyAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    SetLoading(false, "LOGIN FAIL");
                    return;
                }

                UserId = auth.CurrentUser.UserId;
                IsReady = true;
                CheckUserExist();
            });
    }

    // ================= USER =================
    void CheckUserExist()
    {
        SetLoading(true, "CHECKING USER...");

        db.Collection("ranks").Document(UserId)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    SetLoading(false, "ERROR");
                    return;
                }

                if (!task.Result.Exists)
                {
                    board.SetActive(true);
                    SetLoading(false, "ENTER YOUR NAME");
                    return;
                }

                string name = task.Result.GetValue<string>("name");
                namePlayer.text = name;
                board.SetActive(false);
                SetLoading(false, $"WELCOME {name}");
            });
    }

    // ================= NAME VALIDATION =================
    bool ValidateName(string name, out string error)
    {
        error = "";

        // Dài quá
        if (name.Length > 20)
        {
            error = "NAME MUST NOT EXCEED 20 CHARACTERS";
            return false;
        }

        // Phải bắt đầu B + 7 số
        if (!Regex.IsMatch(name, @"^B\d{7}"))
        {
            error = "INVALID STUDENT ID";
            return false;
        }

        // Phải có dấu _
        if (name.Length <= 8 || name[8] != '_')
        {
            error = "NAME MUST FOLLOW THE FORMAT: MSSV_username";
            return false;
        }

        return true;
    }

    // ================= SUBMIT NAME =================
    public void SubmitName()
    {
        if (isLoading || !isVersionValid) return;

        string playerName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            statusText.text = "NAME REQUIRED";
            return;
        }

        if (!ValidateName(playerName, out string error))
        {
            statusText.text = error;
            return;
        }

        SetLoading(true, "CHECKING NAME...");
        CheckNameUnique(playerName, playerName.ToLower());
    }

    void CheckNameUnique(string playerName, string key)
    {
        db.Collection("names").Document(key)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    SetLoading(false, "ERROR");
                    return;
                }

                if (task.Result.Exists)
                {
                    SetLoading(false, "NAME ALREADY EXISTS");
                    return;
                }

                CreateUser(playerName, key);
            });
    }

    void CreateUser(string playerName, string key)
    {
        SetLoading(true, "CREATING USER...");

        WriteBatch batch = db.StartBatch();

        batch.Set(db.Collection("ranks").Document(UserId),
            new Dictionary<string, object>
            {
                { "name", playerName },
                { "score", 0 },
                { "appVersion", APP_VERSION },
                { "createdAt", FieldValue.ServerTimestamp }
            });

        batch.Set(db.Collection("names").Document(key),
            new Dictionary<string, object>
            {
                { "uid", UserId },
                { "appVersion", APP_VERSION }
            });

        batch.CommitAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                SetLoading(false, "CREATE FAIL");
                return;
            }

            if (guideManager != null && guideManager.stepIndex == 0)
                guideManager.NextStep();

            namePlayer.text = playerName;
            board.SetActive(false);
            SetLoading(false, $"WELCOME {playerName}");
        });
    }

    // ================= SCORE =================
    public void AddScore(int delta)
    {
        if (!IsReady || !isVersionValid) return;

        db.Collection("ranks").Document(UserId)
            .UpdateAsync(new Dictionary<string, object>
            {
                { "score", FieldValue.Increment(delta) },
                { "updatedAt", FieldValue.ServerTimestamp }
            });
    }

    // ================= LEADERBOARD =================
    public async Task LoadLeaderBoard(
        int limit,
        System.Action<List<RankData>> onTopLoaded,
        System.Action<MyRankInfo> onMyInfoLoaded)
    {
        while (!IsReady)
            await Task.Delay(100);

        QuerySnapshot topSnap = await db.Collection("ranks")
            .OrderByDescending("score")
            .Limit(limit)
            .GetSnapshotAsync();

        List<RankData> list = new();
        foreach (var doc in topSnap.Documents)
        {
            list.Add(new RankData
            {
                name = doc.GetValue<string>("name"),
                score = (int)doc.GetValue<long>("score")
            });
        }

        onTopLoaded?.Invoke(list);

        DocumentSnapshot mySnap = await db.Collection("ranks")
            .Document(UserId)
            .GetSnapshotAsync();

        int myScore = mySnap.Exists ? (int)mySnap.GetValue<long>("score") : 0;
        string myName = mySnap.Exists ? mySnap.GetValue<string>("name") : "Unknown";

        QuerySnapshot higherSnap = await db.Collection("ranks")
            .WhereGreaterThan("score", myScore)
            .GetSnapshotAsync();

        onMyInfoLoaded?.Invoke(new MyRankInfo
        {
            name = myName,
            score = myScore,
            rank = higherSnap.Count + 1
        });
    }

    // ================= SAVE HIGH SCORE =================
    public void SaveHighScore(int newScore)
    {
        if (!IsReady) return;

        DocumentReference doc = db.Collection("ranks").Document(UserId);

        doc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || !task.Result.Exists)
                return;

            int oldScore = (int)task.Result.GetValue<long>("score");

            if (newScore <= oldScore)
                return;

            doc.UpdateAsync(new Dictionary<string, object>
            {
                { "score", newScore },
                { "updatedAt", FieldValue.ServerTimestamp }
            });
        });
    }
}
