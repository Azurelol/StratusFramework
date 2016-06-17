/******************************************************************************/
/*!
@file   MainMenuManager.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;

/**************************************************************************/
/*!
@class MainMenuManager 
*/
/**************************************************************************/
public class MainMenuManager : MonoBehaviour {

  public string MainMenu;
  public string NewGame;
  public string Credits;

  /**************************************************************************/
  /*!
  @brief  Initializes the Script.
  */
  /**************************************************************************/
  void Start () {
    //DontDestroyOnLoad(this.gameObject);
  }
  public void StartNewGame()
  {
    Time.timeScale = 1.0f;
    SceneManager.LoadScene(NewGame);
  }

  public void Continue()
  {
    // Load the last played level?
  }

  public void LoadCredits()
  {
    SceneManager.LoadScene(Credits);
  }

  public void BackToMainMenu()
  {
    SceneManager.LoadScene(MainMenu);
  }

  public void Quit()
  {
    // Perhaps save before exiting
    Application.Quit();
  }

}
