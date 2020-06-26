using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chute : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.rigidbody.gameObject.tag == "Part")
        {
            Destroy(collision.rigidbody.gameObject);
        }
    }
}
