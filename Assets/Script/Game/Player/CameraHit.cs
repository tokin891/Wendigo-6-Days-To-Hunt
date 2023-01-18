using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraHit : MonoBehaviour
{
    [SerializeField] TMP_Text textMessage;

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 3.5f))
        {
            Inventory inv = FindObjectOfType<Inventory>();

            CarTask carT = hit.transform.GetComponent<CarTask>();
            BoxPlace boxT = hit.transform.GetComponent<BoxPlace>();
            Door doorT = hit.transform.GetComponent<Door>();
            PickupSystem pickT = hit.transform.GetComponent<PickupSystem>();
            Safe safeT = hit.transform.GetComponent<Safe>();
            ShotgunPickSystem r870P = new ShotgunPickSystem();
            NoteObject noT = hit.transform.GetComponent<NoteObject>();
            if(hit.transform.tag == "Forestock" || hit.transform.tag == "Structur")
            { r870P = hit.transform.GetComponentInParent<ShotgunPickSystem>(); }
            Radio radioHit = hit.transform.GetComponent<Radio>();
            RobotArm robotAT = hit.transform.GetComponent<RobotArm>();

            #region AllTexts
            TextConvertLanguage PickBoxT = new TextConvertLanguage("Pick Box", "Podnies Pudelko");
            TextConvertLanguage PickUpT = new TextConvertLanguage("Pickup", "Podnies");
            TextConvertLanguage fineT = new TextConvertLanguage("Fine", "Wystarczy");
            TextConvertLanguage emptyT = new TextConvertLanguage("Empty", "Pusty");
            TextConvertLanguage dropT = new TextConvertLanguage("Drop", "Upusc");
            TextConvertLanguage needT = new TextConvertLanguage("Need", "Potrzeba");
            TextConvertLanguage pressT = new TextConvertLanguage("Press", "Nacisnij");
            TextConvertLanguage tOpenCloseT = new TextConvertLanguage("to Open/Close", "aby Otworzyc/Zamknac");
            TextConvertLanguage tUnlock = new TextConvertLanguage("To Unlock", "Aby Odblokowac");
            TextConvertLanguage lockT = new TextConvertLanguage("Lock", "Zablokowane");
            TextConvertLanguage EnterInputMT = new TextConvertLanguage("Enter Input Mode", "Tryb Wprowadzania Danych");
            TextConvertLanguage busyT = new TextConvertLanguage("Busy Right Now", "Zajety Obecnie");
            TextConvertLanguage readNoteT = new TextConvertLanguage("Read Note", "Przeczytaj Notatke");
            TextConvertLanguage radioT = new TextConvertLanguage("Search Radio", "Sprawdz Radio");
            TextConvertLanguage armT = new TextConvertLanguage("Rotate Arm", "Pokrec Reka");
            TextConvertLanguage correctArmT = new TextConvertLanguage("Put Flare", "Wsadz Flare");
            TextConvertLanguage nCorrectArmT = new TextConvertLanguage("Other Arm", "Inna reka");
            #endregion

            if (carT != null)
            {
                if (carT.Boxs > 0)
                    textMessage.text = TextConvertLanguage.GetText(PickBoxT) + GameInput.Key.FindKey("Interact");
                if(carT.Boxs <= 0)
                    textMessage.text = TextConvertLanguage.GetText(fineT);

                if (GameInput.Key.GetKeyDown("Interact"))
                {
                    if(carT.GetBox())
                    {
                        FindObjectOfType<Inventory>().AddItem(carT.boxItem);
                    }
                }
            }
            if(boxT != null)
            {
                if (boxT._isdrop == isDrop.Yes)
                {
                    if(boxT.howManyYouCanPick > 0)
                    {
                        textMessage.text = TextConvertLanguage.GetText(PickUpT) + GameInput.Key.FindKey("Interact") + " " + boxT.typeItem.name_ + " ->" + boxT.howManyYouCanPick;
                        if (GameInput.Key.GetKeyDown("Interact"))
                        {
                            if (FindObjectOfType<Inventory>().AddItem(boxT.typeItem))
                            {
                                boxT.howManyYouCanPick--;
                            }
                        }
                    }else
                    {
                        textMessage.text = TextConvertLanguage.GetText(emptyT);
                    }
                }

                if(FindObjectOfType<Inventory>().currentUse == boxT.removeItem)
                {
                    if (boxT._isdrop == isDrop.No)
                        textMessage.text = TextConvertLanguage.GetText(dropT) + GameInput.Key.FindKey("Interact");

                    if (GameInput.Key.GetKeyDown("Interact") && boxT._isdrop == isDrop.No)
                    {
                        if (boxT.ChangeTrigger())
                        {
                            FindObjectOfType<Inventory>().RemoveItem(boxT.removeItem);
                        }
                    }
                }
            }
            if(r870P != null)
            {
                if (!r870P.isForestock && !r870P.isStructur)
                {
                    textMessage.text = TextConvertLanguage.GetText(needT) +"Forestock & Structur";
                }
                if (!r870P.isStructur && r870P.isForestock)
                {
                    textMessage.text = TextConvertLanguage.GetText(needT) + "Structur";
                }
                if (!r870P.isForestock && r870P.isStructur)
                {
                    textMessage.text = TextConvertLanguage.GetText(needT) + "Forestock";
                }

                if (inv.currentUse == r870P.itemForestock && GameInput.Key.GetKeyDown("Interact"))
                {
                    r870P.ForestockApply();
                    inv.RemoveItem(r870P.itemForestock);
                }
                if (inv.currentUse == r870P.itemStructur && GameInput.Key.GetKeyDown("Interact"))
                {
                    r870P.StructurApply();
                    inv.RemoveItem(r870P.itemStructur);
                }
            }
            if(doorT != null)
            {
                if(doorT._Lock == isKeyLock.No)
                {
                    textMessage.text = TextConvertLanguage.GetText(pressT) + GameInput.Key.FindKey("Interact") + " " + TextConvertLanguage.GetText(tOpenCloseT);
                    if (GameInput.Key.GetKeyDown("Interact"))
                    {
                        doorT.ChangeStateDoor();
                    }
                }else
                {
                    if(doorT._keyItem != null)
                    {
                        textMessage.text = TextConvertLanguage.GetText(needT) + doorT._keyItem.name_ + " " + TextConvertLanguage.GetText(tUnlock);
                    }
                    else
                    {
                        textMessage.text = TextConvertLanguage.GetText(lockT);
                    }
                }           
                if (GameInput.Key.GetKeyDown("Interact") && inv.currentUse == doorT._keyItem && doorT._keyItem != null)
                {
                    doorT.UnlockKey();
                    inv.RemoveItem(doorT._keyItem);
                }
            }
            if(pickT != null && pickT.isEnabled)
            {
                textMessage.text = TextConvertLanguage.GetText(PickUpT) + pickT.index.name_ + " "+ pickT.howMany + " " + GameInput.Key.FindKey("Interact");
                if(GameInput.Key.GetKeyDown("Interact") && pickT.onClick == false)
                {
                    pickT.Pickup(this);
                }
            } 
            if(safeT != null)
            {
                if(!safeT.isfree)
                {
                    textMessage.text = TextConvertLanguage.GetText(EnterInputMT) + GameInput.Key.FindKey("Interact");
                    if(GameInput.Key.GetKeyDown("Interact"))
                    {
                        if(safeT.isUsePlayer == false)
                        {
                            GetComponentInParent<Game.Player.CharacterMove>().dontMoveAndRotate = true;
                            GetComponentInParent<Game.Player.CharacterMove>().SetCursor(true);
                            safeT.EnterInputCode(GetComponentInParent<Game.Player.CharacterMove>());
                        }else
                            textMessage.text = TextConvertLanguage.GetText(busyT);
                    }
                }else
                {
                    textMessage.text = TextConvertLanguage.GetText(tOpenCloseT) + GameInput.Key.FindKey("Interact");
                    if(GameInput.Key.GetKeyDown("Interact"))
                        safeT.InteractWithDoor();
                }
            }
            if(noT != null)
            {
                textMessage.text = TextConvertLanguage.GetText(readNoteT) + GameInput.Key.FindKey("Interact");

                if (GameInput.Key.GetKeyDown("Interact"))
                    noT.SendNoteText();
            }
            if(radioHit != null)
            {
                textMessage.text = TextConvertLanguage.GetText(radioT) + GameInput.Key.FindKey("Interact");
                if(GameInput.Key.GetKeyDown("Interact"))
                {
                    radioHit.ChangeAvtivity();
                }
            }
            if(robotAT != null)
            {
                if (inv.currentUse != robotAT.indexFlara)
                {
                    textMessage.text = TextConvertLanguage.GetText(armT) + GameInput.Key.FindKey("Interact");
                    if (GameInput.Key.GetKeyDown("Interact"))
                    {
                        robotAT.ChangeRotation();
                    }
                }
                else
                {
                    if (robotAT.flareInArm)
                    {
                        textMessage.text = TextConvertLanguage.GetText(correctArmT) + GameInput.Key.FindKey("Interact");
                        if (GameInput.Key.GetKeyDown("Interact"))
                        {
                            robotAT.curretlyFlara = true;
                            inv.RemoveItem(robotAT.indexFlara);
                        }
                    }
                    else
                    {
                        textMessage.text = TextConvertLanguage.GetText(nCorrectArmT);
                    }
                }
            }

            if (carT == null && boxT == null && r870P == null && doorT == null && pickT == null && 
                safeT == null && noT == null && radioHit == null && robotAT == null)
            {
                textMessage.text = "";
            }
        }
    }

    public GameObject GetHitObject(int range = 100)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            return hit.transform.gameObject;
        }

        return null;
    }
}