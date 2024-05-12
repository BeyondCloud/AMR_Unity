using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        target.SetActive(false);
    }

    public void Switch()
    {
        target.SetActive(!target.activeSelf);
    }
}
