using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum TaskLVL
{
    T00,
    T01,
    T02,
    End
}
public enum WaitingForPlayers
{
    yes,
    no
}
public enum Day
{
    Yes,
    No
}
public enum PlayerPlace
{
    Forest,
    House
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TaskLVL taskLVL = new TaskLVL();
    public WaitingForPlayers _WaitForAllPlayers = new WaitingForPlayers();
    public Day _IsDay= new Day();
    public PlayerPlace _playerPlace = new PlayerPlace();
    private PhotonView PV;

    [SerializeField] GameObject wfpText;
    [SerializeField] GameObject[] ShowObjectTask00;
    [SerializeField] GameObject[] HideObjectTask00;


    public int Day;
    public int TimeDay;
    float currentExposure { get; }
    float fadeTiming = 0.5f;
    bool isFadeUp;
    bool dontLoopChangeWendigo = false;

    private void Awake()
    {
        instance = this;
        PV = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC(nameof(CheckForOtherClients), RpcTarget.All, TimeDay, Day);
            InvokeRepeating(nameof(TimeAdd), 0.1f, 25f);
        }
    }
    private void Update()
    {
        if (FindObjectsOfType<Game.Player.CharacterMove>().Length < PhotonNetwork.PlayerList.Length)
        {
            _WaitForAllPlayers = WaitingForPlayers.yes;
            wfpText.SetActive(true);
        }
        else
        {
            _WaitForAllPlayers = WaitingForPlayers.no;
            wfpText.SetActive(false);
        }

        if (_WaitForAllPlayers == WaitingForPlayers.no)
        {
            if (taskLVL == TaskLVL.T00)
            {
                BoxPlace[] bp = FindObjectsOfType<BoxPlace>();
                CheckTask00(bp);
            }
            if (taskLVL == TaskLVL.T01)
            {
                PickupSystem[] allNeededObject = FindObjectsOfType<PickupSystem>();
                CheckTask01(allNeededObject);
            }
            if(taskLVL == TaskLVL.T02)
            {

            }
        }
        if (TimeDay > 24 || TimeDay < 8)
        {
            _IsDay = global::Day.No;
        }
        else
            _IsDay = global::Day.Yes;

        if (_IsDay == global::Day.Yes)
        {
            if (isFadeUp == false)
            {
                fadeUp();
                SunfadeUp();
            }
        }
        else
        {
            if (isFadeUp == true)
            {
                fadeDown();
                SunfadeDown();
            }
        }

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 1.2f);

        if(PhotonNetwork.IsMasterClient)
        {
            if (_IsDay == global::Day.Yes)
            {
                if(dontLoopChangeWendigo == false)
                {
                    FindObjectOfType<Game.AI.WendigoAI>().ChangeAgressive(false);
                    dontLoopChangeWendigo = true;
                }
            }
            else
            {
                if (dontLoopChangeWendigo == true)
                {
                    FindObjectOfType<Game.AI.WendigoAI>().ChangeAgressive(true);
                    dontLoopChangeWendigo = false;
                }
            }
        }
    }

    private void CheckTask00(BoxPlace[] bp)
    {
        for (int i = 0; i < bp.Length; i++)
        {
            if (bp[i]._isdrop == isDrop.No)
                return;
        }

        for (int i = 0; i < ShowObjectTask00.Length; i++)
        {
            ShowObjectTask00[i].SetActive(true);
        }
        for (int i = 0; i < HideObjectTask00.Length; i++)
        {
            HideObjectTask00[i].SetActive(false);
        }
        taskLVL++;
    }
    private void CheckTask01(PickupSystem[] all)
    {
        foreach (PickupSystem one in all)
        {
            if (one.isImportant)
            {
                return;
            }
        }

        taskLVL++;
    }

    public void fadeDown()
    {
        StartCoroutine(SunFade(1f, 0.03f));
    }
    public void fadeUp()
    {
        StartCoroutine(Fade(0.03f, 1f));
        isFadeUp = true;
    }
    public void SunfadeDown()
    {
        StartCoroutine(Fade(1f, 0.05f));
        isFadeUp = false;
    }
    public void SunfadeUp()
    {
        StartCoroutine(SunFade(0.05f, 1f));
    }

    void TimeAdd()
    {
        if (TimeDay >= 24)
        {
            TimeDay = 0;
            Day++;
        }
        else
        {
            TimeDay++;
        }

        PV.RPC(nameof(CheckForOtherClients), RpcTarget.Others, TimeDay, Day);
    }
    [PunRPC]
    public void CheckForOtherClients(int time, int day)
    {
        TimeDay = time;
        Day = day;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeTiming)
        {
            elapsedTime += 0.11f * Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeTiming));
            RenderSettings.skybox.SetFloat("_Exposure", currentAlpha);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator SunFade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeTiming)
        {
            elapsedTime += 0.11f * Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeTiming));
            foreach (Light dirLight in FindObjectsOfType<Light>())
            {
                if (dirLight.type == LightType.Directional)
                {
                    dirLight.intensity = currentAlpha;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
