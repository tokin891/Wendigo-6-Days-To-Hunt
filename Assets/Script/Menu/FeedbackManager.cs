using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager instance;
    private List<Feedback> allFeedbacks = new List<Feedback>();
    private object[] allFeedbacksItem;

    [SerializeField] List<Feedback> allCurrentHaveFB = new List<Feedback>();
    private Feedback _currentOpen = null;
    private GameObject ReportPanel;

    private TMP_Text _titleText;
    private TMP_Text _DescText;
    private bool isReading = false;

    private void Awake()
    {
        instance = this;
        //--- Get Our Feedbacks
        allFeedbacksItem = Resources.LoadAll("Feedbacks", typeof(Feedback));
        for (int i = 0; i < allFeedbacksItem.Length; i++)
        {
            allFeedbacks.Add((Feedback)allFeedbacksItem[i]);
        }
    }

    private void Update()
    {
        #region ReportPanel 
        ReportPanel = GameObject.Find("Raport Panel");

        if(GameObject.Find("Text (TMP) Title") != null || GameObject.Find("Text (TMP) Desc") != null)
        {
            _titleText = GameObject.Find("Text (TMP) Title").GetComponent<TMP_Text>();
            _DescText = GameObject.Find("Text (TMP) Desc").GetComponent<TMP_Text>();
        }

        if (ReportPanel != null)
        {
            // Current In Menu

            if (allCurrentHaveFB.Count > 0)
            {
                // Some Feedbacks
                StartCoroutine(StartLoopFeedbacks());

                if (_currentOpen != null)
                {
                    // Some Feedback Catch
                    OpenWindowReport();

                    if(_currentOpen._id != 999)
                    {
                        _titleText.text = _currentOpen.GetTitle;
                        _DescText.text = _currentOpen.GetDescriptions;
                    }
                }
            } else
            {
                _titleText.text = "";
                _DescText.text = "";

                CloseWindowReport();
            }
        }
        #endregion
    }

    public void AddFeedback(int id)
    {
        if (CheckFeedbackIsInside(id))
            return;

        allCurrentHaveFB.Add(GetFeedbackFromID(id));
    }

    public void SetupFeedback(Feedback setup_) => _currentOpen = setup_;
    public void RemoveFeedbacks() => allCurrentHaveFB.Clear();
    public void SetNullFeedback() => _currentOpen = null;

    public void OpenWindowReport()
    {
        ReportPanel.GetComponent<Animator>().SetBool("isOn", true);
    }
    public void CloseWindowReport()
    {
        if (allCurrentHaveFB.Count <= 1)
        {
            ReportPanel.GetComponent<Animator>().SetBool("isOn", false);
            _currentOpen = null;
            isReading = false;
        }
        else
        {
            isReading = false;
            _currentOpen = null;
        }
    }


    #region Returns Methods
    private bool CheckFeedbackIsInside(int id)
    {
        for (int i = 0; i < allCurrentHaveFB.Count; i++)
        {
            if(allCurrentHaveFB[i]._id == id)
            {
                return true;
            }
        }

        return false;
    }
    private Feedback GetFeedbackFromID(int id)
    {
        foreach(Feedback one in allFeedbacks)
        {
            if(one._id == id)
                return one;
        }

        return null;
    }
    #endregion

    #region IEnumerator
    private IEnumerator StartLoopFeedbacks()
    {
        if(!CheckFeedbackIsInside(999))
        {
            Feedback _LastFB = new Feedback();
            _LastFB._id = 999;
            allCurrentHaveFB.Add(_LastFB);
        }else
        {
            for (int i = 0; i < allCurrentHaveFB.Count; i++)
            {
                if (isReading)
                {
                    yield return null;
                }
                else
                {
                    isReading = true;
                    SetupFeedback(allCurrentHaveFB[0]);
                    Debug.Log(_currentOpen.GetTitle);
                    allCurrentHaveFB.Remove(allCurrentHaveFB[0]);
                }
            }
        }
        //for (int i = 0; i < allCurrentHaveFB.Count; i++)
        //{
        //    if (_currentOpen != null)
        //        yield return null;

        //    if (_currentOpen == null)
        //    {
        //        SetupFeedback(allCurrentHaveFB[i]);
        //        Debug.Log(_currentOpen.GetTitle);
        //    }
        //}
    }
    #endregion
}

[CreateAssetMenu(fileName ="Defualt Feedback", menuName ="Menu/Feedback")]
public class Feedback: ScriptableObject
{
    [Header("Details")]
    public int _id;
    public TypeFeedback _TypeFB;

    [Space,Header("English")]
    [SerializeField] string _titleE;
    [SerializeField, TextArea(12,12)] string _descriptionE;
    [Space, Header("Polish")]
    [SerializeField] string _titleP;
    [SerializeField, TextArea(12, 12)] string _descriptionP;

    public string GetTitle
    {
        get
        {
            switch((int)GameInput.LanguageKey)
            {
                case 0:
                    return _titleE;
                case 1:
                    return _titleP;
            }

            return _titleE;
        }
    }
    public string GetDescriptions
    {
        get
        {
            switch ((int)GameInput.LanguageKey)
            {
                case 0:
                    return _descriptionE;
                case 1:
                    return _descriptionP;
            }

            return _descriptionE;
        }
    }

    public enum TypeFeedback
    {
        Disconnect
    }
}