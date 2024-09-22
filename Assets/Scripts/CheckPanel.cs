using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class CheckPanel : MonoBehaviour
{
    [SerializeField] public GameObject panel;

    void Start()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
