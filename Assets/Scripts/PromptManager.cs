using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public TMP_InputField promptInput;
    public InputsHandler inputsHandler;
    public Rebellis rebellis;
    public Button sendPromptButton;
    public TextMeshProUGUI generatingText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            inputsHandler.isInPromptMode = !inputsHandler.isInPromptMode;
            promptInput.gameObject.SetActive(!promptInput.gameObject.activeInHierarchy);
            Cursor.visible = !Cursor.visible;
        }
    }

    public void SendPrompt()
    {
        // Like "A person is performing a ballet dance"
        generatingText.gameObject.SetActive(true);
        sendPromptButton.interactable = false;
        StartCoroutine(rebellis.GenerateModel(promptInput.text));
    }
}
