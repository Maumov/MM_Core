
using UnityEngine;


public class StateMachineTemplate : StateManager<StateMachineTemplate.ExampleEnum>
{
    public enum ExampleEnum
    {
        None,
        Walk,
        Run,
        Swim
    }


}

// Idle State
public class IdleState : BaseState<StateMachineTemplate.ExampleEnum>
{
    public IdleState( StateMachineTemplate.ExampleEnum key ) : base( key )
    {
    }

    public override void EnterState()
    {
       
    }

    public override void ExitState()
    {
       
    }

    public override StateMachineTemplate.ExampleEnum GetNextState()
    {
        return StateMachineTemplate.ExampleEnum.None;
    }

    public override void OnTriggerEnter( Collider other )
    {
      
    }

    public override void OnTriggerExit( Collider other )
    {

    }
    public override void OnTriggerStay( Collider other )
    {
        
    }

    public override void UpdateState()
    {
     
    }
}

// Move State
public class MoveState : BaseState<StateMachineTemplate.ExampleEnum>
{
    public MoveState( StateMachineTemplate.ExampleEnum key ) : base( key )
    {
    }

    public override void EnterState()
    {
    
    }

    public override void ExitState()
    {
     
    }

    public override StateMachineTemplate.ExampleEnum GetNextState()
    {
        return StateMachineTemplate.ExampleEnum.None;
    }

    public override void OnTriggerEnter( Collider other )
    {
       
    }

    public override void OnTriggerExit( Collider other )
    {
      
    }

    public override void OnTriggerStay( Collider other )
    {
      
    }

    public override void UpdateState()
    {
       
    }
}

