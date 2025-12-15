using UnityEngine;

//Source code for the level 2 enemy behavior, a ghost inspired by SCP-087-1
public class SCPGhost : MonoBehaviour
{
  public EnemyAPI enemy_api;
  public Material material;

  private float visible_percentage = 1; //For adjusting material transparency
  private float teleport_distance;
  private float speed = 1;

  //Ghost turns invisible when it fades out, entering a cooldown where it's not hostile
  private float invisible_cooldown = 20;
  private float invisible_meter = 0;
  private bool is_invisible = false;
  
  void Awake() {this.material.color = new Color32(109, 109, 109, 255);}

  void Update() {
    this.enemy_api.lookAtPlayer();
    float distance = this.enemy_api.getDistance();
    bool is_visible = this.enemy_api.isLookedAt();
    bool is_seen = this.enemy_api.isSeen();
    
    if (distance < 1.8f) {
      this.enemy_api.killPlayer();
      resetTransparency();
      this.enabled = false;
      return;
    }
    if (is_seen) {
      if (distance < 4) fade(true);
      else fade(false);
      return;
    }
    fade(false);
    this.enemy_api.move(this.speed);     
  }

  void waitInvisibleTime() {
  }

  //Ghost fades in or out, if fully transparent then it returns true
  bool fade(bool fade_out) {
    if (fade_out) {
      if (this.visible_percentage < 0) this.visible_percentage = 0;
      else this.visible_percentage -= 0.2f * Time.deltaTime;
    }
    else {
      if (this.visible_percentage > 1) this.visible_percentage = 1;
      else this.visible_percentage += 0.4f * Time.deltaTime;
    }
    Color c = this.material.color;
    c.a = this.visible_percentage;
    this.material.color = c;
    return c.a == 0;
  }

  void resetTransparency() {
    Color c = this.material.color;
    c.a = 1;
    this.material.color = c;
  }
  
}
