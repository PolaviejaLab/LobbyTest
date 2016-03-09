using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;


public class ICExperimentSetupController: MonoBehaviour 
{
    private enum State { ExperimentBrowser, ServerBrowser, ServerLobby, ClientLobby };

    private State currentState = State.ExperimentBrowser;

    public ICExperimentBrowserController experimentBrowser;
    public ICServerBrowserController serverBrowser;
    public ICLobbyController lobby;

    private GameObject[] panels = new GameObject[3];


    void Awake()
    {
        if(!experimentBrowser) throw new Exception("experimentBrowser field not set.");
        if(!serverBrowser) throw new Exception("serverBrowser field not set.");
        if(!lobby) throw new Exception("lobby field not set.");

        panels[0] = experimentBrowser.gameObject;
        panels[1] = serverBrowser.gameObject;
        panels[2] = lobby.gameObject;
    }


    /**
     * Change the currently active panel.
     */
    private void SwitchToPanel(GameObject panel)
    {
        for(var i = 0; i < panels.Length; i++) {
            if(!panel) continue;

            panels[i].SetActive(panels[i] == panel);
        }
    }


    /**
     * Start the experiment browser.
     */
    public void StartExperimentBrowser()
    {
        SwitchToPanel(experimentBrowser.gameObject);
        currentState = State.ExperimentBrowser;
    }


    /**
     * Start the server browser.
     */
    public void StartServerBrowser() 
    {
        SwitchToPanel(serverBrowser.gameObject);
        currentState = State.ServerBrowser;
    }


    /**
     * Start the lobby in server-mode.
     */
    public void StartServer(ICExperiment experiment)
    {
        SwitchToPanel(lobby.gameObject);
        // lobby.startServer(experiment)

        currentState = State.ServerLobby;
    }


    /**
     * Start the lobby in client-mode.
     */
    public void StartClient(string address, int port)
    {
        SwitchToPanel(lobby.gameObject);
        // lobby.StartClient(address, port);

        currentState = State.ClientLobby;
    }


    /**
     * Start the experiment (by changing the scene)
     */
    public void StartExperiment(ICExperiment experiment)
    {
        if(!experiment) return;

        // Switch back to neutral panel
        SwitchToPanel(experimentBrowser.gameObject);
        currentState = State.ExperimentBrowser;

        // Change level
        SceneManager.LoadScene(experiment.sceneName);
    }


    /**
     * Abort current state, moving back to previous state.
     */
    public void Cancel()
    {
        switch(currentState) {
            case State.ExperimentBrowser: break;
            case State.ServerBrowser: StartExperimentBrowser(); break;
            case State.ServerLobby: StartExperimentBrowser(); break;
            case State.ClientLobby: StartServerBrowser(); break;

            default: StartExperimentBrowser(); break;
        }
    }
}
