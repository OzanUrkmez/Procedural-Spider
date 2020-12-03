using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{


    void Start()
    {
        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = false;
    }

}
