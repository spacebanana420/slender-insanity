using UnityEngine;
using System.Collections;

//Pre-defined event for level 1 victory
//Slenderman jumpscares the player but does not kill him, revealing his "soul orb" instead
public class Level1Victory : MonoBehaviour
{
  public Transform player;
  public Transform player_cam;
  public Player player_script;
  
  public GameObject slender;
  public Slenderman slender_script;
  public Orb orb_script;
  public GameObject orb;
  
  public StaticEffect static_script;
  public BlankScreen blank_screen;
  
  public AudioSource jumpscare;
  public AudioSource thunder;
  
  void Start() {StartCoroutine(victoryEvent());}

  IEnumerator victoryEvent() {
    yield return new WaitForSeconds(15);
    float intensity = 0;
    this.static_script.setStatic_strong(1);
    yield return new WaitForSeconds(0.3f);
    this.static_script.stop();
    this.slender_script.enabled = false;
    this.slender.active = true;
    this.jumpscare.Play();
    this.player_script.caught = true;
    emulateDeath(this.slender.transform, this.player, this.player_cam);
    
    while (intensity < 1) {
      this.static_script.setStatic_strong(intensity);
      intensity += 0.6f * Time.deltaTime;
      yield return new WaitForSeconds(0.005f);
    }
    this.player_script.caught = false;
    this.slender.active = false;
    this.orb.active = true;
    this.thunder.Play();
    this.static_script.setStatic_strong(0);
    this.blank_screen.displayWhiteScreen();
    this.blank_screen.fadeFromWhite(4);
    yield return new WaitForSeconds(10);
    this.orb_script.levitateOrb();
    yield return new WaitForSeconds(10);
    this.blank_screen.displayBlackScreen();
  }

  //Positions Slender and the player as if the player had been caught
  //Duplicate code, taken from Slenderman.kill() and other parts of Slender's class
  void emulateDeath(Transform slender, Transform player, Transform player_cam) {
    //Move Slender next to the player, keep height the same
    Vector3 player_pos = player.position;
    player_pos.y = slender.position.y;
    slender.position = player_pos + (player.forward * 1.2f);

    //Look at Slender
    Vector3 slender_target = slender.position;
    slender_target.y = player_cam.position.y+0.6f;
    player_cam.LookAt(slender_target); 

    //Make Slender Look at player
    Vector3 player_target = player.position;
    player_target.y = slender.position.y;
    slender.LookAt(player_target);    
  }
}
