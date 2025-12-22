using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Controls difficulty according to the number of souls released
public class SoulsControl : MonoBehaviour
{
  public GameObject slenderman;
  public GameObject ghost;
  public Slenderman slender_script;
  public SCPGhost ghost_script;
  
  public TextControl text;
  public StaticEffect static_script;
  public StaticKill gameover;
  public Level2Victory victory;

  //Used for random page placement in map
  public Transform[] orbs;
  public List<Transform> orb_placements;
  
  public AudioSource[] music;
  public byte souls_released = 0;
  
  private SlenderStats slender_stats = new SlenderStats();
  private GhostStats ghost_stats = new GhostStats();
  private float thump_volume; //Preserves original thump volume so it can be played manually from other classes


  void Awake() {
    this.thump_volume = this.music[0].volume;
    //Random page placement
    foreach (Transform orb in this.orbs) {
      int i = Random.Range(0, this.orb_placements.Count);
      orb.position = this.orb_placements[i].position;
      orb.gameObject.active = true;
      this.orb_placements.RemoveAt(i);
    }
  }

  //External classes can play this sound, it's a nice sound to use in some parts of the game
  public void playThump() {
    this.music[0].volume = this.thump_volume;
    this.music[0].Play();
  }

  //Each page calls this function when it's collected
  //Handles music, Slender's difficulty as well as the level 1 victory event
  public void releaseSoul() {
    int i = this.souls_released;
    this.souls_released += 1;
    string text = this.souls_released+"/10 souls released";
    this.text.displayTemporaryText(text, 4);
    this.gameover.gameover_text = text;
    
    if (this.souls_released == 10) {
      StartCoroutine(stopMusic());
      this.slenderman.active = false;
      this.ghost.active = false;
      this.static_script.stopFade(4);
      this.victory.startVictoryEvent();
      return;
    }
    //Change Slenderman's stats
    this.slender_script.setChaseSpeed(this.slender_stats.speeds[i]);
    this.slender_script.setTeleportDistance(this.slender_stats.teleport_distances[i]);
    this.slender_script.setTeleportation(this.slender_stats.teleport_limits[i], this.slender_stats.can_tp_forward[i], this.slender_stats.forward_tp_limits[i]);
    this.slender_script.setInvisibility(this.slender_stats.invisible_limits[i], this.slender_stats.can_be_invisible[i]);

    //Change ghost's stats
    this.ghost_script.setTeleport(this.ghost_stats.teleport_distances[i], this.ghost_stats.teleport_cooldown[i]);
    this.ghost_script.setSpeed(this.ghost_stats.speeds[i]);
    this.ghost_script.setInvisibilityCooldown(this.ghost_stats.invisibility_cooldown[i]);
    
    switch (this.souls_released) {
      case 1:
        this.slenderman.active = true;
        this.ghost.active = true;
        StartCoroutine(firstMusic(this.music[0]));
        return;
      case 3:
        StartCoroutine(playGradual(this.music[1]));
        break;
      case 6:
        StartCoroutine(playGradual(this.music[2]));
        break;
      case 9:
        this.music[3].Play();
        break;
    }
  }

  //For the thump sound, more granular control over how frequently it's heard
  IEnumerator firstMusic(AudioSource music) {
    music.Play();
    while (this.souls_released < 10) {
      yield return new WaitForSeconds(4);
      music.time = 0; //Alternative to constant Play() calls, allows external scripts to stop the music
    }
  }

  //Play with a fade-in
  IEnumerator playGradual(AudioSource music) {
    float max_volume = music.volume;
    float volume_step = max_volume * 0.4f;
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
    bool decreasing = true;
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


//Enemy stats for Slender and the ghost are written in separate classes here to not clutter the main class
//1 to 9 orbs released out of 10
class SlenderStats {
  public float[] speeds = {1, 1.5f, 2, 3, 3.5f, 4, 4.4f, 4.6f, 4.8f};

  public float[] teleport_distances = {14, 12, 10, 9, 8, 7, 6, 5.5f, 5};
  public float[] teleport_limits = {36, 34, 32, 30, 28, 24, 20, 15, 10};
  public float[] forward_tp_limits = {90, 90, 90, 90, 60, 45, 26, 22, 18};
  public bool[] can_tp_forward = {false, false, false, true, true, true, true, true, true};

  public float[] invisible_limits = {40, 80, 100, 110, 120, 120, 120, 120, 120};
  public bool[] can_be_invisible = {true, true, true, true, true, true, false, false, false};
}

class GhostStats {
  public float[] teleport_cooldown = {35, 30, 25, 20, 15, 12, 10, 8, 8};
  public float[] teleport_distances = {20, 19, 18, 17, 16, 15, 14, 14, 14};
  public float[] speeds = {7, 7.5f, 8, 8.5f, 9, 9.5f, 10, 10.5f, 11};
  public float[] invisibility_cooldown = {40, 38, 36, 34, 32, 30, 28, 26, 26};
}
