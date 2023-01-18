using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public enum Character
{
    JustinLeblanc,
    SunilMoore,
    MayankRoss,
    PraveenLeblanc
}
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    public Character _Character = new Character();
    [SerializeField] GameObject playerManager;
    [SerializeField] GameObject playerGame;

    [SerializeField] private List<StoreItemIdentify> allPossibleItemStore;
    [SerializeField] private List<ItemGame> allPossibleItem;
    private object[] allObjectItemStore;
    private object[] allObjectItem;

    public List<ItemGame> AllPossibleItem
    {
        set { allPossibleItem = value; }
        get { return allPossibleItem; }
    }

    private void Start()
    {
        allObjectItem = Resources.LoadAll("Items", typeof(ItemGame));
        for (int i = 0; i < allObjectItem.Length; i++)
        {
            AllPossibleItem.Add((ItemGame)allObjectItem[i]);
        }
        allObjectItemStore = Resources.LoadAll("Item Store", typeof(StoreItemIdentify));
        for (int i = 0; i < allObjectItemStore.Length; i++)
        {
            allPossibleItemStore.Add((StoreItemIdentify)allObjectItemStore[i]);
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    public override void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 1)
        {
            //Respawn Player Manager
            Invoke(nameof(SetupPlayerLobby), 0.5f);
        }
        if(scene.buildIndex == 2)
        {
            Invoke(nameof(SetupPlayerGame), 0.5f);
        }
    }

    public void SetupPlayerGame()
    {
        PhotonNetwork.Instantiate(playerGame.name, FindObjectOfType<RespawnPosition>().GetPosition().position, Quaternion.identity);
    }
    public void SetupPlayerLobby()
    {
        CheckTransform[] Points = FindObjectsOfType<CheckTransform>();
        foreach (CheckTransform one in Points)
        {
            if (one.isFree)
            {
                PhotonNetwork.Instantiate(playerManager.name, one.transform.position, Quaternion.identity);
                one.Setup();
                break;
            }
        }
    }

    public void DisconnectPlayer() =>
        StartCoroutine(_DisconnectAndLoad());

    private IEnumerator _DisconnectAndLoad()
    {
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        while(PhotonNetwork.InRoom)
            yield return null;

        _Character = Character.JustinLeblanc;
        SceneManager.LoadScene(0);
    }

    public static ItemGame GetItemByID(int id)
    {
        foreach (ItemGame one in Instance.AllPossibleItem)
        {
            if (one.ID == id)
            {
                return one;
            }
        }

        return null;
    }
    public static StoreItemIdentify GetItemStoreByName(string _name)
    {
        foreach (StoreItemIdentify one in Instance.allPossibleItemStore)
        {
            if (one._name == _name)
            {
                return one;
            }
        }

        return null;
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
    }
}
