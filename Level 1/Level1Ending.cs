using UnityEngine;
using System.Collections;

public class Level1Ending : MonoBehaviour
{
  public Billboard billboard;
  public TextControl text;
  public BlankScreen screen;
  public LevelLoad level_loader;
  private bool triggered = false;

  void Start() {
    this.screen.displayBlackScreen();
    this.screen.fadeFromBlack(7);
  }
  
  void OnTriggerEnter() {
    if (this.triggered) return;
    
    this.triggered = true;
    StartCoroutine(ending());
  }

  IEnumerator ending() {
    string[] dialogue = {
      "Eight pieces of paper, serving as the seal of a horrible curse.",
      "You possess them, you can get rid of them and break the seal.",
      "Will this free that monster that has caused you so much trouble?",
      "Maybe it won't, maybe he has developed attachment through other means.",
      "This happens when rumors spread, and the widespread fear of a ghost feeds its manifestation.",
      "Nonetheless, getting rid of these papers is useful to silence rumors."
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

