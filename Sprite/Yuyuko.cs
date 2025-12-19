using UnityEngine;

//Yuyuko portrait easter egg, disappears when player gets too close
public class Yuyuko : MonoBehaviour
{
  public SpriteAPI spriteapi;
  private bool fade_out = false;
  
  void FixedUpdate() {this.fade_out = this.fade_out || this.spriteapi.getDistance() < 4;}
  
  void Update() {
    this.spriteapi.lookAtPlayer();
    if (!this.fade_out) return;

    float alpha = this.spriteapi.getAlpha() - 0.2f * Time.deltaTime;
    if (alpha <= 0) {
      this.spriteapi.setAlpha(0);
      this.gameObject.active = false;
      return;
    }
    this.spriteapi.setAlpha(alpha);
  }
}
