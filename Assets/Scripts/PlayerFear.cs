using System.Collections.Generic;
using UnityEngine;

public class PlayerFear : MonoBehaviour
{
    public static PlayerFear Instance;

    [Header("Monsters")]
    [SerializeField] private List<Transform> monsters = new List<Transform>();

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 30f;        
    private Camera mainCamera;

    [Header("Heartbeat Intensity Levels")]
    [SerializeField] private float calmVolume = 0.25f;
    [SerializeField] private float calmPitch = 0.8f;

    [SerializeField] private float nearVolume = 0.6f;
    [SerializeField] private float nearPitch = 1.4f;

    [SerializeField] private float seenVolume = 1.0f;
    [SerializeField] private float seenPitch = 2.0f;

    private float lastUpdateTime;
    private float updateInterval = 0.1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (monsters.Count == 0)
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag("Shadow");
            foreach (GameObject go in found)
            {
                monsters.Add(go.transform);
            }
        }

        if (mainCamera == null)
            mainCamera = Camera.main;

        SoundManager.StartHeartbeat();
    }

    private void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            EvaluateThreatLevel();
            lastUpdateTime = Time.time;
        }
    }

    private void EvaluateThreatLevel()
    {
        bool isMonsterOnScreen = false;
        bool isMonsterNear = false;

        Vector3 playerPos = transform.position;

        foreach (Transform monster in monsters)
        {
            if (monster == null) continue;

            Vector3 monsterPos = monster.position;
            float directDistance = Vector3.Distance(playerPos, monsterPos);

            if (directDistance <= detectionRadius)
            {
                isMonsterNear = true;

                if (IsMonsterOnScreen(monsterPos))
                {
                    isMonsterOnScreen = true;
                    break; 
                }
            }
        }

        if (isMonsterOnScreen)
        {
            SoundManager.UpdateHeartbeat(0f, minDistance: 0f, maxDistance: 1f,
                minVolume: seenVolume, maxVolume: seenVolume,
                minPitch: seenPitch, maxPitch: seenPitch);
        }
        else if (isMonsterNear)
        {
            SoundManager.UpdateHeartbeat(detectionRadius * 0.5f, minDistance: 0f, maxDistance: detectionRadius,
                minVolume: nearVolume, maxVolume: nearVolume,
                minPitch: nearPitch, maxPitch: nearPitch);
        }
        else
        {
            SoundManager.UpdateHeartbeat(detectionRadius, minDistance: 0f, maxDistance: detectionRadius,
                minVolume: calmVolume, maxVolume: calmVolume,
                minPitch: calmPitch, maxPitch: calmPitch);
        }
    }

    private bool IsMonsterOnScreen(Vector3 worldPosition)
    {
        if (mainCamera == null) return false;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPosition);

        bool isInFront = viewportPoint.z > 0; 
        bool isInX = viewportPoint.x >= 0 && viewportPoint.x <= 1;
        bool isInY = viewportPoint.y >= 0 && viewportPoint.y <= 1;

        return isInFront && isInX && isInY;
    }

    private void OnDestroy()
    {
        SoundManager.StopHeartbeat();
        SoundManager.StopParanoiaSounds();
    }
}