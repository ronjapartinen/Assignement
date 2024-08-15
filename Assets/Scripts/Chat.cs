using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
using UnityEngine.UI;

public class Chat : NetworkBehaviour
{

    [SerializeField] InputReader inputReader;
    [SerializeField] TextMeshProUGUI textPlayer1;
    [SerializeField] TextMeshProUGUI textPlayer2;
    [SerializeField] TMP_InputField inputText;


    private void OnSend()
    {
        PlayerPrefs.SetString("message", inputText.text);
        Debug.Log("Message is " + PlayerPrefs.GetString("message"));
        FixedString128Bytes message = PlayerPrefs.GetString("message");

        UpdateMessageRPC(message);
        UpdateMessageForMeRPC(message);
    }


    [Rpc(SendTo.NotMe)]
    public void UpdateMessageRPC(FixedString128Bytes message)
    {
        textPlayer2.text = message.ToString();
        Debug.Log("Message Received");
    }

    [Rpc(SendTo.Me)]
    public void UpdateMessageForMeRPC(FixedString128Bytes message)
    {
        textPlayer1.text = message.ToString();
        Debug.Log("Message Received");
    }

    void Start()
    {
        if(inputReader != null)
        {
            inputReader.SendEvent += OnSend;
        }
    }


    void Update()
    {
        
    }
}
