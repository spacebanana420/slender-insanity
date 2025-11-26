using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

//Game over class, handles static and game over logic
public class StaticKill : MonoBehaviour
{
  public StaticEffect static_script;
  public GameObject black_screen;
  public TextControl text;
  
  float intensity = 0.1f;
  bool await_user_input = false;
 
  void Update() {
    if (await_user_input) {
      bool yes = Keyboard.current.yKey.wasPressedThisFrame;
      bool no = Keyboard.current.nKey.wasPressedThisFrame;
      return;
    }
    if (this.intensity < 1) {
      this.static_script.setStatic_death(this.intensity);
      this.intensity += 0.6f * Time.deltaTime;
    }
    else {
      StartCoroutine(gameOver());
      this.enabled = false;
    }
  }

  IEnumerator gameOver() {
    this.static_script.stop();
    this.static_script.enabled = false;
    this.black_screen.active = true;
    yield return new WaitForSeconds(2f);
    this.text.displayText("X/8 pages collected\nContinue? (y/n)");
    this.await_user_input = true;
  }
}
