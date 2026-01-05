using UnityEngine;

//Simple billboard code to make an object stare at the player camera
//The billboard can also fade out
public class Billboard : MonoBehaviour
{
  public SpriteAPI spriteapi;
  public AudioSource sound;
  
  private float fade_speed;
  private bool fade_out = false;
  private bool disable_on_fade = false;
  private bool sound_exists;

  void Awake() {this.sound_exists = this.sound != null;}
  
  public void fadeOut(float time = 4, bool disableObject = true) {
    this.fade_speed = 1/time;
    this.fade_out = true;
    this.disable_on_fade = disableObject;
  }
  
  void Update() {
    this.spriteapi.lookAtPlayer();
    if (!this.fade_out) return;
    float step = this.fade_speed * Time.deltaTime;
    float alpha = this.spriteapi.getAlpha() - step;
    
    if (alpha <= 0) {
      if (this.disable_on_fade) this.gameObject.active = false;
      else this.enabled = false;
      return;
    }
    this.spriteapi.setAlpha(alpha);
    if (this.sound_exists && this.sound.volume > 0) this.sound.volume -= step;    
  }
}
