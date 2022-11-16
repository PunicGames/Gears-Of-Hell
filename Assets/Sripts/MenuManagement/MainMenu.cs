using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Transform cameraDefaultPos;
    [SerializeField]
    private Transform cameraSelectorPos;

    public GameObject[] characters;
    private int selectedCharacter = 0;
    private int previousSelected = 0;

    // Cursor
    // Cursor
    [SerializeField] private Texture2D cursorSprite;
    private Vector2 cursorHotSpot;

    private void Start()
    {
        cursorHotSpot = new Vector2(0, 0);
        Cursor.SetCursor(cursorSprite, cursorHotSpot, CursorMode.Auto);
    }

    public void PlayGame() {
        SceneManager.LoadScene("MainScene");
    }

    public void SelectJacob()
    {
        characters[1].SetActive(false);
        selectedCharacter = 0;
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        characters[0].SetActive(true);
        characters[0].transform.rotation = characters[previousSelected].transform.rotation;
        previousSelected = 0;
    }

    public void SelectBrenda()
    {
        characters[1].SetActive(true);
        selectedCharacter = 1;
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        characters[0].SetActive(false);
        characters[1].transform.rotation = characters[previousSelected].transform.rotation;
        previousSelected = 1;
    }

    public void MenuToSelector() {
        //characters[selectedCharacter].SetActive(true);
        Camera.main.transform.position = cameraSelectorPos.transform.position;
    }

    public void SelectorToMenu()
    {
        //characters[selectedCharacter].SetActive(false);
        Camera.main.transform.position = cameraDefaultPos.transform.position;
    }

    public void MenuToOptions()
    {
        characters[selectedCharacter].SetActive(false);
    }

    public void OptionsToMenu()
    {
        characters[selectedCharacter].SetActive(true);
    }
}
