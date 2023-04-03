using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField creatingRoomNameInputField;
    [SerializeField] private TMP_InputField joiningRoomNameInputField;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private float infoTextShowSeconds = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        createRoomButton.onClick.AddListener( () => CreateRoom() );
        joinRoomButton.onClick.AddListener( () => JoinRoom() );
        exitButton.onClick.AddListener( () => QuitGame() );

        usernameInputField.text = PlayerPrefs.GetString("playerName");
        PhotonNetwork.NickName = usernameInputField.text;
        usernameInputField.onValueChanged.AddListener( (value) => { PlayerPrefs.SetString("playerName", value); PhotonNetwork.NickName = value; } );
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(creatingRoomNameInputField.text, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32766)
        {
            SetInfoText("Комната с таким именем уже существует", true);
        }
        else
        {
            SetInfoText(returnCode + ": " + message, true);
        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joiningRoomNameInputField.text);
    }

    public override void OnJoinedRoom()
    {
        SetInfoText("Производится вход в комнату " + joiningRoomNameInputField.text, false);
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32765)
        {
            SetInfoText("Комната заполнена", true);
        }
        else if (returnCode == 32764)
        {
            SetInfoText("Данная комната была закрыта", true);
        }
        else if (returnCode == 32758)
        {
            SetInfoText("Комнаты с таким именем не существует", true);
        }
        else
        {
            SetInfoText(returnCode + ": " + message, true);
        }
    }

    public void SetInfoText(string message, bool error)
    {
        infoText.color = error ? Color.red : Color.green;

        infoText.text = message;

        StopCoroutine("HideInfoText");

        StartCoroutine("HideInfoText");
    }

    IEnumerator HideInfoText()
    {
        yield return new WaitForSeconds(infoTextShowSeconds);

        infoText.text = "";
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
