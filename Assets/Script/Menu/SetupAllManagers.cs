using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupAllManagers : MonoBehaviour
{
    [SerializeField] Behaviour[] allComponents;

    private void Awake()
    {
        SetupAllManagers[] allGT = FindObjectsOfType<SetupAllManagers>();
        if (allGT.Length > 1)
        {
            Destroy(gameObject);
        }else
        {
            for (int i = 0; i < allComponents.Length; i++)
            {
                allComponents[i].enabled = true;
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
