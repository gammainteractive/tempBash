using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WatchPrompt : MonoBehaviour {

    TextMeshProUGUI promptText;
    string promptMessage = "Watch!";
    string inputMessage = "Your Turn!";

	// Use this for initialization
	void Awake () {
       // promptText = GetComponentInChildren<TextMeshProUGUI>();
       // ActivatePromptText(false);
    }

    public void ActivatePromptText(bool active)
    {
       // promptText.enabled = active;
    }

    public void Prompt()
    {
        //promptText.text = promptMessage;
    }
    public void Input()
    {
       // promptText.text = inputMessage;
    }


}
