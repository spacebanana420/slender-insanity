using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Handles the start and objective for level 3
public class Level3Objective : MonoBehaviour
{
  public Player player;
  public Pause pause;
  public Level3Door door;
  public Flashlight flashlight;
  public GhostCamera camera;
  public Slenderman slender_script;

  public BlankScreen screen;
  public StaticEffect static_script;
  public TextControl text;
  public StaticKill gameover;
  public LevelLoad level_loader;

  //Used for random page placement in map
  public List<SpriteAPI> ghosts;
  public AudioSource[] music;

  private int ghosts_captured = 0;
  private GameObject slenderman;

  //Slenderman's stats, difficulty adjustment
  //One value for each ghost captured (1 to 6 ghosts)
  float[] speeds = {1.5f, 2.4f, 3.2f, 4, 4.4f, 4.6f};
  float[] teleport_distances = {10, 9, 8, 7, 6.5f, 6};
  float[] teleport_limits = {24, 20, 18, 15, 12, 10};
  float[] invisible_limits = {40, 50, 60, 70, 80, 80};
  bool[] can_be_invisible = {true, true, true, true, true, false};

  void Start() {
    this.slenderman = this.slender_script.gameObject;
    StartCoroutine(levelStart());
  }
  

  IEnumerator levelStart() {
    //Slender's stats before the first ghost is photographed
    this.slender_script.setChaseSpeed(0.8f);
    this.slender_script.setTeleportDistance(13);
    this.slender_script.setTeleportation(30, false, 100);
    this.slender_script.setInvisibility(40, true);
    
    yield return new WaitForSeconds(1);
    this.flashlight.turnOn();
    yield return new WaitForSeconds(2);
    this.text.displayTemporaryText("Photograph 6 ghosts\nUse the left mouse button to take a picture", 10);
    yield return new WaitForSeconds(20);
    this.slenderman.active = true; //Enable Slenderman from the start unlike in previous levels
  }

  //Each page calls this function when it's collected
  //Handles music, Slender's difficulty as well as the level 1 victory event
  public void captureGhost() {
    //Only counts if player successfully takes a picture of a ghost
    if (!getValidGhost()) return;
    
    int i = this.ghosts_captured;
    this.ghosts_captured += 1;

    //Increase difficulty
    this.slender_script.setChaseSpeed(this.speeds[i]);
    this.slender_script.setTeleportDistance(this.teleport_distances[i]);
    this.slender_script.setTeleportation(this.teleport_limits[i], false, 100);
    this.slender_script.setInvisibility(this.invisible_limits[i], this.can_be_invisible[i]);


    //Show objective progress
    string text = this.ghosts_captured+"/6 ghosts photographed";
    this.gameover.gameover_text = text;
    if (this.ghosts.Count == 0) {
      this.door.enabled = true;
      this.text.displayTemporaryText("All evidence is gathered, go back to the exit!", 6);
    }
    else this.text.displayTemporaryText(text, 5);
    
    switch (this.ghosts_captured) {
      case 1:
        this.slenderman.active = true;
        this.music[0].Play();
        return;
      case 3:
        this.music[1].Play();
        break;
      case 5:
        this.music[2].Play();
        break;
      case 6:
        this.music[3].Play();
        break;
    }
  }

  //Check if the player took a picture of a ghost within a short distance and in the player's field of view
  bool getValidGhost() {
    for (int i = 0; i < this.ghosts.Count; i++) {
      SpriteAPI ghost = this.ghosts[i];
      if (!ghost.isLookedAt()) continue;
      if (ghost.getDistance() > 4.5f) continue;
      
      ghost.gameObject.active = false;
      this.ghosts.RemoveAt(i);
      return true;
    }
    return false;
  }

  //Level 3 beaten, triggered by exiting the building after having all ghosts pictured
  //Unfinished
  public void triggerVictory() {
    StartCoroutine(stopMusic());
    this.slenderman.active = false;
    this.camera.enabled = false;
    this.player.caught = true;
    this.pause.can_pause = false;
    this.screen.fadeToBlack(1);
    this.static_script.stopFade(3);
    StartCoroutine(loadEndingScene());
  }

  IEnumerator loadEndingScene() {
    yield return new WaitForSeconds(5);
    string[] ending_text = {
      "You survive this encounter and leave with your evidence",
      "You couldn't even believe it, real footage of ghost sightings!",
      "Ghost hunting and sightings have become very profitable, a new market surges for these things",
      "But is this profit worth the trouble it causes?",
    };
    float duration = this.text.startSequence(ending_text);
    yield return new WaitForSeconds(duration+1);
    this.level_loader.loadLevel3_end(); //The rest of the ending continues in a dedicated scene
  }

  //Gradually decreases the volume of all music tracks based on a percentage of their original volume
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
