using UnityEngine;

//Source code for the level 2 enemy behavior, a ghost inspired by SCP-087-1
public class SCPGhost : MonoBehaviour
{
  public EnemyAPI enemy_api;
  public SpriteAPI sprite_api;
  public AudioSource sound_loop; //Audio cue to aid the player
  private float sound_loop_volume; //Original volume

  private float visible_percentage = 1; //For adjusting material transparency
  private float speed = 1;

  private float teleport_distance = 5;
  private float teleport_cooldown = 20;
  private float teleport_meter = 0;

  //Ghost turns invisible when it fades out, entering a cooldown where it's not hostile
  private float invisible_cooldown = 20;
  private float invisible_meter = 0;

  public void setTeleport(float distance, float cooldown) {
    this.teleport_distance = distance;
    this.teleport_cooldown = cooldown;
  }
  public void setSpeed(float speed) {this.speed = speed;}
  public void setInvisibilityCooldown(float cooldown) {this.invisible_cooldown = cooldown;}
  
  void Awake() {this.sound_loop_volume = this.sound_loop.volume;}

  void Update() {
    if (isInvisible()) {
      waitInvisibleTime();
      return;
    }
    this.enemy_api.lookAtPlayer();
    float distance = this.enemy_api.getDistance();
    bool is_visible = this.enemy_api.isLookedAt();
    bool is_seen = this.enemy_api.isSeen();
    
    if (distance < 1.8f) {
      this.enemy_api.killPlayer();
      this.sprite_api.setAlpha(1);
      this.enabled = false;
      return;
    }
    bool faded_away = fade(is_seen, distance);
    if (faded_away) return;

    checkTeleport(distance, is_visible);
    if (is_visible) return;
    this.enemy_api.move(this.speed);     
  }

  void checkTeleport(float distance, bool is_visible) {
    float count_speed = is_visible ? 2 : 1;
    if (distance < 8) {return;}
    if (this.teleport_meter > this.teleport_cooldown) {
      this.teleport_meter = 0;
      this.enemy_api.teleport(distance, this.teleport_distance);
      return;
    }
    this.teleport_meter += count_speed * Time.deltaTime;
  }

  void turnInvisible(bool invisible) {
    this.enemy_api.toggleMesh(!invisible);
    this.enemy_api.toggleController(!invisible); //To disable collisions
  }

  void waitInvisibleTime() {
    if (this.invisible_meter > this.invisible_cooldown) {
      turnInvisible(false);
      this.invisible_meter = 0;
    }
    this.invisible_meter += 1 * Time.deltaTime;
  }

  bool isInvisible() {return !this.enemy_api.isMeshEnabled();}

  //Ghost fades in or out, if fully transparent then it returns true
  bool fade(bool is_seen, float distance) {
    if (is_seen && distance < 4) {
      if (this.visible_percentage < 0) this.visible_percentage = 0;
      else this.visible_percentage -= 0.2f * Time.deltaTime;
    }
    else {
      if (this.visible_percentage > 1) this.visible_percentage = 1;
      else this.visible_percentage += 0.6f * Time.deltaTime;
    }
    this.sprite_api.setAlpha(this.visible_percentage);
    this.sound_loop.volume = this.sound_loop_volume * this.visible_percentage;
    if (this.visible_percentage == 0) {
      turnInvisible(true);
      return true;
    }
    return false;
  }
}
