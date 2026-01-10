using UnityEngine;

//Easter egg in level 3 ending
public class KogasaJumpscare : MonoBehaviour
{
  public SpriteAPI sprite_drawing;
  public SpriteAPI sprite_kogasa;
  
  public BlankScreen screen;
  public AudioSource jumpscare_sound;

  GameObject kogasa;
  bool waitForPlayer = false;
  bool jumpscare = false;
  float look_time = 0;
  
  void Awake() {this.kogasa = this.sprite_kogasa.gameObject;}

  void Update() {
    if (this.jumpscare) {jumpscarePlayer();}
    else if (this.waitForPlayer) {
      //Kogasa waits for player to look to jumpscare
      if (!this.sprite_kogasa.isLookedAt()) return;
      this.jumpscare_sound.Play();
      this.jumpscare = true;
      this.waitForPlayer = false;
    }
    else checkTrigger();
  }

  //Player needs to stare at the drawing from up close for 1 second
  void checkTrigger() {
    if (this.sprite_drawing.getDistance() > 4.5f) return;
    if (this.sprite_drawing.isLookedAt()) this.look_time += 1 * Time.deltaTime;
    if (this.look_time >= 1) {
      this.kogasa.active = true;
      this.waitForPlayer = true;
    }
  }

  //Kogasa runs to player
  void jumpscarePlayer() {
    if (this.sprite_kogasa.getDistance() <= 1.7f) {
      this.screen.displayWhiteScreen();
      this.screen.fadeFromWhite(6);
      this.jumpscare_sound.Stop();
      this.kogasa.active = false;
      this.enabled = false;
      return;
    }
    this.sprite_kogasa.lookAtPlayer();
    this.sprite_kogasa.move(14);
  }
}
