using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Controls difficulty according to the number of collected pumpkins
public class PumpkinControl : MonoBehaviour
{
  public Slenderman slender_script;
  public SCPGhost ghost;
  public TextControl text;
  public StaticKill gameover;

  //Used for random pumpkin placement in map
  public Transform[] pumpkins;
  public List<Transform> pumpkin_placements;
  public AudioSource[] music;
  public LevelExtra1Victory victory;

  private byte collected = 0; //How many pumpkins have been collected

  private GameObject slenderman;
  private byte thumpFrequency = 8; //Frequency increases with more pumpkins collected

  void Awake() {
    this.slenderman = this.slender_script.gameObject;
    //Random page placement
    foreach (Transform pumpkin in this.pumpkins) {
      int i = Random.Range(0, this.pumpkin_placements.Count);
      pumpkin.position = this.pumpkin_placements[i].position;
      pumpkin.rotation = this.pumpkin_placements[i].rotation;
      this.pumpkin_placements.RemoveAt(i);
    }
  }

  //Each page calls this function when it's collected
  //Handles music, Slender's difficulty as well as the level 1 victory event
  public void collectPumpkin() {
    this.collected += 1;
    string text = this.collected+"/12 pumpkins collected";
    this.text.displayTemporaryText(text, 4);
    this.gameover.gameover_text = text;
    
    if (this.collected == 12) {
      StartCoroutine(stopMusic());
      this.victory.startVictoryEvent();
      return;
    }
    float enemyDifficulty = (float)this.collected/11;
    this.slender_script.setDifficulty(enemyDifficulty); //Slender difficulty set in percentage
    this.ghost.setDifficulty(enemyDifficulty/3);// Ghost's difficulty is reduced for balancing
    
    switch (this.collected) {
      case 1:
        this.slenderman.active = true;
        StartCoroutine(playThump(this.music[0]));
        break;
      case 2:
        this.thumpFrequency = 6;
        break;
      case 4:
        StartCoroutine(playGradual(this.music[1]));
        this.ghost.gameObject.active = true;
        break;
      case 6:
        this.thumpFrequency = 4;
        break;
      case 8:
        StartCoroutine(playGradual(this.music[2]));
        break;
      case 10:
        StartCoroutine(playGradual(this.music[3]));
        break;
    }
  }

  //For the thump sound, more granular control over how frequently it's heard
  IEnumerator playThump(AudioSource music) {
    while (this.collected < 12) {
      music.Play();
      yield return new WaitForSeconds(this.thumpFrequency);
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
