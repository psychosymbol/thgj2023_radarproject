using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentController : MonoBehaviour
{
    public int currentPage = 0;
    public int maxPage = 4;

    public List<GameObject> documentPages = new List<GameObject>();

    private Animator documentAnim;

    private void Start()
    {
        documentAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        foreach (var item in documentPages)
        {
            if (item.name.Contains(currentPage.ToString()))
                item.SetActive(true);
            else
                item.SetActive(false);
        }
    }

    public void nextPage()
    {
        currentPage = currentPage + 1 >= maxPage ? 0 : currentPage + 1;
    }

    public void toggleDocument()
    {
        documentAnim.SetBool("Open", !documentAnim.GetBool("Open"));
    }
}
