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

  private byte pages_collected = 0;
  private bool first_page = true;

  //Slenderman's stats, difficulty adjustment
  //One value for each page collected (1 to 7 pages)
  float[] speeds = {0.5f, 1, 2, 3, 4, 5, 5.5f};
  float[] look_limits = {8, 7, 6, 5, 4, 3, 2};
  float[] teleport_limits = {30, 25, 20, 15, 10, 5, 5};
  float[] invisible_limits = {80, 90, 100, 110, 120, 120, 120};
  bool[] can_be_invisible = {true, true, true, true, true, false, false};

  void Start() {
    foreach (Transform page in this.pages) {
      int i = Random.Range(0, this.page_placements.Count-1);
      page.position = this.page_placements[i].position;
      page.rotation = this.page_placements[i].rotation;
      this.page_placements.RemoveAt(i);
    }
  }
  
  public void collectPage() {
    int i = this.pages_collected;
    this.pages_collected += 1;
    StartCoroutine(displayText(this.pages_collected));
    if (this.pages_collected == 8) {
      victory();
      return;
    }
    
    this.slender_script.speed = this.speeds[i];
    this.slender_script.look_limit = this.look_limits[i];
    this.slender_script.teleport_limit = this.teleport_limits[i];
    this.slender_script.invisible_limit = this.invisible_limits[i];
    this.slender_script.can_be_invisible = this.can_be_invisible[i];
    
    if (first_page) {
      this.slenderman.active = true;
      this.first_page = false;
    }
    //Adjust static intensity according to the changes done to look_limit
    //Prevents the player from dying for collecting a page (e.g look_limit decreases from 5 to 4 when the meter is at 4.5)
    else if (i > 0) {this.slender_script.look_meter -= (this.look_limits[i-1]-this.look_limits[i]);}
  }

  void victory() {//Unfinished of course
    this.static_effect.stop();
    this.slenderman.active = false;
  }
  
  //Todo
  // IEnumerator victoryEvent() {
  // }

  IEnumerator displayText(byte page_count) {
    this.text.displayText(page_count+"/8 pages collected");
    yield return new WaitForSeconds(4);
    this.text.close();
  }
}
