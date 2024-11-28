using UnityEngine;

[RequireComponent (typeof(ShadowPathController))]
[RequireComponent (typeof(PlayerDetector))]
public class Shadow : MonoBehaviour
{
    public ShadowFSM shadowFSM;
    public ShadowPathController pathController;
    public PlayerDetector playerDetector;

    private void Awake()
    {
        pathController = GetComponent<ShadowPathController>();
        playerDetector = GetComponent<PlayerDetector>();
        
    }
    private void Start()
    {
        shadowFSM = new();

        shadowFSM.Add(new ShadowPatrollingState(this));
        shadowFSM.Add(new ShadowInterestedState(this));
        shadowFSM.Add(new ShadowShyState(this));
        shadowFSM.Add(new ShadowAggressiveState(this));

        shadowFSM.SetCurrentState(ShadowFSMStateType.PATROLLING);
    }

    private void Update()
    {
        shadowFSM.Update();
    }

    private void FixedUpdate()
    {
        shadowFSM.FixedUpdate();
    }
}