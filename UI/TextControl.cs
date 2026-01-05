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
  IEnumerator showText_seconds(string text, float time) {
    this.text_ui.text = text;
    yield return new WaitForSeconds(time);
    //If the text was changed by another function call, do not interfere with its perpetuity or duration
    if (this.text_ui.text == text) {close();} 
  }

  
  //Text sequence, good for ending texts and dialogue
  //Each line has a unique duration based on the amount of characters and spaces
  public float startSequence(string[] text) {
    float total_duration = 0;
    foreach (string line in text) {total_duration+=getLineDuration(line);}
    
    StartCoroutine(textSequence(text));
    return total_duration; //Returns the total duration of the sequence in seconds
  }
  IEnumerator textSequence(string[] text) {
    foreach (string line in text) {
      this.text_ui.text = line;
      yield return new WaitForSeconds(getLineDuration(line));
    }
    close();
  }
  
  float getLineDuration(string line) {return Mathf.Clamp(line.Length/10, 3.5f, 8);}
}
