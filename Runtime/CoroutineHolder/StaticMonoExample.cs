using System.Collections;
using UnityEngine;
using designpatterns;

//This is the tester class that calls a full C# class (not mono) that can then run the coroutine.
public class ExampleTester : MonoBehaviour
{
    private ExampleClass example;

    private void Start()
    {
        example = new ExampleClass();

        example.StartWork();
    }
}

//This is a simple C# class that can run a coroutine using the staticMonobehaviour helper class.
public class ExampleClass
{
    public void StartWork()
    {
        Debug.Log( "Started." );

        StaticMonoBehaviour.StartCoroutine( WorkRoutine() );
    }

    private IEnumerator WorkRoutine()
    {
        yield return new WaitForSeconds( 2f );

        Debug.Log( "Finished after 2 seconds." );
    }
}