using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GUIBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image icon;
    [HideInInspector] public bool isFree = true;
    [HideInInspector] public bool inChangeMode = false;
    [HideInInspector] public bool isUse = false;
    [HideInInspector] public int stackableRealtime = 0;

    [HideInInspector] public GUIBox GUIIndex;
    [SerializeField] public ItemGame itemIndex;
    [SerializeField] TMP_Text textStack;

    private bool CursorOnGUI;
    private bool changeLoop = false;
    private bool isGUI = false;

    private void Awake()
    {
        isFree = true;
    }
    private void Update()
    {
        if(GUIIndex != null && GUIIndex.itemIndex == null )
        {
            ClearAll();
            GUIIndex = null;
            icon.color = new Color32(255, 255, 255, 25);
        }

        if(isUse == false && isFree)
            icon.color = new Color32(255, 255, 255, 0);

        if(isUse)
        {
            if(itemIndex != null)
            {
                icon.color = new Color32(255, 255, 255, 255);
            }else
                icon.color = new Color32(255, 255, 255, 0);
        }else if(!isFree)
        {
            if(inChangeMode)
            {
                icon.color = new Color32(138, 18, 26, 255);
            }
            else
            {
                if(isGUI)
                {
                    icon.color = new Color32(255, 255, 255, 25);
                }
                else
                    icon.color = new Color32(255, 255, 255, 255);
            }
        }

        if (stackableRealtime != 0)
        {        
            textStack.text = stackableRealtime.ToString();
        }
        else
            textStack.text = "";

        if (stackableRealtime <= 0 && itemIndex != null)
            ClearAll();

        if(CursorOnGUI)
        {
            Inventory inv = FindObjectOfType<Inventory>();
            if (Input.GetMouseButtonDown(1) && inv.SetBoxToChangePlace() == null && !isFree)
            {
                inv.SetBoxToChangePlace(this);

                inChangeMode = true;
            }
            if(inv.SetBoxToChangePlace() != null && itemIndex == null && isFree && !isUse)
            {
                icon.sprite = inv.SetBoxToChangePlace().itemIndex.icon_;
                icon.color = new Color32(255, 255, 255, 15);
            }
            if(Input.GetMouseButtonDown(2))
            {
                if (itemIndex != null)
                {
                    FindObjectOfType<InfoItem>().GetInfo(itemIndex.name_, itemIndex.description_, this);
                }
                else
                    FindObjectOfType<InfoItem>().ClearInfo();
            }
            if (Input.GetMouseButtonDown(1) && inv.SetBoxToChangePlace() != null && inv.SetBoxToChangePlace().itemIndex == itemIndex && inv.SetBoxToChangePlace() != this)
            {
                stackableRealtime += inv.SetBoxToChangePlace().stackableRealtime;
                inv.ClearBoxToChangePlace();
            }
        }
        else
        {
            if (icon.sprite != null && isFree && !isUse)
            {
                icon.sprite = null;
            }
        }
    }
    public void AddStack(int index)
    {
        stackableRealtime += index;
    }
    public void Add(ItemGame item, bool freeSlot = false, int howMany = 1)
    {
        itemIndex = item;

        icon.sprite = item.icon_;
        isFree = freeSlot;
        stackableRealtime = howMany;
        changeLoop = true;
    }
    public void GUIAdd(GUIBox one)
    {
        GUIIndex = one;
        Add(one.itemIndex, true);
        isUse = true;
        isFree = false;
        isGUI = true;
    }
    public void ClearAll()
    {
        // ClearSlots

        itemIndex = null;
        GUIIndex = null;

        icon.color = new Color32(0, 0, 0, 0);
        icon.sprite = null;
        isFree = true;
        isGUI = false;
        isUse = false;
        stackableRealtime = 0;
        changeLoop = true;
    }
    public void TryRemove()
    {
        // Try Remove Or Just Clear Box

        if(stackableRealtime > 1)
        {
            stackableRealtime--;
        }else
        {
            ClearAll();
        }
    }
    public void Click()
    {
        Inventory inv = FindObjectOfType<Inventory>();

        if (itemIndex != null && inv.AddGUIItem(this))
            Debug.Log("Succes add item");

        if(itemIndex == null && inv.SetBoxToChangePlace() != null)
        {
            if(inv.SetBoxToChangePlace() == this)
            {               
                inv.ClearBoxToChangePlace();
            }else
            {
                inv.AddItem(inv.SetBoxToChangePlace().itemIndex, inv.SetBoxToChangePlace().stackableRealtime, this, false);
                inv.ClearBoxToChangePlace();
            }
        }
    }

    public bool OnGUIChange()
    {
        if (changeLoop)
        {
            changeLoop = false;
            return true;
        }
        else
            return false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorOnGUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorOnGUI = false;
    }
}
