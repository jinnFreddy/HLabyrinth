using Patterns;

public class ShadowFSM : FSM
{
    public ShadowFSM() : base() { }

    public void Add(ShadowFSMState state)
    {
        m_states.Add((int)state.ID, state);
    }

    public ShadowFSMState GetState(ShadowFSMStateType key)
    {
        return (ShadowFSMState)GetState((int)key);
    }

    public void SetCurrentState(ShadowFSMStateType stateKey)
    {
        State state = m_states[(int)stateKey];
        if (state != null)
        {
            SetCurrentState(state);
        }
    }
}

public class ShadowFSMState : State
{
    public ShadowFSMStateType ID { get { return _id; } }
    protected Shadow _shadow = null;
    protected ShadowFSMStateType _id;

    public ShadowFSMState(FSM fsm, Shadow shadow) : base(fsm)
    {
        _shadow = shadow;
    }

    public ShadowFSMState(Shadow shadow) : base(fsm: shadow.shadowFSM)
    {
        _shadow = shadow;
        m_fsm = _shadow.shadowFSM;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}