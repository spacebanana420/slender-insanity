using UnityEngine;
using System.Collections;

//The ending dialogue seen in level 1
public class Level1Ending : MonoBehaviour
{
  public Billboard billboard;
  public TextControl text;
  public BlankScreen screen;
  public LevelLoad level_loader;

  private bool triggered = false;

  void Start() {
    this.screen.displayBlackScreen();
    this.screen.fadeFromBlack(4);
  }
  
  void OnTriggerEnter() {
    if (this.triggered) return;
    
    this.triggered = true;
    StartCoroutine(ending());
  }

  IEnumerator ending() {
    string[] dialogue = {
      "When rumors spread, the widespread fear of a ghost can feed its manifestation.",
      "Its physical presence becomes stronger and more destructive.",
      "Eight pieces of paper, serving as the seal of a horrible curse.",
      "You possess them, you can get rid of them and break the seal.",
      "The seal binds him, without it his reason to stay fades away.",
      "Now, why these papers are so meaningful, we don't know...",
      "Have you noticed what happened after you obtained them all?",
      "His violent manifestation weakens, becomes similar to the others...",
      "Is this enough to free him, and free you from him? We will see.",
    };
    float duration = this.text.startSequence(dialogue);
    yield return new WaitForSeconds(duration+2);
    this.billboard.fadeOut(6, false);
    yield return new WaitForSeconds(8);
    this.screen.fadeToBlack(8);
    yield return new WaitForSeconds(8);
    this.level_loader.loadMainMenu();
  }
}

