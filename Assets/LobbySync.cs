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
        Debug.LogError("Requested update of client list...");
        RpcUpdateClientList();
    }


    [ClientRpc]
    public void RpcUpdateClientList() {
        // Find lobby object and controller
        ICLobbyController lobbyController = GameObject.Find("Lobby").GetComponent<ICLobbyController>();

        Debug.LogError("Updating player list...");
        Debug.LogError(lobbyController.isClient);

        // The server maintains the master copy, so only update clients
        if(lobbyController.isClient) {
            lobbyController.participantList.items.Clear();
            lobbyController.participantList.items.Add("Client", "Client");
        }
    }

}
