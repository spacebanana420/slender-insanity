using UnityEngine;
using System.Collections;

public class Level3Ending : MonoBehaviour
{
  public Billboard billboard;
  public TextControl text;
  public BlankScreen screen;
  public LevelLoad level_loader;
  public GameObject easterEgg;
  //public AudioSource[] audio;
  private bool triggered = false;

  void Start() {
    this.screen.displayBlackScreen();
    this.screen.fadeFromBlack(4);
  }
  
  void OnTriggerEnter() {
    if (this.triggered) return;
    
    this.triggered = true;
    this.easterEgg.active = false;
    StartCoroutine(ending());
  }

  IEnumerator ending() {
    string[] dialogue = {
      "Your interest in the paranormal has not gone unnoticed.",
      "Having reached so far, having discovered so much, but with more questions than ever.",
      "Consider a life that has reached its end.",
      "A life very well lived, but not free of attachment.",
      "An attachment so strong it chains you to the world you were supposed to leave behind.",
      "You freeze in space and time.",
      "A ghost wandering aimlessly, intercepting the lives of others.",
      "The stronger the attachment, the more vivid your physical presence is.",
      "Those who cling to violent emotions cause a deeper disturbance.",
      "They can, however, be released of their curse too.",
      "They feed off of the fear and imagination of others.",
      "If they become forgotten, if the world leaves them behind, then they will perish.",
      "Alternatively, sometimes all that is needed is to get rid of what keeps their desire for revenge.",
      "You and I have seen it is possible to heal even the most troublesome souls."
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
