using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
  public Player player;
  public Light light;
  public AudioSource sound;

  // Update is called once per frame
  void Update() {
    if (player.caught || player.paused) {return;}
    bool f_key_press = Keyboard.current.fKey.wasPressedThisFrame;
    bool r_mouse_press = Mouse.current.rightButton.wasPressedThisFrame;
    if (f_key_press || r_mouse_press) {
      light.enabled = !light.enabled;
      sound.Play();
    }
  }
}
