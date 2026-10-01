using UnityEngine;
using System.Collections;

//Pre-defined event for level 1 victory
//Slenderman jumpscares the player but does not kill him, revealing his "soul orb" instead
public class LevelExtra1Victory : MonoBehaviour
{
  public Transform player;
  public Transform player_cam;
  public Player player_script;
  public Pause pause_script;
  
  public Slenderman slender_script;
  public Orb orb_script;
  public GameObject pumpkin;
  
  public StaticEffect static_script;
  public BlankScreen blank_screen;
  public TextControl text;
  
  public AudioSource jumpscare;
  public AudioSource thunder;

  public LevelLoad level_loader;
  
  private GameObject slender;

  void Awake() {this.slender = this.slender_script.gameObject;}
  
  public void startVictoryEvent() {StartCoroutine(victoryEvent());}

  IEnumerator victoryEvent() {
    //Slender vanishes
    this.slender.active = false;
    this.slender_script.enabled = false;
    this.static_script.stopFade(4);
    yield return new WaitForSeconds(15);
    //Slender re-appears
    this.static_script.disableWeakStatic();
    this.static_script.setStatic_strong(1);
    yield return new WaitForSeconds(0.5f);
    this.static_script.stop();
    this.slender.active = true;
    this.jumpscare.Play();
    this.player_script.caught = true;
    emulateDeath(this.slender.transform, this.player, this.player_cam);

    float intensity = 0;
    while (intensity < 1) {
      this.static_script.setStatic_strong(intensity);
      intensity += 0.6f * Time.deltaTime;
      yield return null;
    }
    //Slender disappears, orb appears
    this.player_script.caught = false;
    turnIntoAPumpkin(this.slender, this.pumpkin);
    this.thunder.time=0.1f; //Skip silent part in audio
    this.thunder.Play();    
    this.static_script.stopFade_strong(4);
    this.blank_screen.displayWhiteScreen();
    yield return new WaitForSeconds(0.5f);
    this.blank_screen.fadeFromWhite(4);

    //Orb levitates and level ends
    yield return new WaitForSeconds(15);
    this.orb_script.levitateOrb();
    yield return new WaitForSeconds(12);
    this.pause_script.can_pause = false;
    this.player_script.caught = true;
    this.blank_screen.displayBlackScreen();
    yield return new WaitForSeconds(3);
    string[] ending_text = {
      "placeholder"
    };
    float duration = this.text.startSequence(ending_text);
    yield return new WaitForSeconds(duration);
    this.text.close();
    yield return new WaitForSeconds(2);
    level_loader.loadMainMenu();
  }

  //Positions Slender and the player as if the player had been caught
  //Inspired by Slenderman.kill() and other parts of Slender's class
  static void emulateDeath(Transform slender, Transform player, Transform player_cam) {
    //Move Slender next to the player, keep height the same
    Vector3 player_pos = player.position;
    player_pos.y = slender.position.y;
    slender.position = player_pos + (player.forward * 1.4f);

    //Look at Slender
    Vector3 slender_target = slender.position;
    slender_target.y = player_cam.position.y+0.6f;
    player_cam.LookAt(slender_target); 

    //Make Slender Look at player
    Vector3 player_target = player.position;
    player_target.y = slender.position.y;
    slender.LookAt(player_target);    
  }

  //Replaces Slenderman with a pumpkin
  static void turnIntoAPumpkin(GameObject slender, GameObject pumpkin) {
    slender.active = false;
    Vector3 slender_pos = slender.transform.position;
    slender_pos.y = pumpkin.transform.position.y;
    pumpkin.transform.position = slender_pos;
    pumpkin.active = true;
  }
}
