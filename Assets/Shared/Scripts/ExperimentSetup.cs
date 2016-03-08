using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * Responsible for bootstrapping an experiment
 */
public class ExperimentSetup: MonoBehaviour 
{
    [Tooltip("Name shown in the UI")]
    public string friendlyName = "";

    [Tooltip("Minimum number of participants")]
    public int minimumParticipants = 1;

    [Tooltip("Maximum number of participants")]
    public int maximumParticipants = 1;

    [Tooltip("Name of the scene to transition to")]
    public string sceneName;

    [Tooltip("Use this prefab/object as a panel while configuring the experiment")]
    public bool showPanel = false;


    /**
     * Returns display name
     */
    public string getDisplayName() {
        if(friendlyName != "")
            return friendlyName;
        if(name != "")
            return name;
        return "Unnamed experiment";
    }
}
