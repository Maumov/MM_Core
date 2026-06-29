using MM;
using UnityEngine;

public class PlayerHitMsg : EventMessageID<PlayerHitMsg> { }

public class EventManagerUsageExampleListener
{
    private void OnEnable()
    {
        EventManager<int>.StartListening(
            PlayerHitMsg.ID,
            OnPlayerDamaged );
    }

    private void OnDisable()
    {
        EventManager<int>.StopListening(
            PlayerHitMsg.ID,
            OnPlayerDamaged );
    }

    private void OnPlayerDamaged( int damage )
    {
        Debug.Log( "Damage: " + damage );
    }
}
public class EventManagerUsageExampleSender
{
    public void SomeEvent() 
    {
        EventManager<int>.TriggerEvent( PlayerHitMsg.ID, 25 );
    }
}