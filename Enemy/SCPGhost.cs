using UnityEngine;
using System.Collections;

//Source code for the level 2 enemy behavior, a ghost inspired by SCP-087-1
public class SCPGhost : MonoBehaviour
{
  public EnemyAPI enemy_api;
  public SpriteAPI sprite_api;
  public Light player_light;
  public AudioSource sound_loop; //Audio cue to aid the player
  public AudioSource jumpscare;
  public BlankScreen screen;
  
  private float sound_loop_volume; //Original volume

  private float visible_percentage = 1; //For adjusting material transparency
  private float speed = 1;

  private float teleport_distance = 5;
  private float teleport_cooldown = 20;
  private float teleport_meter = 0;

  //Ghost turns invisible when it fades out, entering a cooldown where it's not hostile
  private float invisible_cooldown = 20;
  private float invisible_meter = 0;

  private bool stunning_player = false;

  public void setTeleport(float distance, float cooldown) {
    this.teleport_distance = distance;
    this.teleport_cooldown = cooldown;
  }
  public void setSpeed(float speed) {this.speed = speed;}
  public void setInvisibilityCooldown(float cooldown) {this.invisible_cooldown = cooldown;}

  //Preserve original max volume
  void Awake() {this.sound_loop_volume = this.sound_loop.volume;}

  void Update() {
    if (this.stunning_player) return;
    if (isInvisible()) {
      waitInvisibleTime();
      return;
    }
    this.enemy_api.lookAtPlayer();
    float distance = this.enemy_api.getDistance();
    bool is_visible = this.enemy_api.isLookedAt();
    bool is_seen = this.enemy_api.isSeen();
    
    if (distance < 1.8f) {
      this.enemy_api.killPlayer(0.2f); //Does not actually kill, just positions the player and camera
      this.stunning_player = true;
      this.sprite_api.setAlpha(1);
      StartCoroutine(stunPlayer());
      return;
    }
    bool faded_away = fade(is_seen, distance); //Fade-in or fade-out
    if (faded_away) return;

    checkTeleport(distance, is_visible);
    if (is_visible) this.enemy_api.applyInstantGravity(); //Only apply gravity
    else this.enemy_api.move(this.speed); //Move toward player
  }

  void checkTeleport(float distance, bool is_visible) {
    float count_speed = is_visible ? 2 : 1;
    if (distance < this.teleport_distance) {return;}
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
    int fade_distance = this.player_light.enabled ? 7 : 3;
    float fade_speed = this.player_light.enabled ? 0.5f : 0.2f;

    if (is_seen && distance < fade_distance) {
      if (this.visible_percentage < 0) this.visible_percentage = 0;
      else this.visible_percentage -= fade_speed * Time.deltaTime;
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

  //Freezes player, disappears and hinders player visibility
  IEnumerator stunPlayer() {
    this.jumpscare.Play();
    yield return new WaitForSeconds(0.35f);
    this.screen.fadeToBlack(0.1f);
    yield return new WaitForSeconds(0.16f);
    this.invisible_meter = 0;
    this.visible_percentage = 0;
    this.sound_loop.volume = 0;
    this.turnInvisible(true);
    this.enemy_api.releasePlayer();
    this.stunning_player = false;
    this.screen.fadeFromBlack(20);
  }
}
