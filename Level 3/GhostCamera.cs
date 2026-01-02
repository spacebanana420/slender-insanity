using UnityEngine;
using UnityEngine.InputSystem;

//Logic for taking pictures in level 3
public class GhostCamera : MonoBehaviour
{
  public Player player;
  public Pause pause;
  public BlankScreen screen;
  public AudioSource camera_sound;
  public Level3Objective objective;
  
  //Can only take a picture every 5 seconds
  private float cooldown = 5;
  private float cooldown_time = 5;
  
  // Update is called once per frame
  void Update() {
    if (this.player.caught || !this.pause.can_pause) return;
    if (this.cooldown < this.cooldown_time) {
      this.cooldown += 1 * Time.deltaTime;
      return;
    }
    this.cooldown = this.cooldown_time;
    
    bool takepicture = Mouse.current.leftButton.wasPressedThisFrame;
    if (!takepicture) return;
    this.cooldown = 0;
    this.camera_sound.Play();
    this.screen.displayWhiteScreen();
    this.screen.fadeFromWhite(0.6f);
    this.objective.captureGhost();
  }
}
