using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VentReplayScript : MonoBehaviour
{
    public Transform ContentHolder;

    public GameObject StartLevelButton;

    public void Start()
    {
        PopulateMenu();
    }

    public void PopulateMenu()
    {
        foreach(string LevelName in PurificationHolderScript.LevelHolders.Keys)
        {
            GameObject newButton = Instantiate(StartLevelButton, ContentHolder);

            newButton.GetComponentInChildren<TMP_Text>().text = LevelName;
            newButton.GetComponent<Button>().onClick.AddListener(() => StartVentMinigame(LevelName));
        }
    }

    public void StartVentMinigame(string KeyName)
    {
        PurificationHolderScript.LevelHolders[KeyName].LevelStart();
        PurificationGameScript.instance.StartGame();
    }
}
