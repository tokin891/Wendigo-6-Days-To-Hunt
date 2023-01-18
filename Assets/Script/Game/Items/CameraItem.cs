using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraItem : MonoBehaviour
{
    [SerializeField] string pathCamera;
    [SerializeField] ItemGame index;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            PlaceCamera();
        }
    }

    private void PlaceCamera()
    {
        Inventory inv = FindObjectOfType<Inventory>();
        if(inv != null)
        {
            if (GetComponentInParent<Game.Player.CharacterMove>().DropItemByPath(pathCamera, GetComponentInParent<Game.Player.CharacterMove>().transform.rotation))
            {
                inv.RemoveItem(index);
            }
        }
    }
}
