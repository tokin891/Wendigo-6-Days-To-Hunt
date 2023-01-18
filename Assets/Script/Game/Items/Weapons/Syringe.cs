using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] Animator[] anms;
    [SerializeField] ItemGame _syringeIndex;

    private ItemGame _currentItem;
    private bool isUse = false;
    private int syringeStackable = 0;

    private void Update()
    {
        Inventory inv = FindObjectOfType<Inventory>();
        _currentItem = inv.currentUse;
        if (inv.CheckAmountItem(_syringeIndex) != null)
        {
            syringeStackable = inv.CheckAmountItem(_syringeIndex).stackableRealtime;
        }
        if(Input.GetMouseButtonDown(0) && !isUse && _currentItem == _syringeIndex && syringeStackable > 0 && GetComponentInParent<Game.Player.CharacterMove>()._playerDetails.health < 100)
        {
            Debug.Log("Use");
            isUse = true;
            for (int i = 0; i < anms.Length; i++)
            {
                anms[i].SetTrigger("isUse");
            }
        }
    }

    public void AddHealing()
    {
        Inventory inv = FindObjectOfType<Inventory>();
        _currentItem = inv.currentUse;

        inv.CheckAmountItem(_syringeIndex).TryRemove();

        GetComponentInParent<Game.Player.CharacterMove>()._playerDetails.health = 100;
        isUse = false;
    }
}
