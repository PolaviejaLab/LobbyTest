using UnityEngine;

using System;
using System.Collections;

public class ICLobbyController : MonoBehaviour 
{
    public ICExperimentSetupController experimentSetup;

    private bool isClient = false;
    private bool isServer = false;


    void Awake()
    {
        if(!experimentSetup) throw new Exception("experimentSetup field not set.");
    }


    void OnEnable()
    {
    }


    public void StartServer(ICExperiment experiment)
    {
        var networkDiscovery = ICNetworkUtilities.GetNetworkDiscovery();
        var networkManager = ICNetworkUtilities.GetNetworkManager();

        networkManager.StartHost();

        networkDiscovery.Initialize();
        networkDiscovery.broadcastData = 
            "NetworkManager:" + 
            networkManager.networkAddress + ":" +
            networkManager.networkPort + ":" +
            experiment.getDisplayName();

        isServer = true;
    }


    public void StartClient(string address, int port)
    {
        var networkManager = ICNetworkUtilities.GetNetworkManager();

        networkManager.networkAddress = address;
        networkManager.networkPort = port;
        networkManager.StartClient();

        isClient = true;
    }


    void OnDisable()
    {
        var networkDiscovery = ICNetworkUtilities.GetNetworkDiscovery();
        var networkManager = ICNetworkUtilities.GetNetworkManager();

        if(isServer) {
            networkManager.StopHost();
            networkDiscovery.StopBroadcast();
            isServer = false;
        }

        if(isClient) {
            networkManager.StopClient();
            isClient = false;
        }
    }
}
