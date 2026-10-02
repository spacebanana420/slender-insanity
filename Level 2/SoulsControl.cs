using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Controls difficulty according to the number of souls released
public class SoulsControl : MonoBehaviour
{
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

  private GameObject slenderman;
  private float thump_volume; //Preserves original thump volume so it can be played manually from other classes


  void Awake() {
    this.slenderman = this.slender_script.gameObject;
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
    //Change Slenderman and ghost stats in percentage
    float difficultyPercentage = (float)this.souls_released/9;
    this.slender_script.setDifficulty(difficultyPercentage);
    this.ghost_script.setDifficulty(difficultyPercentage);
    
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
        this.music[3].time = 0.6f; //Skip slow introduction
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
      yield return null;
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
      if (all_done) break;
      yield return null;
    }
  }
}
