using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

public class ICLobbyController : MonoBehaviour 
{
    public ICExperimentSetupController experimentSetup;

    public ICListBox participantList;
    public Button startButton;
    public Button cancelButton;

    private bool isClient = false;
    private bool isServer = false;


    void Awake()
    {
        if(!experimentSetup) throw new Exception("experimentSetup field not set.");
        if(!cancelButton) throw new Exception("cancelButton field not set.");
        if(!startButton) throw new Exception("startButton field not set.");

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => { experimentSetup.Cancel(); });
    }


    void OnEnable()
    {
    }


    void UpdateParticipantList()
    {
        var networkManager = ICNetworkUtilities.GetNetworkManager();

        Debug.Log("Updating list of participants, found: " + networkManager.connections.Count.ToString());

        participantList.items.Clear();
        for(var i = 0 ; i < networkManager.connections.Count; i++) {
            participantList.items.Add(
                networkManager.connections[i].address,
                networkManager.connections[i].address);
        }
    }

   

    public void StartServer(ICExperiment experiment)
    {
        var networkDiscovery = ICNetworkUtilities.GetNetworkDiscovery();
        var networkManager = ICNetworkUtilities.GetNetworkManager();

        networkManager.ClientConnect.RemoveAllListeners();
        networkManager.ClientDisconnect.RemoveAllListeners();
        networkManager.ServerConnect.RemoveAllListeners();
        networkManager.ServerDisconnect.RemoveAllListeners();

        networkManager.ClientConnect.AddListener((client) => { UpdateParticipantList(); });
        networkManager.ClientDisconnect.AddListener((client) => { UpdateParticipantList(); });
        networkManager.ServerConnect.AddListener((client) => { UpdateParticipantList(); });
        networkManager.ServerDisconnect.AddListener((client) => { UpdateParticipantList(); });

        networkManager.StartHost();

        networkDiscovery.Initialize();
        networkDiscovery.broadcastData = 
            "NetworkManager:" + 
            networkManager.networkAddress + ":" +
            networkManager.networkPort + ":" +
            experiment.getDisplayName();

        if(!networkDiscovery.StartAsServer()) {
            throw new Exception("StartAsServer returned false in ICLobbyController.");
        }

        startButton.enabled = true;
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => { experimentSetup.StartExperiment(experiment); });

        isServer = true;
    }


    public void StartClient(string address, int port)
    {
        var networkManager = ICNetworkUtilities.GetNetworkManager();

        Debug.Log("Connecting to '" + address + "' at port " + port.ToString());

        networkManager.ClientError.RemoveAllListeners();
        networkManager.ClientError.AddListener((conn, code) => { Debug.LogError("Error while connecting to server, code: " + code.ToString()); });

        networkManager.ClientConnect.RemoveAllListeners();
        networkManager.ClientConnect.AddListener((conn) => { Debug.Log("Connecting to server..."); });

        networkManager.networkAddress = address;
        networkManager.networkPort = port;
        networkManager.StartClient();

        startButton.enabled = false;
        startButton.onClick.RemoveAllListeners();

        isClient = true;
    }


    public void StopBroadcast()
    {
        var networkDiscovery = ICNetworkUtilities.GetNetworkDiscovery();

        if(networkDiscovery.isServer)
            networkDiscovery.StopBroadcast();
    }


    public void StopAll()
    {
        var networkDiscovery = ICNetworkUtilities.GetNetworkDiscovery();
        var networkManager = ICNetworkUtilities.GetNetworkManager();

        if(isServer) {
            networkManager.StopHost();
            if(networkDiscovery.isServer)
                networkDiscovery.StopBroadcast();
            isServer = false;
        }

        if(isClient) {
            networkManager.StopClient();
            isClient = false;
        }
    }
}
