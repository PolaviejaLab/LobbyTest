using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ExperimentSelectionUI: MonoBehaviour 
{
    public ListBox listBox;
    public Button configureButton;
    public Button joinButton;

    public PanelSwitcher panelSwitcher;
    public GameObject serverBrowserPanel;
    public GameObject lobbyPanel;

    public ExperimentSetup[] experiments;



    /**
     * Add items from experiments field to listbox
     */
    private void RefreshExperimentListBox()
    {
        if(listBox == null) return;

        listBox.items.Clear();
        for(var i = 0; i < experiments.Length; i++) {
            if(experiments[i] == null) continue;

            string name = experiments[i].getDisplayName();
            listBox.items.Add(i.ToString(), name);
        }
    }


    /**
     * Open experiment configuration interface
     */
    private void OpenConfigureInterface()
    {        
        if(listBox == null) return;

        int selection;

        if(!int.TryParse(listBox.selectedItem, out selection)) {
            Debug.LogWarning("Cannot configure experiment, invalid selection.");
            return;
        }

        ExperimentSetup experiment = experiments[selection];

        if(experiment.showPanel) {
            // Instantiate configuration panel
        } else {
            if(experiment.maximumParticipants > 1) {
                panelSwitcher.ShowPanel(lobbyPanel);
            } else {
                SceneManager.LoadScene(experiment.sceneName);
            }
        }
    }


    /**
     * Start search for servers
     */
    private void OpenJoinInterface()
    {
        if(panelSwitcher && serverBrowserPanel)
            panelSwitcher.ShowPanel(serverBrowserPanel);
    }


	void Start () 
    {
        RefreshExperimentListBox();

        if(configureButton) {
            configureButton.onClick.RemoveAllListeners();
            configureButton.onClick.AddListener(OpenConfigureInterface);
        }

        if(joinButton) { 
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(OpenJoinInterface);
        }
	}   
}
