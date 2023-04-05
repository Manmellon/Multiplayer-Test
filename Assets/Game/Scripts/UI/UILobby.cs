using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;

public class UILobby : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField creatingRoomNameInputField;
    [SerializeField] private TMP_InputField joiningRoomNameInputField;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private float infoTextShowSeconds = 3.0f;

    public static UILobby singleton;

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        createRoomButton.onClick.AddListener(() => Lobby.singleton.CreateRoom(creatingRoomNameInputField.text));
        joinRoomButton.onClick.AddListener(() => Lobby.singleton.JoinRoom(joiningRoomNameInputField.text));
        exitButton.onClick.AddListener(() => Lobby.singleton.QuitGame());

        usernameInputField.text = PlayerPrefs.GetString("playerName");
        PhotonNetwork.NickName = usernameInputField.text;
        usernameInputField.onValueChanged.AddListener((value) => { PlayerPrefs.SetString("playerName", value); PhotonNetwork.NickName = value; });
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

    public string GetJoiningRoomName()
    {
        return joiningRoomNameInputField.text;
    }
}
