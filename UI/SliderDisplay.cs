using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Displays the value of a UI slider
//Called by the slider itself when its value is changed by the player
public class SliderDisplay : MonoBehaviour
{
  public TextMeshProUGUI text_ui;
  public Slider slider;

  void Start() {setValue();}

  public void setValue() {this.text_ui.text = ""+this.slider.value;}
}
