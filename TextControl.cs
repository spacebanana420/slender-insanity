using UnityEngine;
using TMPro;

//Abstraciton class so external scripts can manipulate GUI text without duplicate code
public class TextControl : MonoBehaviour
{
  public TextMeshProUGUI text_ui;

  public void displayText(string text) {
    this.text_ui.text = text;
    this.text_ui.enabled = true;
  }

  public void close() {
    this.text_ui.enabled = false;
    this.text_ui.text = "";
  }
}
