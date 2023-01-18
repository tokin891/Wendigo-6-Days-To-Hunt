using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageIndex : MonoBehaviour
{
    [SerializeField] GameObject[] pages;
    private GameObject currentPage;
    [SerializeField] private int currentPageIndex;
    [SerializeField] TMPro.TMP_Text cpText;

    private void Awake()
    {
        currentPage = pages[0];
        currentPageIndex = 0;
    }
    private void Update()
    {
        cpText.text = currentPageIndex + 1 + "/" + pages.Length;
    }
    public void RightPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            pages[currentPageIndex + 1].SetActive(true);
            currentPage.SetActive(false);
            currentPage = pages[currentPageIndex + 1];
            currentPageIndex = currentPageIndex + 1;
        }
    }
    public void LeftPage()
    {
        if (currentPageIndex > 0)
        {
            pages[currentPageIndex - 1].SetActive(true);
            currentPage.SetActive(false);
            currentPage = pages[currentPageIndex - 1];
            currentPageIndex = currentPageIndex - 1;
        }
    }
}
