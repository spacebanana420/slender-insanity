using UnityEngine;
using UnityEngine.InputSystem;

//Logic for taking pictures in level 3
public class GhostCamera : MonoBehaviour
{
  public BlankScreen screen;
  public AudioSource camera_sound;
  public Level3Objective objective;
  
  //Can only take a picture every 5 seconds
  private float cooldown = 0;
  private float cooldown_time = 5;
  
  // Update is called once per frame
  void Update() {
    if (this.cooldown > this.cooldown_time) this.cooldown = this.cooldown_time;
    else this.cooldown += 1 * Time.deltaTime;
    
    bool ready = this.cooldown == this.cooldown_time;
    bool takepicture = Mouse.current.leftButton.wasPressedThisFrame;
    if (!ready || !takepicture) return;
    this.objective.captureGhost();
    this.camera_sound.Play();
    this.screen.displayWhiteScreen();
    this.screen.fadeFromWhite(2);
  }
}
