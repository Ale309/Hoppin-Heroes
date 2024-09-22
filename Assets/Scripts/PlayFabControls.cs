using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class PlayFabControls : MonoBehaviour
{

	[SerializeField] GameObject signUpTab, logInTab;
	public Text username,
				userEmailSignUp, userPasswordSignUp,
				userEmailReset, infoReset,
				userEmailLogIn, userPasswordLogIn, 
				errorSignUp, errorLogIn;
	public string usernameDisplay;
	string encryptedPassword;

	public GameObject rowPrefab;
	public Transform rowsParent;

	public void SwitchToSignUpTab()
	{
		signUpTab.SetActive(true);
		logInTab.SetActive(false);
		errorSignUp.text = "";
		errorLogIn.text = "";
	}

	public void SwitchToLoginTab()
	{
		signUpTab.SetActive(false);
		logInTab.SetActive(true);
		errorSignUp.text = "";
		errorLogIn.text = "";
	}

	public void SignUp()
	{
		if(userPasswordSignUp.text.Length < 6){
			errorSignUp.text = "Password too short";
			return;
		}
		var registerRequest = new RegisterPlayFabUserRequest {Email = userEmailSignUp.text, Password = userPasswordSignUp.text, Username = username.text};
		PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterError);
	}

	public void RegisterSuccess(RegisterPlayFabUserResult result)
	{
		//errorSignUp.text = "";
		//errorLogIn.text = "";
		StartGame();
		//SwitchToLoginTab();
	}

		public void RegisterError(PlayFabError error)
	{
		errorSignUp.text = error.GenerateErrorReport();
		if(errorSignUp.text == "/Client/RegisterPlayFabUser: Invalid input parameters\nEmail: Email address is not valid."){
			errorSignUp.text = "Invalid email format. Please check your email.";
		}
		else if(errorSignUp.text == "/Client/RegisterPlayFabUser: Invalid input parameters\nEmail: Email address not available"){
			errorSignUp.text = "Email already in use. Please try a different email.";
		}
	}



	public void LogIn()
	{
		var request = new LoginWithEmailAddressRequest {Email = userEmailLogIn.text, Password = userPasswordLogIn.text};
		PlayFabClientAPI.LoginWithEmailAddress(request, LogInSuccess, LogInError);
	}


	public void LogInSuccess(LoginResult result)
	{
		errorSignUp.text = "";
		errorLogIn.text = "";
		GetUserAccountInfo();
		StartGame();
	}


	public void LogInError(PlayFabError error)
	{
		errorLogIn.text = error.GenerateErrorReport();
		if(errorLogIn.text == "/Client/LoginWithEmailAddress: Invalid input parameters\nEmail: Email address is not valid."){
			errorLogIn.text = "This email is not registered. Try again.";
		}
	}

	public void ResetPassword(){
		var request = new SendAccountRecoveryEmailRequest {
			Email = userEmailReset.text,
			TitleId = "E52BA"
		};
		PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordResetSuccess, OnPasswordResetError);
	}

	void OnPasswordResetSuccess(SendAccountRecoveryEmailResult result){
		infoReset.text = "Password reset email sent!";
	}

	void OnPasswordResetError(PlayFabError error)
	{
		infoReset.text = "Error sending password reset email: " + error.GenerateErrorReport();
	}


	public void StartGame()
	{
		SceneManager.LoadScene(0);
	}

	void GetUserAccountInfo()
	{
		if (!string.IsNullOrEmpty(PlayFabSettings.staticPlayer.PlayFabId))
		{
			var request1 = new GetAccountInfoRequest { PlayFabId = PlayFabSettings.staticPlayer.PlayFabId };
			PlayFabClientAPI.GetAccountInfo(request1, OnGetAccountInfoSuccess, OnGetAccountInfoError);
		}
		else
		{
			errorLogIn.text = "PlayFabId non disponibile. Riprova il login.";
		}

        var request = new GetAccountInfoRequest { PlayFabId = PlayFabSettings.staticPlayer.PlayFabId };
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoError);

    }

    void OnGetAccountInfoSuccess(GetAccountInfoResult result)
	{
        usernameDisplay = result.AccountInfo.Username;
    }

    void OnGetAccountInfoError(PlayFabError error)
	{
        errorLogIn.text = error.GenerateErrorReport();
    }


	public void SendLeaderboard(float elapsedTime){
    // Convertiamo il tempo da secondi a millisecondi
    int scoreInMilliseconds = Mathf.RoundToInt(elapsedTime * 1000); 

    var request = new UpdatePlayerStatisticsRequest {
        Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = "PlatformScore",
                Value = scoreInMilliseconds
            }
        }
    };
    PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdateSuccess, OnLeaderboardUpdateError);
}


	void OnLeaderboardUpdateSuccess(UpdatePlayerStatisticsResult result){
		Debug.Log("Successfull leaderboard sent.");
	}

	void OnLeaderboardUpdateError(PlayFabError error)
	{
		Debug.LogError("Error while updating leaderboard: " + error.GenerateErrorReport());
	}

	public void GetLeaderboard(){
		var request = new GetLeaderboardRequest {
			StatisticName = "PlatformScore",
			StartPosition = 0,
			MaxResultsCount = 10
		};
		PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGetSuccess, OnLeaderboardGetError);
	}

	void OnLeaderboardGetSuccess(GetLeaderboardResult result)
	{
		// Rimuovere gli elementi esistenti nella UI
		foreach (Transform item in rowsParent)
		{
			Destroy(item.gameObject);
		}

		// Invertire la classifica se necessario
		result.Leaderboard.Reverse();

		// Variabile per tenere traccia della posizione manualmente
		int position = 1;

		foreach (var item in result.Leaderboard)
		{
			GameObject newGo = Instantiate(rowPrefab, rowsParent);

			Text[] texts = newGo.GetComponentsInChildren<Text>();

			texts[0].text = position.ToString(); // Posizione manuale
			texts[1].text = "Loading..."; // Placeholder per l'username

			float val = item.StatValue / 1000f;
			int minutes = Mathf.FloorToInt(val / 60);
            int seconds = Mathf.FloorToInt(val % 60);
            int milliseconds = Mathf.FloorToInt((val - Mathf.Floor(val)) * 1000);

			texts[2].text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds); // Valore della statistica

			// Incrementare il contatore della posizione
			position++;

			// Ottieni le informazioni sull'account usando PlayFabId
			GetUsername(item.PlayFabId, texts[1]); // texts[1] verrà aggiornato con l'username
		}
	}


	void GetUsername(string playFabId, Text usernameText)
	{
		var request = new GetAccountInfoRequest
		{
			PlayFabId = playFabId
		};

		PlayFabClientAPI.GetAccountInfo(request, result =>
		{
			string username = result.AccountInfo.Username;
			usernameText.text = username; // Aggiorna il campo con l'username
		},
		error =>
		{
			Debug.LogError("Error retrieving account info: " + error.GenerateErrorReport());
			usernameText.text = "Unknown"; // In caso di errore
		});
	}



	void OnLeaderboardGetError(PlayFabError error)
	{
		Debug.LogError("Error while retrieving leaderboard: " + error.GenerateErrorReport());
	}


}
