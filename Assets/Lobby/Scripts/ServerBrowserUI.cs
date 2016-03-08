using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerBrowserUI : MonoBehaviour 
{
    public ListBox serverList;
    public Button connectButton;
    public Button cancelButton;

    public PanelSwitcher panelSwitcher;
    public GameObject backPanel;


    private WatchedNetworkDiscovery GetNetworkDiscovery()
    {
        GameObject networkManagerObject = GameObject.Find("NetworkManager");
        if(networkManagerObject == null) return null;

        return networkManagerObject.GetComponent<WatchedNetworkDiscovery>();
    }



    private void CancelBrowsing()
    {
        if(panelSwitcher && backPanel)
            panelSwitcher.ShowPanel(backPanel);
    }


	// Use this for initialization
	void Start () {
	    if(cancelButton) {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelBrowsing);
        }

        Debug.Log("Server browser...");
	}

    void UpdateServers() {
        WatchedNetworkDiscovery networkDiscovery = GetNetworkDiscovery();
        if(networkDiscovery == null) return;

        serverList.items.Clear();
        foreach(var item in networkDiscovery.servers) {
            serverList.items.Add(item.Key, item.Key);
        }
    }

    void OnEnable() {
        WatchedNetworkDiscovery networkDiscovery = GetNetworkDiscovery();
        if(networkDiscovery == null) return;

        networkDiscovery.OnNewServer.RemoveAllListeners();
        networkDiscovery.OnNewServer.AddListener((a, b) => { UpdateServers(); });

        networkDiscovery.Initialize();
        networkDiscovery.StartAsClient();

        Debug.Log("Waiting for servers...");
    }

    void OnDisable() {
        WatchedNetworkDiscovery networkDiscovery = GetNetworkDiscovery();
        if(networkDiscovery == null) return;

        if(networkDiscovery.hostId != -1 || networkDiscovery.running)
            networkDiscovery.StopBroadcast();

        Debug.Log("No longer waiting for servers.");
    }	
}
