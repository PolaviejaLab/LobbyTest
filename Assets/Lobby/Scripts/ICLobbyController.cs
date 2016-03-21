using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using System;
using System.Collections;

public class ICLobbyController : MonoBehaviour 
{
    public ICExperimentSetupController experimentSetup;

    public ICListBox participantList;
    public Button startButton;
    public Button cancelButton;

    private LobbySync syncScript = null;

    private bool _isClient = false;
    private bool _isServer = false;   

    public bool isClient { get { return _isClient; } }
    public bool isServer { get { return _isServer; } }


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

        networkManager.ClientConnect.AddListener((client) => { UpdateParticipantList(); if(syncScript) syncScript.UpdateClientList(); });
        networkManager.ClientDisconnect.AddListener((client) => { UpdateParticipantList(); });
        networkManager.ServerConnect.AddListener((client) => { UpdateParticipantList(); if(syncScript) syncScript.UpdateClientList(); });
        networkManager.ServerDisconnect.AddListener((client) => { UpdateParticipantList(); });
        networkManager.ServerReady.AddListener((client) => { UpdateParticipantList(); if(syncScript) syncScript.UpdateClientList(); });

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

        if(syncScript) {
            //NetworkServer.Destroy(syncScript.gameObject);
            Destroy(syncScript.gameObject);
        }

        // Create synchronization script
        syncScript = GameObject.Instantiate(networkManager.spawnPrefabs[0]).GetComponent<LobbySync>();
        NetworkServer.Spawn(syncScript.gameObject);
        syncScript.transform.SetParent(gameObject.transform);

        _isServer = true;
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

        _isClient = true;
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

        if(_isServer) {
            networkManager.StopHost();
            if(networkDiscovery.isServer)
                networkDiscovery.StopBroadcast();
            _isServer = false;
        }

        if(isClient) {
            networkManager.StopClient();
            _isClient = false;
        }
    }
}
