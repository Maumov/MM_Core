using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MM;


//Como suscribirse al evento,
//Como dessuscribirse al evento,
//como llamar al evento.
//como escuchar al evento.

public class EvenBusUsageExample : MonoBehaviour 
{
    void OnEnable()
    {
        EventBus.Instance.Subscribe<PlayerScoredEvent>( OnPlayerScored );
    }

    void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PlayerScoredEvent>( OnPlayerScored );
    }

    void OnPlayerScored( PlayerScoredEvent evt )
    {
        Debug.Log( "Jugador anotó: " + evt.Score );
    }

    // Publicar un evento en alguna parte del código, por ejemplo, cuando el jugador anota
    void PlayerScored( int score )
    {
        var playerScoredEvent = new PlayerScoredEvent( score );
        EventBus.Instance.Publish( playerScoredEvent );
    }

}

//la clase del evento que esta siendo llamado.
public class PlayerScoredEvent : EventBase
{
    public int Score
    {
        get;
    }

    public PlayerScoredEvent( int score )
    {
        Score = score;
    }
}