using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Map : MonoBehaviour
{
    public static Map instance;
    [Header("Details")]
    [SerializeField] float wendiTimeOnMap = 8f;

    [Header("Events")]
    [SerializeField] UnityEvent _onSignalize;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            SignalizeWendigo();
        }
    }

    public void SignalizeWendigo()
    {
        _onSignalize.Invoke();

        FindObjectOfType<Game.AI.WendigoAI>().SignalizeOnMap(wendiTimeOnMap);
    }
}
