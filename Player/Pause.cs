using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
  public Player player;
  public GameObject pause_menu;
  public GameObject settings_menu;
  public bool can_pause = true;
  
  private bool paused;

  void Awake(){this.can_pause = true;}
  
  void Update() {
    if (!this.can_pause) return;
    if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

    if (this.paused) unpauseGame();
    else pauseGame();
  }

  //Used for unpausing the game to load the main menu or a different level
  //Does not fully unpause the game, only what is necessary for the next scene to work well
  public void unpauseLoad() {
    Cursor.lockState = CursorLockMode.None;
    Time.timeScale = 1;
    AudioListener.pause = false;
    this.paused = false;
  }

  //Also called from the settings UI
  public void unpauseGame() {
    if (this.settings_menu.active) { //Leave settings menu instead
      this.settings_menu.active = false;
      this.pause_menu.active = true;
      return;
    }
    Cursor.lockState = CursorLockMode.Locked;
    Time.timeScale = 1;
    AudioListener.pause = false;
    this.pause_menu.active = false;
    this.player.caught = false;
    this.paused = false;
  }

  void pauseGame() {
    Cursor.lockState = CursorLockMode.None;
    Time.timeScale = 0;
    AudioListener.pause = true;
    this.player.caught = true;
    this.pause_menu.active = true;
    this.paused = true;
  }
}
