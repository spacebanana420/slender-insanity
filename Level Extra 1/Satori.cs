using UnityEngine;
using System.Collections;

//Handles the start of extra level 1, with the Satori dialogue
public class Satori : MonoBehaviour
{
  public SpriteAPI api;
  public TextControl text;

  void Start() {
    this.api.enableBillboard();
    StartCoroutine(enableDialogue());
  }

  IEnumerator enableDialogue() {
    while (this.api.getDistance() > 5 || !this.api.isLookedAt()) {yield return null;}

    string[] dialogue = {
      "Well, hello.",
      "Looks like you're another human curious enough to go on a hike here.",
      "My name is Satori Komeiji.",
      "You seem to be a curious person, I'm sure you will appreciate what I have to tell.",
      "I see that you brought a digital camera, and so I must ask you a favour.",
      "Beyond this valley lies a haunted place, full of ghosts and youkai and all kinds of people.",
      "Don't worry about them, they are mostly harmless.",
      "I need you to photograph them and bring the evidence back to me.",
      "I would do it myself but they flee the moment they see me, maybe you will have better luck.",
      "Can you do that for me, please?",
      "Oh, there's something else you need to know.",
      "My sister is roaming that place. Don't mind her, I think it will be fine.",
      "I wish you the best of luck."
    };
    float duration = this.text.startSequence(dialogue);
    yield return new WaitForSeconds(duration);
  }
}
