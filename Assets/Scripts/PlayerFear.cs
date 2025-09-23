using System.Collections.Generic;
using UnityEngine;

public class PlayerFear : MonoBehaviour
{
    public static PlayerFear Instance;

    [Header("Monsters")]
    public List<Transform> monsters = new List<Transform>();

    [Header("Heartbeat Settings")]
    public float maxHeartbeatDistance = 20f;  
    public float minHeartbeatDistance = 3f;    
    public float updateInterval = 0.1f;        

    [Header("Audio Settings")]
    public float maxVolume = 1f;
    public float minVolume = 0.1f;
    public float maxPitch = 2f;
    public float minPitch = 1f;

    private float lastUpdateTime;
    private float currentThreatDistance;

    private void Awake()
    {
        if (monsters.Count == 0)
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag("Shadow");
            foreach (GameObject go in found)
            {
                monsters.Add(go.transform);
            }
        }
    }

    private void Start()
    {
        SoundManager.StartHeartbeat();
        SoundManager.StartParanoiaSounds();
        currentThreatDistance = maxHeartbeatDistance;
    }

    private void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            currentThreatDistance = maxHeartbeatDistance;

            foreach (Transform monster in monsters)
            {
                if (monster == null) continue;

                float dist = NavMeshUtils.GetPathDistance(transform.position, monster.position);

                if (dist < currentThreatDistance)
                {
                    currentThreatDistance = dist;
                }
            }

            lastUpdateTime = Time.time;
        }

        if (currentThreatDistance < maxHeartbeatDistance)
        {
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
        else
        {
            SoundManager.UpdateHeartbeat(maxHeartbeatDistance);
        }
    }

    private void OnDestroy()
    {
        SoundManager.StopHeartbeat();
        SoundManager.StopParanoiaSounds();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxHeartbeatDistance);
    }
}
