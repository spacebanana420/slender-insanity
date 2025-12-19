using UnityEngine;

//Yuyuko portrait easter egg, disappears when player gets too close
public class Yuyuko : MonoBehaviour
{
  public SpriteAPI spriteapi;
  public AudioSource sound;
  private bool fade_out = false;
  
  void FixedUpdate() {
    if (this.fade_out) return;
    if (this.spriteapi.getDistance() < 4) {
      this.fade_out = true;
      this.sound.Play();
    }
  }
  
  void Update() {
    this.spriteapi.lookAtPlayer();
    if (!this.fade_out) return;

    float alpha = this.spriteapi.getAlpha() - 0.2f * Time.deltaTime;
    if (alpha <= 0) {
      this.spriteapi.setAlpha(0);
      this.enabled = false;
      this.spriteapi.toggleMesh(false); //Alternative to disabling object so sound can still play
      return;
    }
    this.spriteapi.setAlpha(alpha);
  }
}
