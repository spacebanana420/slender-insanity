using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Abstraction class for controlling a solid color on the UI
public class BlankScreen : MonoBehaviour
{
  public Image screen;

  public void displayBlackScreen() {this.screen.color = new Color(0, 0, 0, 1);}  
  public void setBlackScreen(float opacity) {this.screen.color = new Color(0, 0, 0, opacity);}
  public void displayWhiteScreen() {this.screen.color = new Color(1, 1, 1, 1);}
  public void setWhiteScreen(float opacity) {this.screen.color = new Color(1, 1, 1, opacity);}
  public void hideScreen() {this.screen.color = new Color(0, 0, 0, 0);}

  public void fadeToBlack(float time) {StartCoroutine(fadeScreen(time, 0, true));}
  public void fadeFromBlack(float time) {StartCoroutine(fadeScreen(time, 0, false));}
  public void fadeToWhite(float time) {StartCoroutine(fadeScreen(time, 1, true));}
  public void fadeFromWhite(float time) {StartCoroutine(fadeScreen(time, 1, false));}

  void Awake() {
    Color color = this.screen.color;
    color.a = 0;
    this.screen.color = color;
  }
  
  IEnumerator fadeScreen(float time, float color, bool fade_in) {
    float transparency = this.screen.color.a;
    float step = 1/time;
    if (!fade_in) {step *= -1;} //Fade-out instead
    
    while (fade_in ? transparency < 1 : transparency > 0) {
      transparency += step * Time.deltaTime;
      this.screen.color = new Color(color, color, color, transparency);
      yield return new WaitForSeconds(0.008f);
    }
  }
}
