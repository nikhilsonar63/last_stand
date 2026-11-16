using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinController : MonoBehaviour
{
    public Vector3 coin;
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(coin);

    }
   
}
