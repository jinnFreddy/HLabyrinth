using UnityEngine;

[RequireComponent(typeof(ShadowPathControllerNM))]
[RequireComponent (typeof(PlayerDetector))]
public class Shadow : MonoBehaviour
{
    public ShadowFSM shadowFSM;
    public ShadowPathControllerNM pathController;
    public PlayerDetector playerDetector;

    private void Awake()
    {
        pathController = GetComponent<ShadowPathControllerNM>();
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