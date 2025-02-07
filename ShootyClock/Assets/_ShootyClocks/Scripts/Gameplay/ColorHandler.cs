using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;


public class ColorHandler : MonoBehaviour
{
    Image imgComp;
    TMP_Text txtComp;


    void Start()
    {
        imgComp = GetComponent<Image>();
        txtComp = GetComponent<TMP_Text>();
    }
    // Update is called once per frame
    void Update()
    {
        // Change color to the background color of the main camera.
        if (gameObject.activeInHierarchy)
        {
            if (imgComp != null)
            {
                imgComp.color = Camera.main.backgroundColor;
            }

            if (txtComp != null)
            {
                txtComp.color = Camera.main.backgroundColor;
            }
        }
    }
}
