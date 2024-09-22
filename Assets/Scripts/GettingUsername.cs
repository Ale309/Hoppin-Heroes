using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class GettingUsername : MonoBehaviour
{
    [SerializeField] public Text usernameDisplay;

    void Start()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            GetUserAccountInfo();
        }
    }

    void GetUserAccountInfo()
	{
        var request = new GetAccountInfoRequest { PlayFabId = PlayFabSettings.staticPlayer.PlayFabId };
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoError);
    }

    void OnGetAccountInfoSuccess(GetAccountInfoResult result)
	{
        usernameDisplay.text = result.AccountInfo.Username;
    }

    void OnGetAccountInfoError(PlayFabError error)
	{

    }
}
