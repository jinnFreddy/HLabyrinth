using System.Collections.Generic;
using UnityEngine;

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

    [Header("Trap Sets")]
    [SerializeField] private List<GameObject> trapSetA = new List<GameObject>();
    [SerializeField] private List<GameObject> trapSetB = new List<GameObject>();
    [SerializeField] private List<GameObject> trapSetC = new List<GameObject>();

    [Header("Monsters")]
    [SerializeField] private List<GameObject> allMonsters = new List<GameObject>();

    private List<List<GameObject>> allTrapSets = new List<List<GameObject>>();
    private List<GameObject> activeTrapSet;
    private List<GOState> allObjectStates = new List<GOState>();

    [Header("Settings")]
    [SerializeField] private bool enableOnStart = true;

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

        allTrapSets.Add(trapSetA);
        allTrapSets.Add(trapSetB);
        allTrapSets.Add(trapSetC);

        CaptureOriginalStates();

        if (enableOnStart)
        {
            StartNewPlaythrough();
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
        ResetAllObjectsToOriginal();
        ResetTrapStates();
        DisableAllTraps();

        int randomIndex = Random.Range(0, allTrapSets.Count);
        activeTrapSet = allTrapSets[randomIndex];

        EnableObjects(activeTrapSet);
        EnableObjects(allMonsters);
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
                        Debug.Log($"Reset wire trap: {wireTrap.name}");
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
}
