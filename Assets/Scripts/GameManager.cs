using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Scene Management")] 
    public Canvas datingSimInterface;
    public EventSystem datingSimEventSystem;
    private bool fpsLoaded = false;
    
    [Header("Characters")] 
    public Image characterImage;
    public Characters characterSO;

    // This should be populated in Unity Editor
    // Variable to store different ending screens
    // string: The name of the ending screen (for function call)
    // GameObject: The actual object of the ending screen (like an image)
    [Header("EndScreens")] public Dictionary<string, GameObject> EndScreens;
    
    void Update()
    { 
        // this is just for testing additive scene loading
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (fpsLoaded) EndFPS();
            else StartFPS();
        }
    }
    
    public void StartFPS()
    {
        fpsLoaded = true;
        
        // hides dating sim ui
        datingSimInterface.enabled = false;
        characterImage.enabled = false;
        
        // handles event system control over to FPS scene
        datingSimEventSystem.enabled = false;

        // load in fps scene
        Cursor.lockState = CursorLockMode.Locked; // also done in the FirstPersonCamera script but here again just in case
        SceneManager.LoadScene("FPSScene", LoadSceneMode.Additive);
    }
    public void EndFPS()
    {
        fpsLoaded = false;
        
        // enable dating sim ui
        datingSimInterface.enabled = true;
        characterImage.enabled = true;

        // handles event system control back
        datingSimEventSystem.enabled = true;
        
        // unload fps scene
        Cursor.lockState = CursorLockMode.None;
        SceneManager.UnloadSceneAsync("FPSScene");
    }
    
    // boilerplate to expose a method to yarn runtime
    // https://docs.yarnspinner.dev/using-yarnspinner-with-unity/creating-commands-functions
    [YarnCommand("TestYarnUnityIntegration")]
    public static void TestYarnUnityIntegration() {
        Debug.Log($"I am called from yarn :)");
    }
    
    [YarnCommand("SetSprite")]
    public void SetSprite(string characterName) 
    {
        Debug.Log($"Switching to {characterName}");
        // Find the character in the CharacterList by name
        Character character = characterSO.CharacterList.Find(c => c.CharacterName == characterName);
        if (character != null)
        {
            characterImage.sprite = character.CharacterImage;
        }
        else
        {
            Debug.LogWarning($"Character '{characterName}' not found.");
        }
    }

    // Function that can be called by Yarn to change game view
    [YarnCommand("ChangeMode")]
    public void ChangeMode(bool fpsMode)
    {
        if (fpsMode)
        {
            StartFPS();
        } else
        {
            EndFPS();
        }
    }

    // This is a Yarn-callable function that shows screen if existed by name
    [YarnCommand("ShowEndScreen")]
    public void ShowEndScreen(string screenName)
    {
        if (EndScreens.TryGetValue(screenName, out var screenObj))
        {
            screenObj.SetActive(true);
        }
    }
    
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
