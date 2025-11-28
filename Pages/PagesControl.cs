using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Controls difficulty according to the number of collected pages
public class PagesControl : MonoBehaviour
{
  public GameObject slenderman;
  public Slenderman slender_script;
  public TextControl text;
  public StaticEffect static_effect;

  //Used for random page placement in map
  public Transform[] pages;
  public List<Transform> page_placements;
  public AudioSource[] music;

  private byte pages_collected = 0;

  //Slenderman's stats, difficulty adjustment
  //One value for each page collected (1 to 7 pages)
  float[] speeds = {0.5f, 1, 2, 3, 4, 5, 5.5f};
  float[] look_limits = {8, 7, 6, 5, 4, 3, 2};
  
  float[] teleport_limits = {30, 25, 20, 15, 10, 5, 5};
  float[] forward_tp_limits = {120, 120, 120, 80, 60, 30, 15};
  bool[] can_tp_forward = {false, false, true, true, true, true, true};

  float[] invisible_limits = {80, 90, 100, 110, 120, 120, 120};
  bool[] can_be_invisible = {true, true, true, true, true, false, false};

  void Start() {
    //Random page placement
    foreach (Transform page in this.pages) {
      int i = Random.Range(0, this.page_placements.Count-1);
      page.position = this.page_placements[i].position;
      page.rotation = this.page_placements[i].rotation;
      this.page_placements.RemoveAt(i);
    }
  }

  //Each page calls this function when it's collected
  //Handles music, Slender's difficulty as well as the level 1 victory event
  public void collectPage() {
    int i = this.pages_collected;
    this.pages_collected += 1;
    StartCoroutine(displayText(this.pages_collected));
    if (this.pages_collected == 8) {
      victory();
      return;
    }
    this.slender_script.setChaseSpeed(this.speeds[i]);
    this.slender_script.setLookDamage(this.look_limits[i]);
    this.slender_script.setTeleportation(this.teleport_limits[i], this.can_tp_forward[i], this.forward_tp_limits[i]);
    this.slender_script.setInvisibility(this.invisible_limits[i], this.can_be_invisible[i]);
    
    switch (this.pages_collected) {
      case 1:
        this.slenderman.active = true;
        StartCoroutine(firstMusic(this.music[0]));
        return;
      case 3:
        this.music[1].Play();
        break;
      case 5:
        this.music[2].Play();
        break;
      case 7:
        this.music[3].Play();
        break;
    }
  }

  //For the thump sound, more granular control over how frequently it's heard
  IEnumerator firstMusic(AudioSource music) {
    while (this.pages_collected < 8) {
      music.Play();
      yield return new WaitForSeconds(4);
    }
  }

  void victory() {
    this.slenderman.active = false;
    this.static_effect.stop();
    StartCoroutine(stopMusic());
    StartCoroutine(victoryEvent());
  }
  
  //Need to finish
  IEnumerator victoryEvent() {
    yield return new WaitForSeconds(15);
    this.static_effect.setStatic_strong(1);
    yield return new WaitForSeconds(0.3f);
    this.static_effect.stop();
  }

  IEnumerator displayText(byte page_count) {
    this.text.displayText(page_count+"/8 pages collected");
    yield return new WaitForSeconds(4);
    this.text.close();
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
