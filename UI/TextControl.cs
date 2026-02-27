using UnityEngine;
using TMPro;
using System.Collections;

//Class for displaying text for both one-liners and entire dialogues
//Supports character dialogues and changing faces
public class TextControl : MonoBehaviour
{
  public TextMeshProUGUI text_ui;

  public void displayText(string text) {this.text_ui.text = text;}
  public void close() {this.text_ui.text = "";}

  public void displayTemporaryText(string text, float time) {StartCoroutine(showText_seconds(text, time));}
  IEnumerator showText_seconds(string text, float time) {
    this.text_ui.text = text;
    yield return new WaitForSeconds(time);
    //If the text was changed by another function call, cancel execution
    if (this.text_ui.text == text) close(); 
  }

  
  //Text sequence, good for ending texts and dialogue
  //Each line has a unique duration based on the amount of characters and spaces
  //Also good for dialogues, allows you to change the facial expressions of a character
  public float startSequence(string[] text) {return startSequence(new CharDialogue(text));}
  public float startSequence(CharDialogue dialogue) {
    string[] text = dialogue.text;
    float total_duration = 0;
    foreach (string line in text) {total_duration+=getLineDuration(line);}

    if (dialogue.hasFaces) StartCoroutine(textSequence(dialogue));
    else StartCoroutine(textSequence(text));
    return total_duration; //Returns the total duration of the dialogue sequence in seconds
  }

  //Display text in dialogue style, no face changes
  IEnumerator textSequence(string[] text) {
    for (int i = 0; i < text.Length; i++) {
      string line = text[i];
      this.text_ui.text = line;
      yield return new WaitForSeconds(getLineDuration(line));
    }
    close();
  }

  //Display in text dialogue style, also change a character's texture (for facial expressions)
  IEnumerator textSequence(CharDialogue dialogue) {
    string[] text = dialogue.text;
    for (int i = 0; i < text.Length; i++) {
      string line = text[i];
      this.text_ui.text = line;
      dialogue.changeFace(i);
      yield return new WaitForSeconds(getLineDuration(line));
    }
    close();
  }
  
  float getLineDuration(string line) {return Mathf.Clamp(line.Length/10, 4f, 9);}
}
