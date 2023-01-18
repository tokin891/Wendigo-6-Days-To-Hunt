using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerListItemToKick : MonoBehaviour
{
    [SerializeField] TMP_Text _playername;
    [SerializeField] TMP_Text _playertype;
    [SerializeField] Toggle _toggleSelected;

    [SerializeField] Image _playericon;
    [Tooltip("1.Justin, 2.Sunil, 3.Mayank, 4.Praveen")]
    [SerializeField] Sprite[] _mcSprite;

    public Player playerCurrent;

    private void Awake()
    {
        _toggleSelected.isOn = false;
    }
    public void Setup(Player _player)
    {
        playerCurrent = _player;
        Character _type = GetPlayerType(_player);
        _playername.text = _player.NickName;
        _playertype.text = _type.ToString();

        switch ((int)_type)
        {
            case 0:
                _playericon.sprite = _mcSprite[0];
                break;
            case 1:
                _playericon.sprite = _mcSprite[1];
                break;
            case 2:
                _playericon.sprite = _mcSprite[2];
                break;
            case 3:
                _playericon.sprite = _mcSprite[3];
                break;
        }

        //if (_player.IsMasterClient)
            //_toggleSelected.interactable = false;
    }
    private Character GetPlayerType(Player _player)
    {
        Game.Player.CharacterMove[] AllPlayer = FindObjectsOfType<Game.Player.CharacterMove>();

        for (int i = 0; i < AllPlayer.Length; i++)
        {
            if(AllPlayer[i].photonView.Owner == _player)
            {
                return AllPlayer[i].GetCharacter;
            }
        }

        return Character.JustinLeblanc;
    }

    public bool GetToggleSelected()
    {
        return _toggleSelected.isOn;
    }
}
