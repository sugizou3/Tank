using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyScript : MonoBehaviour
{
    void OnCollisionEnter(Collision other) 
    {
        Destroy(this.transform.parent);
    }
}
