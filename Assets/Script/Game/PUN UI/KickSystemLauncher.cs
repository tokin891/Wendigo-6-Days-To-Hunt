using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KickSystemLauncher : MonoBehaviourPunCallbacks
{
    [Header("Details")]
    [SerializeField] PlayerListItemToKick _prefabItem;
    [SerializeField] Transform _conent;

    private List<PlayerListItemToKick> _allPlayer = new List<PlayerListItemToKick>();

    #region Override
    private void OnEnable()
    {
        RefreshValue();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshValue();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshValue();
    }

    #endregion

    private void RefreshValue()
    {
        StartCoroutine(RefreshPlayer());
    }
    public void KickPlayer()
    {
        PlayerListItemToKick[] allPlayers = _conent.transform.GetComponentsInChildren<PlayerListItemToKick>();
        List<Player> _playerToKick = new List<Player>();

        foreach (PlayerListItemToKick item in allPlayers)
        {
            if(item.GetToggleSelected())
            {
                _playerToKick.Add(item.playerCurrent);
                break;
            }
        }

        KickPlayer(_playerToKick.ToArray());
    }

    private void KickPlayer(Player[] _kickPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < _kickPlayer.Length; i++)
        {
            PhotonNetwork.CloseConnection(_kickPlayer[i]);
        }
    }

    private IEnumerator RefreshPlayer()
    {
        if (_allPlayer != null)
        {
            for (int i = 0; i < _allPlayer.Count; i++)
            {
                Destroy(_allPlayer[i].gameObject);
            }
        }

        yield return new WaitForSeconds(0.15f);

        Player[] allPlayer = PhotonNetwork.PlayerList;

        for (int i = 0; i < allPlayer.Length; i++)
        {
            PlayerListItemToKick one = Instantiate(_prefabItem, _conent);
            one.Setup(allPlayer[i]);
            _allPlayer.Add(one);
        }
    }
}
