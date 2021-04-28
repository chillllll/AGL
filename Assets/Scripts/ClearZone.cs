using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClearZone : MonoBehaviour
{
    public Enemy enemy;
    public UnityEvent clearEvent;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
    void clearZone()
    {
        clearEvent.Invoke();
    }
}
