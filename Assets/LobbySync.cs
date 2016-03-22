using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LobbySync : NetworkBehaviour {

    public void UpdateClientList()
    {
        CmdUpdateClientList();
    }

    [Command]
    public void CmdUpdateClientList() {
        ICLobbyController lobbyController = GameObject.Find("Lobby").GetComponent<ICLobbyController>();

        RpcClearParticipantList();
        foreach(var item in lobbyController.participantList.items) {
            RpcAddParticipant(item.Key, item.Value);
        }
    }


    [ClientRpc]
    public void RpcClearParticipantList()
    {
        ICLobbyController lobbyController = GameObject.Find("Lobby").GetComponent<ICLobbyController>();

        if(lobbyController.isClient)
            lobbyController.participantList.items.Clear();
    }


    [ClientRpc]
    public void RpcAddParticipant(string key, string value)
    {
        ICLobbyController lobbyController = GameObject.Find("Lobby").GetComponent<ICLobbyController>();

        if(lobbyController.isClient)
            lobbyController.participantList.items.Add(key, value);
    }

}
