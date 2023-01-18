using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportLauncher : MonoBehaviour
{
    public void ClickOkey()
    {
        FeedbackManager.instance.CloseWindowReport();
    }
}
