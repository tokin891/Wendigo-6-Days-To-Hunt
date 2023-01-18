using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBoo : MonoBehaviour
{
    [Header("Details"), Tooltip("One Use")]
    [SerializeField] string tagObject;
    [SerializeField] float delay;
    [SerializeField,Space] UnityEvent _onEnterTriggerAfterDelay;

    private float NTTF = 0;
    private bool isUsed = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == tagObject && !isUsed)
        {
            NTTF = Time.time + delay;
        }
    }

    private void Update()
    {
        if(NTTF != 0)
        {
            if(NTTF < Time.time && !isUsed)
            {
                _onEnterTriggerAfterDelay.Invoke();
                isUsed = true;
            }
            Debug.Log("In Trigger");
        }
    }
}
