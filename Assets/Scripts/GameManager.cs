using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GOState
{
    public GameObject gameObject;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public Vector3 originalScale;
    public bool originalActive;

    public GOState(GameObject obj)
    {
        this.gameObject = obj;
        if (obj != null)
        {
            originalPosition = obj.transform.position;
            originalRotation = obj.transform.rotation;
            originalScale = obj.transform.localScale;
            originalActive = obj.activeSelf;
        }
    }

    public void ResetToOriginal()
    {
        if (gameObject == null) return;

        gameObject.transform.SetPositionAndRotation(originalPosition, originalRotation);
        gameObject.transform.localScale = originalScale;
    }
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("NewPlaythrough")]
    [SerializeField] private Transform firstSpawnPoint;
    [SerializeField] private float minParanoiaDelay = 10f;
    [SerializeField] private float maxParanoiaDelay = 25f;

    [Header("Trap Sets")]
    [SerializeField] private List<GameObject> trapSetA = new List<GameObject>();
    [SerializeField] private List<GameObject> trapSetB = new List<GameObject>();
    [SerializeField] private List<GameObject> trapSetC = new List<GameObject>();

    [Header("Monsters")]
    [SerializeField] private List<GameObject> allMonsters = new List<GameObject>();

    [Header("Settings")]
    [SerializeField] private bool enableOnStart = true;
    [SerializeField] private GameObject deathScreenPanel;
    [SerializeField] private Animator deathScreenAnimator;
    [SerializeField] private Animator fadeInAnimator;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject mainTP;
    [SerializeField] private GameObject[] deathMessages;
    public PlayerMovement playerMovement;
    public float postRestartDelay = 5f;
    public GameObject teleporter;
    public bool isDead { get; set; } = false;

    private List<List<GameObject>> allTrapSets = new List<List<GameObject>>();
    private List<GameObject> activeTrapSet;
    private List<GOState> allObjectStates = new List<GOState>();
    private bool firstPlaythrough = true;
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (deathScreenPanel != null)
        {
            foreach (GameObject msg in deathMessages)
            {
                if (msg != null)
                    msg.SetActive(false);
            }
        }

        allTrapSets.Add(trapSetA);
        allTrapSets.Add(trapSetB);
        allTrapSets.Add(trapSetC);

        CaptureOriginalStates();

        if (enableOnStart)
        {
            StartNewPlaythrough();
            isDead = false;
        }
    }

    private void CaptureOriginalStates()
    {
        allObjectStates.Clear();
        HashSet<GameObject> uniqueObjects = new HashSet<GameObject>();

        CollectUniqueObjects(trapSetA, uniqueObjects);
        CollectUniqueObjects(trapSetB, uniqueObjects);
        CollectUniqueObjects(trapSetC, uniqueObjects);
        CollectUniqueObjects(allMonsters, uniqueObjects);

        foreach (GameObject obj in uniqueObjects)
        {
            allObjectStates.Add(new GOState(obj));
        }
    }

    private void CollectUniqueObjects(List<GameObject> list, HashSet<GameObject> unique)
    {
        foreach (GameObject obj in list)
        {
            if (obj != null && !unique.Contains(obj))
            {
                unique.Add(obj);
            }
        }
    }

    public void StartNewPlaythrough()
    {
        playerMovement.enabled = false;
        SoundManager.StopHeartbeat();
        SoundManager.StopParanoiaSounds();

        if (!firstPlaythrough)
        {
            DeathScreen();
        }        

        if (firstPlaythrough)
        {
            Invoke(nameof(SpawnAtFirstSpawnPoint), 0.05f);

            fadeInAnimator.Play("FadeIn");

            float delay = Random.Range(minParanoiaDelay, maxParanoiaDelay);
            Invoke(nameof(StartParanoiaSounds), delay);
            Invoke(nameof(ResumeAfterRestart), 1f);
        }
        else
        {
            Invoke(nameof(SpawnAtTP), 0.05f);
            Invoke(nameof(ResumeAfterRestart), postRestartDelay);
        }

        ResetAllObjectsToOriginal();
        ResetTrapStates();
        DisableAllTraps();

        int randomIndex = Random.Range(0, allTrapSets.Count);
        activeTrapSet = allTrapSets[randomIndex];

        EnableObjects(activeTrapSet);
        EnableObjects(allMonsters);

        SoundManager.Unmute();
        firstPlaythrough = false;
    }

    private void SpawnAtFirstSpawnPoint()
    {
        if (firstSpawnPoint != null && player != null)
        {
            player.transform.SetPositionAndRotation(firstSpawnPoint.position, firstSpawnPoint.rotation);
        }
    }

    private void SpawnAtTP()
    {
        if (mainTP != null && player != null)
        {
            player.transform.position = teleporter.transform.position;
        }
    }

    private void ResumeAfterRestart()
    {
        isDead = false;
        SoundManager.StartHeartbeat();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void ResetAllObjectsToOriginal()
    {
        foreach (GOState state in allObjectStates)
        {
            state.ResetToOriginal();
        }
    }

    private void ResetTrapStates()
    {
        foreach (GOState state in allObjectStates)
        {
            if (state.gameObject != null)
            {
                WireTrap[] allWireTraps = state.gameObject.GetComponentsInChildren<WireTrap>(true);

                foreach (WireTrap wireTrap in allWireTraps)
                {
                    if (wireTrap != null)
                    {
                        wireTrap._isDisabled = false;
                        wireTrap.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private void DisableAllTraps()
    {
        foreach (List<GameObject> trapSet in allTrapSets)
        {
            foreach (GameObject trap in trapSet)
            {
                if (trap != null)
                {
                    trap.SetActive(false);
                }
            }
        }
    }

    private void EnableObjects(List<GameObject> objectSet)
    {
        foreach (GameObject obj in objectSet)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    public void RestartPlaythrough()
    {
        StartNewPlaythrough();
    }

    private void StartParanoiaSounds()
    {
        SoundManager.StartParanoiaSounds();
    }

    public void DeathScreen()
    {
        if (isDead) return;
        isDead = true;

        SoundManager.BlockNonDeathSounds();
        SoundManager.PlaySoundWithPitch(SoundType.DEATH, 1f);
        
        deathScreenPanel.SetActive(true);
        YouDiedMessage();
        deathScreenAnimator.Play("DeathScreen", 0, 0f);
        
    }

    public void DeathScreenPlatforming()
    {
        if (isDead) return;
        isDead = true;

        SoundManager.BlockNonDeathSounds();
        SoundManager.PlaySoundWithPitch(SoundType.DEATH, 1f);

        deathScreenPanel.SetActive(true);
        YouDiedMessage();
        deathScreenAnimator.Play("DeathScreen2", 0, 0f);
    }

    private void YouDiedMessage()
    {
        if (deathScreenPanel != null)
        {
            foreach (GameObject msg in deathMessages)
            {
                if (msg != null)
                    msg.SetActive(false);
            }
        }

        if (deathMessages.Length > 0)
        {
            int randomIndex = Random.Range(0, deathMessages.Length);
            GameObject selectedMessage = deathMessages[randomIndex];

            if (selectedMessage != null)
            {
                selectedMessage.SetActive(true);                
            }
        }
    }
}
