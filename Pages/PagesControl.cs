using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Controls difficulty according to the number of collected pages
public class PagesControl : MonoBehaviour
{
  public GameObject slenderman;
  public Slenderman slender_script;
  public TextControl text;
  public Level1Victory l1victory;

  //Used for random page placement in map
  public Transform[] pages;
  public List<Transform> page_placements;
  public AudioSource[] music;

  public byte pages_collected = 0;

  private float thump_volume; //Preserves original thump volume so it can be played manually from other classes

  //Slenderman's stats, difficulty adjustment
  //One value for each page collected (1 to 7 pages)
  float[] speeds = {1, 1.5f, 2.5f, 3.5f, 4, 4.5f, 4.8f};
  float[] look_limits = {7, 6, 5, 4, 3.5f, 3.2f, 3};

  float[] teleport_distances = {8, 7, 6.2f, 5.8f, 5.5f, 5, 4.8f};
  float[] teleport_limits = {35, 30, 26, 22, 20, 12, 6};
  float[] forward_tp_limits = {100, 100, 100, 60, 40, 25, 20};
  bool[] can_tp_forward = {false, false, true, true, true, true, true};

  float[] invisible_limits = {60, 80, 100, 110, 120, 120, 120};
  bool[] can_be_invisible = {true, true, true, true, true, false, false};

  void Awake() {
    this.thump_volume = this.music[0].volume;
    //Random page placement
    foreach (Transform page in this.pages) {
      int i = Random.Range(0, this.page_placements.Count);
      page.position = this.page_placements[i].position;
      page.rotation = this.page_placements[i].rotation;
      this.page_placements.RemoveAt(i);
    }
  }

  //External classes can play this sound, it's a nice sound to use in some parts of the game
  public void playThump() {
    this.music[0].volume = this.thump_volume;
    this.music[0].Play();
  }

  //Each page calls this function when it's collected
  //Handles music, Slender's difficulty as well as the level 1 victory event
  public void collectPage() {
    int i = this.pages_collected;
    this.pages_collected += 1;
    this.text.displayTemporaryText(this.pages_collected+"/8 pages collected", 4);
    
    if (this.pages_collected == 8) {
      StartCoroutine(stopMusic());
      this.l1victory.enabled = true;
      return;
    }
    this.slender_script.setChaseSpeed(this.speeds[i]);
    this.slender_script.setLookDamage(this.look_limits[i]);
    this.slender_script.setTeleportDistance(this.teleport_distances[i]);
    this.slender_script.setTeleportation(this.teleport_limits[i], this.can_tp_forward[i], this.forward_tp_limits[i]);
    this.slender_script.setInvisibility(this.invisible_limits[i], this.can_be_invisible[i]);
    
    switch (this.pages_collected) {
      case 1:
        this.slenderman.active = true;
        StartCoroutine(firstMusic(this.music[0]));
        return;
      case 3:
        StartCoroutine(playGradual(this.music[1]));
        break;
      case 5:
        StartCoroutine(playGradual(this.music[2]));
        break;
      case 7:
        StartCoroutine(playGradual(this.music[3]));
        break;
    }
  }

  //For the thump sound, more granular control over how frequently it's heard
  IEnumerator firstMusic(AudioSource music) {
    music.Play();
    while (this.pages_collected < 8) {
      yield return new WaitForSeconds(4);
      music.time = 0; //Alternative to constant Play() calls, allows external scripts to stop the music
    }
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
