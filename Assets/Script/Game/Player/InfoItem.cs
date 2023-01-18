using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoItem : MonoBehaviour
{
    [SerializeField] TMP_Text name_;
    [SerializeField] TMP_Text des_;
    [SerializeField] UnityEngine.UI.Slider howManyDrop;
    [SerializeField] UnityEngine.UI.Button buttonDrop;
    private GUIBox itemGUI;

    private void Awake()
    {
        buttonDrop.interactable = false;
        howManyDrop.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (itemGUI != null && itemGUI.itemIndex == null)
            ClearInfo();
    }
    public void GetInfo(string _name, string _des, GUIBox guiSelected)
    {
        name_.text = _name;
        des_.text = _des;
        itemGUI = guiSelected;

        howManyDrop.gameObject.SetActive(true);
        buttonDrop.interactable = true;
        howManyDrop.value = 1;
        howManyDrop.maxValue = itemGUI.stackableRealtime;
    }
    public void DropItem()
    {
        if (howManyDrop.value == 0)
            return;

        if(howManyDrop.value == howManyDrop.maxValue)
        {
            //Drop All
            if (GetComponentInParent<Game.Player.CharacterMove>().DropItemByInfo(itemGUI.itemIndex, (int)howManyDrop.value))
            {
                name_.text = "";
                des_.text = "";
                itemGUI.ClearAll();

                itemGUI = null;
            }
        }else if(howManyDrop.value < howManyDrop.maxValue)
        {
            //Drop Value
            if(GetComponentInParent<Game.Player.CharacterMove>().DropItemByInfo(itemGUI.itemIndex, (int)howManyDrop.value))
            {
                itemGUI.stackableRealtime -= (int)howManyDrop.value;
                howManyDrop.value = 1;
                howManyDrop.maxValue = itemGUI.stackableRealtime;
            }
        }
    }
    public void ClearInfo()
    {
        name_.text = "";
        des_.text = "";
        itemGUI = null;

        buttonDrop.interactable = false;
        howManyDrop.gameObject.SetActive(false);
    }
}
