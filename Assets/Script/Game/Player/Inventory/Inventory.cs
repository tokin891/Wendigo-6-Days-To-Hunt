using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Inventory : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] GUIBox[] GUIBoxs;
    [SerializeField] GUIBox[] INVBoxs;
    [SerializeField] ItemGame addOnStart;
    [SerializeField] GameObject _windowNote;
    [SerializeField] UnityEvent _onGrabItem;

    [Header("Message Item")]
    [SerializeField] Transform _parentMessage;
    [SerializeField] MessageItem _prefabMessage;

    private int SelectGUI;
    private GUIBox boxSelectedToChange;

    [HideInInspector] public ItemGame currentUse;

    #region Awake & Update
    private void Awake()
    {
        GUIBoxs[01].isUse = false;
        GUIBoxs[00].isUse = true;
        GUIBoxs[02].isUse = false;

        if (addOnStart)
            Invoke(nameof(addItemInvoke), 1);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && SelectGUI != 0)
        {
            SelectGUI = 0;

            GUIBoxs[01].isUse = false;
            GUIBoxs[00].isUse = true;
            GUIBoxs[02].isUse = false;

            CheckGUICurrent();
            Invoke(nameof(CheckObjectArm), 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && SelectGUI != 1)
        {
            SelectGUI = 1;

            GUIBoxs[00].isUse = false;
            GUIBoxs[01].isUse = true;
            GUIBoxs[02].isUse = false;

            CheckGUICurrent();
            Invoke(nameof(CheckObjectArm), 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && SelectGUI != 2)
        {
            SelectGUI = 2;
            GUIBoxs[00].isUse = false;
            GUIBoxs[01].isUse = false;
            GUIBoxs[02].isUse = true;

            CheckGUICurrent();
            Invoke(nameof(CheckObjectArm), 0.1f);
        }

        if(GUIBoxs[SelectGUI].OnGUIChange())
        {
            CheckGUICurrent();
            Invoke(nameof(CheckObjectArm), 0.1f);
        }
    }
    #endregion

    #region Void
    private void CheckGUICurrent()
    {
        if (GUIBoxs[SelectGUI].itemIndex != null)
        {
            currentUse = GUIBoxs[SelectGUI].itemIndex;
        }
        else
        {
            currentUse = null;
        }
    }
    private void CheckObjectArm()
    {
        if (currentUse != null)
        {
            ObjectArm[] allObjects = FindObjectsOfType<ObjectArm>();
            foreach (ObjectArm one in allObjects)
            {
                if (one.index == currentUse)
                {
                    for (int i = 0; i < one.objectItem.Length; i++)
                    {
                        one.objectItem[i].SetActive(true);
                        if (one.objectItem[i].GetComponentInChildren<Game.Weapons.FireWepon>() != null)
                            one.objectItem[i].GetComponentInChildren<Game.Weapons.FireWepon>().StopRelode();
                    }
                }
                else
                {
                    for (int i = 0; i < one.objectItem.Length; i++)
                    {
                        one.objectItem[i].SetActive(false);
                    }
                }
            }
            _onGrabItem.Invoke();
        }
        else
        {
            ObjectArm[] allObjects = FindObjectsOfType<ObjectArm>();
            for (int i = 0; i < allObjects.Length; i++)
            {
                for (int i_ = 0; i_ < allObjects[i].objectItem.Length; i_++)
                {
                    allObjects[i].objectItem[i_].SetActive(false);
                }
            }
        }
    }
    private void addItemInvoke()
    {
        AddItem(addOnStart);
    }
    public bool AddItem(ItemGame item, int howMany = 1, GUIBox selectBox = null, bool showMessage = true)
    {
        if(selectBox == null)
        {
            foreach (GUIBox box in INVBoxs)
            {
                if (box.itemIndex == item)
                {
                    box.AddStack(howMany);
                    if(showMessage)
                    {
                        ShowMessage(item, howMany);
                    }
                    return true;
                }
                if (box.isFree == true)
                {
                    box.Add(item, false, howMany);
                    if (showMessage)
                    {
                        ShowMessage(item, howMany);
                    }
                    Debug.Log("Add " + item.name_);
                    return true;
                }
            }
        }else
        {
            if (selectBox.itemIndex == item)
            {
                selectBox.AddStack(howMany);
                if (showMessage)
                {
                    ShowMessage(item, howMany);
                }
                return true;
            }
            if (selectBox.isFree == true)
            {
                selectBox.Add(item, false, howMany);
                if (showMessage)
                {
                    ShowMessage(item, howMany);
                }
                Debug.Log("Add " + item.name_);
                return true;
            }
        }
        return false;
    }
    public void RemoveItem(ItemGame item)
    {
        foreach (GUIBox box in INVBoxs)
        {
            if (box.itemIndex == item)
            {
                box.stackableRealtime--;

                break;
            }
        }
    }
    public bool AddGUIItem(GUIBox GUI)
    {
        foreach(GUIBox one in GUIBoxs)
        {
            if(one.itemIndex == GUI.itemIndex)
            {
                one.GUIIndex = null;
                one.isUse = false;
                one.ClearAll();
            }
        }
        GUIBoxs[SelectGUI].GUIAdd(GUI);
        return true;
    }
    public GUIBox CheckAmountItem(ItemGame item)
    {
        foreach(GUIBox one in INVBoxs)
        {
            if(one.itemIndex == item)
            {
                return one;
            }
        }

        return null;
    }
    public void GetNote(string __text)
    {
        GetComponentInParent<Game.Player.CharacterMove>().OpenInv();

        _windowNote.SetActive(true);
        _windowNote.GetComponentInChildren<TMP_Text>().text = __text;
    }
    public void OffNote()
    {
        if(_windowNote.activeSelf == true)
        {
            _windowNote.SetActive(false);
            _windowNote.GetComponentInChildren<TMP_Text>().text = "";
        }
    }
    private void ShowMessage(ItemGame item, int hm)
    {
        MessageItem mess = Instantiate(_prefabMessage, _parentMessage);
        mess.Setup(item, hm);
    }
    public GUIBox SetBoxToChangePlace(GUIBox index = null)
    {
        if (boxSelectedToChange == null && index != null)
        {
            boxSelectedToChange = index;
            return null;
        }
        if(boxSelectedToChange != null)
        {
            return boxSelectedToChange;
        }

        return null;
    }
    public Dictionary<ItemGame, int> GetAllEquipmentItem()
    {
        Dictionary<ItemGame, int> items = new Dictionary<ItemGame, int>();
        for (int i = 0; i < INVBoxs.Length; i++)
        {
            items.Add(INVBoxs[i].itemIndex, INVBoxs[i].stackableRealtime);
        }

        return items;
    }

    public void ClearBoxToChangePlace()
    {
        boxSelectedToChange.ClearAll();
        boxSelectedToChange.inChangeMode = false;
        boxSelectedToChange = null;
    }

    #endregion
}
