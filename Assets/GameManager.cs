using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Trap Sets")]
    [SerializeField] private List<GameObject> trapSetA = new List<GameObject>();
    [SerializeField] private List<GameObject> trapSetB = new List<GameObject>();
    [SerializeField] private List<GameObject> trapSetC = new List<GameObject>();

    private List<List<GameObject>> allTrapSets = new List<List<GameObject>>();
    private List<GameObject> activeTrapSet;

    [Header("Settings")]
    [SerializeField] private bool enableTrapsOnStart = true;

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

        if (enableTrapsOnStart)
        {
            StartNewPlaythrough();
        }
    }

    public void StartNewPlaythrough()
    {
        DisableAllTraps();

        int randomIndex = Random.Range(0, allTrapSets.Count);
        activeTrapSet = allTrapSets[randomIndex];

        EnableTrapSet(activeTrapSet);
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

    private void EnableTrapSet(List<GameObject> trapSet)
    {
        foreach (GameObject trap in trapSet)
        {
            if (trap != null)
            {
                trap.SetActive(true);
            }
        }
    }

    public void RestartPlaythrough()
    {
        StartNewPlaythrough();
    }

    public void SetActiveTrapSet(int index)
    {
        if (index >= 0 && index < allTrapSets.Count)
        {
            DisableAllTraps();
            activeTrapSet = allTrapSets[index];
            EnableTrapSet(activeTrapSet);
        }
    }
}
