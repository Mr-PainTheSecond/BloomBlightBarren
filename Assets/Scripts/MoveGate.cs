using System;
using UnityEngine;

public class MoveGate : MonoBehaviour
{
    public GameObject Lgate, Rgate;

    private void Start()
    {
        Lgate.SetActive(false);
        Rgate.SetActive(false);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("PLayer Passed gate");
            Lgate.SetActive(true);
            Rgate.SetActive(true);
            
        }
    }
}
