using System.Collections.Generic;
using UnityEngine;

public class PlayerFear : MonoBehaviour
{
    public static PlayerFear Instance;

    [Header("Monsters")]
    public List<Transform> monsters = new List<Transform>();

    [Header("Heartbeat Settings")]
    public float maxHeartbeatDistance = 25f;  
    public float minHeartbeatDistance = 5f;
    public float updateInterval = 0.1f;

    [Header("Audio Settings")]
    public float maxVolume = 1f;             
    public float minVolume = 0.2f;           
    public float maxPitch = 2f;               
    public float minPitch = 0.8f;             

    private float lastUpdateTime;
    private float currentThreatDistance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        currentThreatDistance = maxHeartbeatDistance;

        if (monsters.Count == 0)
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag("Shadow");
            foreach (GameObject go in found)
            {
                monsters.Add(go.transform);
            }
        }
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
        currentThreatDistance = maxHeartbeatDistance;

        Vector3 playerPos = transform.position;

        foreach (Transform monster in monsters)
        {
            if (monster == null) continue;

            float directDistance = Vector3.Distance(playerPos, monster.position);
            if (directDistance < currentThreatDistance)
            {
                currentThreatDistance = directDistance;
            }
        }

        SoundManager.UpdateHeartbeat(
            currentThreatDistance,
            minDistance: minHeartbeatDistance,
            maxDistance: maxHeartbeatDistance,
            minVolume: minVolume,
            maxVolume: maxVolume,
            minPitch: minPitch,
            maxPitch: maxPitch
        );
    }

    private void OnDestroy()
    {
        SoundManager.StopHeartbeat();
        SoundManager.StopParanoiaSounds();
    }
}