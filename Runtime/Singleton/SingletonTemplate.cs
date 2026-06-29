using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MM;

//To use this class simply use SingletonTemplate.Instance.DoSomething();

public class SingletonTemplate : Singleton<SingletonTemplate>
{
    public void DoSomething()
    {
        Debug.Log( "Haciendo algo desde GameManager." );
    }
}
