using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] GameObject[] TaskObject;

    private Animator anm;
    private int lvlTask;

    private void Awake()
    {
        anm = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            anm.SetTrigger("Show");

        lvlTask = (int)GameManager.instance.taskLVL;
        TaskObject[lvlTask].SetActive(true);

        for (int i = 0; i < lvlTask; i++)
        {
            TaskObject[i].SetActive(false);
        }
        for (int i = lvlTask + 1; i < TaskObject.Length; i++)
        {
            TaskObject[i].SetActive(false);
        }
    }
}
