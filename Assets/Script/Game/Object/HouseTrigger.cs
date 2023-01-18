using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Game.Player.CharacterMove>() != null)
            other.GetComponent<Game.Player.CharacterMove>().inHouse = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Game.Player.CharacterMove>() != null)
            other.GetComponent<Game.Player.CharacterMove>().inHouse = true;
    }
}
