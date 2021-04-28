using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayercollideEvent : MonoBehaviour
{
   
    public UnityEvent feedback;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            feedback.Invoke();
        }
    }
   
}
