using UnityEngine;
using TMPro;
using System.Collections;

//Abstraciton class so external scripts can manipulate GUI text without duplicate code
public class TextControl : MonoBehaviour
{
  public TextMeshProUGUI text_ui;

  public void displayText(string text) {this.text_ui.text = text;}
  public void close() {this.text_ui.text = "";}

  public void displayTemporaryText(string text, float time) {StartCoroutine(showText_seconds(text, time));}

  //Text sequence, good for ending texts and dialogues
  public float startSequence(string[] text, float duration = 6) {
    StartCoroutine(textSequence(text, duration));
    return text.Length * duration; //Returns the total duration of the sequence
  }
  IEnumerator textSequence(string[] text, float duration) {
    foreach (string line in text) {
      this.text_ui.text = line;
      yield return new WaitForSeconds(duration);
    }
    close();
  }
  
  IEnumerator showText_seconds(string text, float time) {
    this.text_ui.text = text;
    yield return new WaitForSeconds(time);
    //If the text was changed by another function call, do not interfere with its perpetuity or duration
    if (this.text_ui.text == text) {close();} 
  }
}
