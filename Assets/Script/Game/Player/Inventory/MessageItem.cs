using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class MessageItem : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] TMPro.TMP_Text _text;
    [SerializeField] UnityEngine.UI.Image _icon;
    [SerializeField] float delayDestroy;

    public void Setup(ItemGame item, int howMany = 1)
    {
        _text.text = "+ " + item.name_ + " & " + howMany.ToString() + "x";
        _icon.sprite = item.icon_;

        Destroy(gameObject, delayDestroy);
    }

    public void SetupPlayerMessage(Player _player)
    {
        TextConvertLanguage _Disc = new TextConvertLanguage("Player disconnected", "Gracz rozlaczyl sie");
        _text.text = "-> " + _player.NickName + ": " + TextConvertLanguage.GetText(_Disc);

        Destroy(gameObject, delayDestroy);
    }
}
