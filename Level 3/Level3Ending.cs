using UnityEngine;
using System.Collections;

public class Level3Ending : MonoBehaviour
{
  public Billboard billboard;
  public TextControl text;
  public BlankScreen screen;
  public LevelLoad level_loader;
  //public AudioSource[] audio;
  private bool triggered = false;
  
  void OnTriggerEnter() {
    if (this.triggered) return;
    
    this.triggered = true;
    StartCoroutine(ending());
  }

  IEnumerator ending() {
    string[] dialogue = {
      "Your interest in the paranormal has not gone unnoticed",
      "Having reached so far, having discovered so much, but with more questions than ever",
      "Having your life threatened by one of many cursed souls, one with hostile intent",
      "Consider a life that has reached its end",
      "A life very well lived, but not free of attachment",
      "An attachment so strong it chains you to the world you were supposed to leave behind",
      "Grudges, resentment, hatred, love, suffering, pleasure",
      "You freeze in space and time",
      "A ghost wandering aimlessly, intercepting the lives of others",
      "The stronger the attachment, the more vivid your physical presence is",
      "I was once a living person too, just like you",
      "I passed away just like my peers, but unlike them I stayed here",
      "They moved on while I could not",
      "I cling to something I will never have again",
      "I cannot help it but to contemplate the beauty of this place that meant so much to me",
      "Such a desire to stay, to keep what I cherish the most forever, an impossible immutable state",
      "Now I suffer the consequences of my choices",
      "Those who cling to wrath, hatred, violence....",
      "They cause a great disturbance",
      "They can, however, be released of their curse too",
      "They feed off of the fear and imagination of others",
      "If they become forgotten, if the world leaves them behind, then they will perish",
      "You must erase him from everyone's mind and he will decay with time"
    };
    float duration = this.text.startSequence(dialogue);
    yield return new WaitForSeconds(duration+2);
    this.billboard.fadeOut(6);
    yield return new WaitForSeconds(8);
    this.screen.fadeToBlack(8);
    this.level_loader.loadMainMenu();
  }
}
