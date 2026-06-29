using System.Collections.Generic;
using UnityEngine;
using MM.Attribute;

public class ParameterValuesTest : MonoBehaviour
{
    [AnimatorParameter]
    public int animatorParameter; //AnimatorParameter, requires animator component

    [Enum( typeof( TestEnum ) )]
    public string m_CurrentState; //Enum Attribute

    public List<int> list = new List<int>() { 1, 2, 3, 4 };

    [SelectFromList("list")]
    public int selectedFromList;

    
}

public enum TestEnum{
    None,
    First,
    Second
}