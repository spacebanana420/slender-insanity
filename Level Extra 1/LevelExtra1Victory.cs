using UnityEngine;
using System.Collections;

//Pre-defined event for extra level 1 victory
//Slenderman jumpscares the player but turns into a pumpkin instead
public class LevelExtra1Victory : MonoBehaviour
{
  public Transform player;
  public Transform player_cam;
  
  public Slenderman slender_script;
  public GameObject pumpkin;
  public GameObject[] enemyGhosts;
  
  public StaticEffect static_script;
  public BlankScreen blank_screen;
  public TextControl text;
  
  public AudioSource thunder;

  public LevelLoad level_loader;
  
  private GameObject slender;
  private Player player_script;
  private Pause pause_script;
  private AudioSource pumpkinSound;

  void Awake() {
    this.player_script = this.player.gameObject.GetComponent<Player>();
    this.pause_script = this.player.gameObject.GetComponent<Pause>();
    this.slender = this.slender_script.gameObject;
    this.pumpkinSound = this.pumpkin.GetComponent<AudioSource>();
  }

  
  public void startVictoryEvent() {StartCoroutine(victoryEvent());}

  IEnumerator victoryEvent() {
    //Slender vanishes
    this.slender_script.disable();
    this.slender.active = false;
    foreach (GameObject ghost in this.enemyGhosts) {ghost.active = false;}
    this.static_script.stopFade(4);
    yield return new WaitForSeconds(15);
    //Slender re-appears
    this.static_script.disableWeakStatic();
    this.static_script.setStatic_strong(1);
    yield return new WaitForSeconds(0.5f);
    this.static_script.stop();
    this.slender.active = true;
    this.slender_script.jumpscare_sound.Play();
    this.player_script.caught = true;
    emulateDeath(this.slender.transform, this.player, this.player_cam);

    float intensity = 0;
    while (intensity < 1) {
      this.static_script.setStatic_strong(intensity);
      intensity += 0.6f * Time.deltaTime;
      yield return null;
    }
    //Slender disappears, turns into a pumpkin
    this.player_script.caught = false;
    turnIntoAPumpkin(this.slender, this.pumpkin);
    StartCoroutine(rotatePumpkin());
    this.thunder.time=0.1f; //Skip silent part in audio
    this.thunder.Play();    
    this.static_script.stopFade_strong(4);
    this.blank_screen.displayWhiteScreen();
    yield return new WaitForSeconds(0.5f);
    this.blank_screen.fadeFromWhite(4);

    //Pumpkin levitates and level ends
    yield return new WaitForSeconds(10);
    StartCoroutine(levitatePumpkin());
    yield return new WaitForSeconds(12);
    this.pause_script.can_pause = false;
    this.player_script.caught = true;
    this.blank_screen.displayBlackScreen();
    yield return new WaitForSeconds(3);
    string[] ending_text = {
      "The spirit of Halloween flourishes in us.",
      "Exciting trick or treat, endless corn fields, levitating pumpkins, and more.",
      "The souls of the damned thrive tonight, but so do we.",
      "Halloween is now clean of the devious green pumpkins and the tricksters that come with them.",
      "The everlasting night awaits us, the perfect October dream..."
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
    pumpkin.transform.position = slender.transform.position + new Vector3(0, 0.25f, 0);
    pumpkin.active = true;
  }

  IEnumerator rotatePumpkin() {
    while (true) {
      this.pumpkin.transform.Rotate(0, 40*Time.deltaTime, 0);
      yield return null;
    }
  }

  IEnumerator levitatePumpkin() {
    float elapsedTime = 0;
    float speed = 0;
    this.pumpkinSound.Play();
    while (elapsedTime < 20) {
      this.pumpkin.transform.Translate(0, speed * Time.deltaTime, 0);
      speed += 6f * Time.deltaTime;
      yield return null;
    }
  }
}
