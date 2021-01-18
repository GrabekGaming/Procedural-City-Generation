using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    BoxCollider thisCollider;

    private void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Building"))
        {
            if (gameObject.GetInstanceID() > other.GetInstanceID())
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
