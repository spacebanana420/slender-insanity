using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
  public Player player;
  public Light light;
  public AudioSource sound;

  // Update is called once per frame
  void Update() {
    if (this.player.caught || this.player.paused) {return;}
    bool f_key_press = Keyboard.current.fKey.wasPressedThisFrame;
    bool r_mouse_press = Mouse.current.rightButton.wasPressedThisFrame;
    if (f_key_press || r_mouse_press) {
      this.light.enabled = !this.light.enabled;
      this.sound.Play();
    }
  }

  //Called externally, used in level 1 start
  public void turnOn() {
    if (this.light.enabled) {return;}
    this.light.enabled = true;
    this.sound.Play();
  }
}
