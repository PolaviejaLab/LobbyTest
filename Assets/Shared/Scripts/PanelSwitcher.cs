using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelSwitcher : MonoBehaviour 
{
    public GameObject startPanel;

    public void ShowPanel(GameObject panel) 
    {
        for(var i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            child.SetActive(child == panel);
        }
    }

	void Start () {
	    ShowPanel(startPanel);
	}	
}
