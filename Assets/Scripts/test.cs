using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class test : MonoBehaviour
{
    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (i % 500 == 0)
        {
            Debug.Log("Hello World");
            i = 0;
        }
        i++;
    }
}
