/******************************************************************************/
/*!
@file   PauseMenuManager.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;
using Stratus;

/**************************************************************************/
/*!
@class PauseMenuManager 
*/
/**************************************************************************/
public class PauseMenuManager : MonoBehaviour {

  public bool Debugging = true;
  public string SceneName = "Pause";
  private bool Active { get; set; }
  private Scene PauseScene;
  private Scene CurrentScene;

  /**************************************************************************/
  /*!
  @brief  Initializes the Script.
  */
  /**************************************************************************/
  void Start() {
    DontDestroyOnLoad(this.gameObject);
    if (FindObjectsOfType(GetType()).Length > 1)
      Destroy(gameObject);
    
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      this.Active = !this.Active;
      if (Active) this.Open();
      else this.Close();
    }
  }

  void Open()
  {
    Trace.Script("Opening!");
    // Save a reference to the current scene
    CurrentScene = SceneManager.GetActiveScene();
    // Pause the current scene
    Time.timeScale = 0.0f;
    // Load the pause scene
    SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
    PauseScene = SceneManager.GetSceneByName(SceneName);
    //SceneManager.SetActiveScene(PauseScene);
    
  }

  void Close()
  {
    Trace.Script("Closing!");
    // Unload the pause scene
    SceneManager.UnloadScene(PauseScene.buildIndex);
    // Restore the current scene
    //SceneManager.SetActiveScene(CurrentScene);
    // Pause the current scene
    Time.timeScale = 1.0f;
  }
  
}
