using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class PauseMenu : MonoBehaviour
{

  public GameObject menu;
  public bool isPaused;
  private PlayerInput _input;

  private void Awake()
  {
    _input = new PlayerInput();
    _input.CharacterControls.Pause.started += OnPause;

    isPaused = false;
    menu.SetActive(false);
  }

  public void OnPause(InputAction.CallbackContext obj)
  {
    pauseGame();
  }

  public void pauseGame()
  {
    if (isPaused)
    {
      isPaused = false;
      Time.timeScale = 1f;
      toggleMenu();
    }
    else
    {
      isPaused = true;
      Time.timeScale = 0f;
      toggleMenu();
    }
  }

  private void toggleMenu()
  {
    menu.SetActive(isPaused);
  }

  public void restartGame()
  {
    isPaused = true;
    pauseGame();
    SceneManager.LoadScene("GameScene");
  }

  public void quitGame()
  {
    Application.Quit();
  }

  void OnEnable()
  {
    _input.CharacterControls.Enable();
  }

  void OnDisable()
  {
    _input.CharacterControls.Disable();
  }
}
