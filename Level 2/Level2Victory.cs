using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

//Pre-defined event for level 2 victory
public class Level2Victory : MonoBehaviour
{
  public Transform player;
  public Transform player_cam;
  public Player player_script;
  public CharacterController player_controller;
  public Pause pause_script;
  
  public SCPGhost ghost;
  public EnemyAPI ghost_api;
  
  public BlankScreen screen;
  public TextControl text;
  
  public AudioSource jumpscare;
  public AudioSource environment;
  public LevelLoad level_loader;
  public Material end_skybox;

  //10 orbs rotating around a center
  public Transform orb;
  private GameObject[] orbs = new GameObject[10];
  private Transform[] orbs_t = new Transform[10];
  private Material[] orbs_m = new Material[10];
  private Light[] orbs_l = new Light[10];
  private float[] angles = new float[10];
  private Vector3 center; //The center of the circle is defined by the first orb's initial position
  private bool rotate = false;
  private bool disappear = false;

  void Start() {startVictoryEvent();}
  
  public void startVictoryEvent() {StartCoroutine(victoryEvent());}

  IEnumerator victoryEvent() {
    GameObject ghost_obj = this.ghost.gameObject;
    this.ghost.enabled = false;
    yield return new WaitForSeconds(15);
    //Ghost jumpscare
    ghost_obj.active = true;
    this.ghost_api.teleport(this.ghost_api.getDistance(), 1.8f);
    this.ghost_api.killPlayer();
    this.jumpscare.Play();
    this.player_script.caught = true;
    this.pause_script.can_pause = false;
    yield return new WaitForSeconds(0.3f);
    this.screen.fadeToBlack(0.1f);
    yield return new WaitForSeconds(7);
    //Wake up with the 10 orbs floating
    ghost_obj.active = false;
    this.player_script.caught = false;
    this.player_controller.enabled = false;
    this.player.position = new Vector3(514.203f, 0.09f, 437.17f); //Pre-defined position, close to the rotating orbs
    this.player_controller.enabled = true;
    this.screen.fadeFromBlack(10f);
    changeTimeOfDay();
    spawnOrbs();
    yield return new WaitForSeconds(20);
    this.disappear = true;
    
    yield return new WaitForSeconds(15);
    this.screen.displayBlackScreen();
    this.player_script.caught = true;
    yield return new WaitForSeconds(3);
    this.text.displayText("Bound to a world where they were left behind...");
    yield return new WaitForSeconds(6);
    this.text.displayText("Long forgotten, the lost souls have finally found peace");
    yield return new WaitForSeconds(6);
    this.text.displayText("The terror brought to this small town has come to an end");
    yield return new WaitForSeconds(6);
    this.text.close();
    yield return new WaitForSeconds(1);
    level_loader.loadScene("Main Menu");
  }

  //Rotate the orbs and eventually make them disappear
  void Update() {
    if (!this.rotate) return;
    for (int i = 0; i < this.orbs.Length; i++) {
      this.angles[i] = Mathf.Repeat(this.angles[i] + 0.1f * Time.deltaTime, 2*Mathf.PI);
      this.orbs_t[i].position = getOrbPosition(this.angles[i]);
    }
    if (!this.disappear) return;
    foreach (Material m in this.orbs_m) { //Orbs disappear
      Color c = m.color;
      c.a -= 0.1f * Time.deltaTime;
      if (c.a < 0) c.a = 0;
      m.color = c;
    }
    //Orb lights fade out
    foreach (Light l in this.orbs_l) {l.intensity -= 0.3f * Time.deltaTime;}
  }

  void changeTimeOfDay() {
    this.environment.Stop();
    RenderSettings.skybox = this.end_skybox;
    RenderSettings.ambientMode = AmbientMode.Flat;
    RenderSettings.ambientLight = new Color32(130, 130, 130, 255);
    RenderSettings.fogDensity *= 0.95f;
    RenderSettings.fogColor = new Color32(255, 255, 255, 255);
  }

  //Duplicate the original orb, make them a circle of orbs
  void spawnOrbs() {
    //Getting all orb objects
    this.center = this.orb.position;
    this.orbs[0] = this.orb.gameObject;
    for (int i = 1; i < orbs.Length; i++) {this.orbs[i] = Object.Instantiate(this.orbs[0]);}

    //Assigning the components for each orb
    for (int i = 0; i < orbs.Length; i++) {
      this.orbs_t[i] = this.orbs[i].GetComponent<Transform>();
      this.orbs_m[i] = this.orbs[i].GetComponent<MeshRenderer>().material;
      this.orbs_l[i] = this.orbs[i].GetComponent<Light>();
    }

    //Set the position of each orb around a center point, making a circle
    float angle_step = 36*Mathf.PI/180; //360 degrees, 36 degrees for each orb, converted to radians
    float angle = Mathf.PI/2; //Start at 90 degrees
    for (int i = 0; i < orbs.Length; i++) {
      this.orbs_t[i].position = getOrbPosition(angle);
      this.angles[i] = angle;
      this.orbs[i].active = true;
      angle += angle_step;
    }
    this.rotate = true;
  }

  //Get the position around the center for a particular orb
  Vector3 getOrbPosition(float angle_rad) {
    Vector3 new_pos = new Vector3();
    new_pos.x = this.center.x;
    new_pos.y = this.center.y;
    new_pos.z = this.center.z;

    //Radius 2
    new_pos.y += 2 * Mathf.Sin(angle_rad);
    new_pos.z += 2 * Mathf.Cos(angle_rad);
    return new_pos;
  }
}
