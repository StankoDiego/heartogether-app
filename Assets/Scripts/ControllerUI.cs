using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerUI : MonoBehaviour
{
    public GameObject modal;

    private void Start()
    {
        modal.SetActive(false);
    }
    
    public void OpenModal()
    {
        modal.SetActive(true);
    }

    public void CloseModal()
    {
        modal.SetActive(false);
    }
}
