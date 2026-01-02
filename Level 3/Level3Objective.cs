using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Handles the start and objective for level 3
public class Level3Objective : MonoBehaviour
{
  public Flashlight flashlight;
  public Slenderman slender_script;
  public TextControl text;
  public StaticKill gameover;
  //public Level3Victory l3victory;

  //Used for random page placement in map
  public List<SpriteAPI> ghosts;
  public AudioSource[] music;

  private int ghosts_captured = 0;
  private GameObject slenderman;

  //Slenderman's stats, difficulty adjustment
  //One value for each ghost captured (1 to 5 ghosts)
  float[] speeds = {1, 2, 2.5f, 3, 4};
  float[] teleport_distances = {7.5f, 7, 6.5f, 6, 5};
  float[] teleport_limits = {20, 18, 16, 14, 10};
  float[] invisible_limits = {40, 60, 80, 100, 100};
  bool[] can_be_invisible = {true, true, true, true, false};

  void Awake() {this.slenderman = this.slender_script.gameObject;}
  void Start() {StartCoroutine(levelStart());}
  

  IEnumerator levelStart() {
    yield return new WaitForSeconds(1);
    this.flashlight.turnOn();
    yield return new WaitForSeconds(2);
    text.displayTemporaryText("Take photographs of 5 ghosts\nUse left mouse button to take a picture", 12);
    yield return new WaitForSeconds(15);
    
    //Enable Slenderman from the start, unlike in previous levels
    this.slender_script.setChaseSpeed(0.4f);
    this.slender_script.setTeleportDistance(9);
    this.slender_script.setTeleportation(30, false, 100);
    this.slender_script.setInvisibility(40, true);
    this.slenderman.active = true;
  }

  //Each page calls this function when it's collected
  //Handles music, Slender's difficulty as well as the level 1 victory event
  public void captureGhost() {
    //Only counts if player successfully takes a picture of a ghost
    int ghost_i = getValidGhost();
    if (ghost_i == -1) return;
    this.ghosts.RemoveAt(ghost_i);
    
    int i = this.ghosts_captured;
    this.ghosts_captured += 1;
    
    string text = this.ghosts_captured+"/5 ghosts captured";
    this.text.displayTemporaryText(text, 4);
    this.gameover.gameover_text = text;
    
    // if (remaining_ghosts == 0) {
    //   StartCoroutine(stopMusic());
    //   //this.l3victory.startVictoryEvent();
    //   return;
    // }
    this.slender_script.setChaseSpeed(this.speeds[i]);
    this.slender_script.setTeleportDistance(this.teleport_distances[i]);
    this.slender_script.setTeleportation(this.teleport_limits[i], false, 100);
    this.slender_script.setInvisibility(this.invisible_limits[i], this.can_be_invisible[i]);
    
    switch (this.ghosts_captured) {
      case 1:
        StartCoroutine(playGradual(this.music[0]));
        return;
      case 3:
        StartCoroutine(playGradual(this.music[1]));
        break;
      case 5:
        StartCoroutine(playGradual(this.music[2]));
        break;
    }
  }

  //Check if the player took a picture of a ghost within a short distance and in the player's field of view
  int getValidGhost() {
    for (int i = 0; i < this.ghosts.Count; i++) {
      SpriteAPI ghost = this.ghosts[i];
      if (!ghost.isLookedAt()) continue;
      if (ghost.getDistance() > 4) continue;
      Billboard ghost_b = ghost.gameObject.GetComponent<Billboard>();
      ghost_b.fadeOut();
      return i;
    }
    return -1;
  }
  
  //Play with a fade-in
  IEnumerator playGradual(AudioSource music) {
    float max_volume = music.volume;
    float volume_step = max_volume * 0.3f;
    music.volume = 0;
    music.Play();
    while (music.volume < max_volume) {
      music.volume += volume_step * Time.deltaTime;
      yield return new WaitForSeconds(0.005f);
    }
    music.volume = max_volume; //Clamp
  }

  //Gradually decreases the volume of the pages music based on a percentage of their original volume
  IEnumerator stopMusic() {
    float[] volume_steps = new float[this.music.Length];
    for(int i = 0; i < this.music.Length; i++) {
      volume_steps[i] = this.music[i].volume * 0.2f; //Get the volume step to reduce every second
    }
    while (true) {
      for (int i = 0; i < this.music.Length; i++) {
        this.music[i].volume -= volume_steps[i] * Time.deltaTime; 
      }
      bool all_done = true;
      foreach (AudioSource m in this.music) {
        if (m.volume > 0) {all_done = false; break;}
      }
      if (all_done) {break;}
      yield return new WaitForSeconds(0.005f);
    }
  }
}
