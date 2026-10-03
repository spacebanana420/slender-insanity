using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Handles the game over event when Slenderman gets you
public class StaticKill : MonoBehaviour
{
  public StaticEffect static_script;
  public string gameover_text; //Varies between levels, could be for example "5/8 pages collected"
  public BlankScreen black_screen;
  public TextControl text;
  public AudioSource[] music;
  public LevelLoad level_loader;
  
  public void kill() {StartCoroutine(staticKill());}

  IEnumerator staticKill() {
    float staticIntensity = 0.1f;
    while (staticIntensity < 1) {
      staticIntensity += 0.6f * Time.deltaTime;
      this.static_script.setStatic_strong(staticIntensity);
      yield return null;
    }
    StartCoroutine(gameOver());
  }

  //The game over screen with black background
  IEnumerator gameOver() {
    this.static_script.stop();
    this.static_script.enabled = false;
    this.black_screen.displayBlackScreen();
    foreach (AudioSource track in music) {track.Stop();}
    yield return new WaitForSeconds(2f);
    this.text.displayText(this.gameover_text+"\nTry again? (y/n)");
    while (true) {
      bool yes = Keyboard.current.yKey.wasPressedThisFrame;
      bool no = Keyboard.current.nKey.wasPressedThisFrame;
      if (yes) level_loader.reloadThisScene();
      else if (no) level_loader.loadMainMenu();
      yield return null;
    }
  }
}
