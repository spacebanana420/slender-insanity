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
  //public Level1Victory l1victory;

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
    this.text.displayTemporaryText(this.souls_released+"/10 souls released", 4);
    
    if (this.souls_released == 10) {
      StartCoroutine(stopMusic());
      //this.l1victory.startVictoryEvent();
      return;
    }
    //Change Slenderman's stats
    this.slender_script.setChaseSpeed(this.slender_stats.speeds[i]);
    this.slender_script.setLookDamage(this.slender_stats.look_limits[i]);
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
        StartCoroutine(playGradual(this.music[3]));
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
  public float[] speeds = {0.5f, 1, 2, 2.5f, 3, 3.5f, 3.8f, 4, 4.2f};
  public float[] look_limits = {10, 9, 8, 7, 6, 5, 4.5f, 4, 3.5f, 3};

  public float[] teleport_distances = {20, 15, 10, 9, 8, 7, 6, 5, 5};
  public float[] teleport_limits = {70, 60, 50, 40, 35, 30, 20, 15, 10};
  public float[] forward_tp_limits = {120, 120, 120, 120, 100, 80, 40, 30, 20};
  public bool[] can_tp_forward = {false, false, false, false, true, true, true, true, true};

  public float[] invisible_limits = {40, 80, 100, 110, 120, 120, 120, 120, 120};
  public bool[] can_be_invisible = {true, true, true, true, true, true, false, false, false};
}

class GhostStats {
  public float[] teleport_cooldown = {30, 25, 20, 15, 12, 10, 8, 6, 4};
  public float[] teleport_distances = {15, 12, 10, 9, 8, 8, 8, 8, 8};
  public float[] speeds = {5, 5.5f, 6, 6.5f, 7, 7.5f, 8, 8.5f, 9};
  public float[] invisibility_cooldown = {40, 38, 36, 34, 32, 30, 28, 26, 24};
}
