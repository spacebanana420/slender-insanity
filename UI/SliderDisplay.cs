using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Displays the value of a UI slider
//Called by the slider itself when its value is changed by the player
public class SliderDisplay : MonoBehaviour
{
  public TextMeshProUGUI text_ui;
  public Slider slider;
  private bool disabled;

  void Start() {setValue();}

  public void setValue() {
    if (this.disabled) return;
    int value_rounded = (int)(this.slider.value * 1000); //Rounding with 3 decimal cases
    float value = (float)value_rounded / 1000;
    this.text_ui.text = ""+value;
  }

  public void disable() {
    this.text_ui.text = "Disabled";
    this.disabled = true;
  }

  //Skip rounding for integer values
  public void setValue_int() {
    if (this.disabled) return;
    this.text_ui.text = ""+this.slider.value;
  }
}
