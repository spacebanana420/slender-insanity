using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Game over class, handles static and game over logic
public class StaticKill : MonoBehaviour
{
  public StaticEffect static_script;
  public string gameover_text; //e.g 5/8 pages collected
  public BlankScreen black_screen;
  public TextControl text;
  public AudioSource[] music;
  public LevelLoad level_loader;
  
  float intensity = 0.1f;
  bool await_user_input = false;
  bool stop_check = false;
 
  void Update() {
    if (await_user_input) {
      bool yes = Keyboard.current.yKey.wasPressedThisFrame;
      bool no = Keyboard.current.nKey.wasPressedThisFrame;
      if (yes) level_loader.loadScene(SceneManager.GetActiveScene().buildIndex);
      else if (no) level_loader.loadScene("Main Menu");
    }
    if (stop_check) {return;}
    if (this.intensity < 1) {
      this.static_script.setStatic_strong(this.intensity);
      this.intensity += 0.6f * Time.deltaTime;
    }
    else {
      StartCoroutine(gameOver());
      this.stop_check = true;
    }
  }

  IEnumerator gameOver() {
    this.static_script.stop();
    this.static_script.enabled = false;
    this.black_screen.displayBlackScreen();
    foreach (AudioSource track in music) {track.Stop();}
    yield return new WaitForSeconds(2f);
    this.text.displayText(this.gameover_text+"\nTry again? (y/n)");
    this.await_user_input = true;
  }
}
