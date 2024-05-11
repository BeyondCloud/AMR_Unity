using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableChildsOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

}
