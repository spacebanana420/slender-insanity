using UnityEngine;

public class Slenderman : MonoBehaviour
{
  public CharacterController controller;
  public MeshRenderer model;
  public Transform player;
  public Transform player_camera;
  public Player player_controller;
  public AudioSource jumpscare_sound;
  public GameObject static_object;
  public StaticEffect static_script; //Handles the static effect
  public StaticKill kill_script; //Rapidly increases static, for the death screen

  //Meter and limit variables compose timers measured in seconds
  //e.g jumpscare_limit=15 means that it takes 15 seconds for the counter to end
  public float debug_distance;
  public float teleport_meter = 0;
  public float teleport_limit = 60;
  public float look_meter = 0;
  public float look_limit = 5;
  public float jumpscare_meter = 15;
  public float jumpscare_limit = 15;

  public bool can_be_invisible = true;
  public float invisible_meter = 0;
  public float invisible_limit = 120;

  public float speed = 2f;
  private bool is_seen;

  void Start() {this.static_object.active = true;}
  
  void Update() {
    Vector3 player_pos = this.player.position;
    player_pos.y = this.transform.position.y;
    float distance = Vector3.Distance(this.transform.position, player_pos);
    this.is_seen = this.model.isVisible;
    this.debug_distance = distance;

    if (is_invisible()) {
      turn_visible(distance, player_pos);
      return;
    }
    turn_invisible();
    teleport(distance, player_pos);
    
    //Kill the player by proximity
    if (distance < 1.2f) {
      kill(player_pos);
      return;
    }
    if (!this.is_seen) { //When not looking at Slender
      this.transform.LookAt(player_pos);
      this.controller.Move(transform.forward * speed * Time.deltaTime);
      decreaseStatic();
      jumpscare_count();
    }
    else { //When looking at Slender
      bool dead = distance > 18 ? false : increaseStatic();
      //Kill the player for staring for too long
      if (dead) {
        controller.Move(transform.forward * (distance-1.2f)); //Get close to the player to be identical to death by proximity
        kill(player_pos);
        return;
      }
      jumpscare(distance);
    }
    //Sets both static transparency and volume
    this.static_script.setStatic(this.look_meter/this.look_limit);
  }

  void jumpscare_count() {
    if (this.jumpscare_meter < this.jumpscare_limit) {
      this.jumpscare_meter += 1 * Time.deltaTime;
      return;
    }
    this.jumpscare_meter = this.jumpscare_limit; //Clamp
  }

  //Jumpscare sound logic, trigger the scare
  void jumpscare(float distance) {
    if (this.jumpscare_meter < this.jumpscare_limit || distance > 7f) {return;}
    this.jumpscare_meter = 0;
    this.jumpscare_sound.Play();
    return;
  }

  //Teleportation logic, count the timer or teleport
  void teleport(float distance, Vector3 player_pos) {
    if (this.is_seen && distance <= 18) {return;}

    if (this.teleport_meter < this.teleport_limit) {
      //Slender should teleport earlier if the player looks at him from afar, for balancing
      int increment_speed = this.is_seen ? 2 : 1;
      this.teleport_meter += increment_speed * Time.deltaTime;
      return;
    }
    this.teleport_meter = 0; //Clamp
    //Teleporting isn't worth it if Slender is close to the player
    if (distance < 8) {return;}
    this.transform.LookAt(player_pos);
    controller.Move(transform.forward * (distance-4));
  }

  bool is_invisible() {return !model.enabled;}

  //Count the timer for invisibility or turn invisible
  void turn_invisible() {
    if (!this.can_be_invisible || this.is_seen) {return;}
    if (this.invisible_meter < this.invisible_limit) {
      this.invisible_meter += 1 * Time.deltaTime;
      return;
    }
    this.invisible_meter = this.invisible_limit; //Clamp
    this.model.enabled = false;
    this.controller.enabled = false;
  }

  //Count the timer to revert invisibility or become visible
  void turn_visible(float distance, Vector3 player_pos) {
    if (this.invisible_meter > 0) {
      this.invisible_meter -= 2 * Time.deltaTime;
      return;
    }
    this.invisible_meter = 0; //Clamp
    this.model.enabled = true;
    this.controller.enabled = true;
    //Teleport to keep up with the player after idling
    this.transform.LookAt(player_pos);
    controller.Move(transform.forward * (distance-4));
  }

  //Returns true if the player looked for too long and so will die
  bool increaseStatic() {
    if (this.look_meter < this.look_limit) {
      this.look_meter += 1 * Time.deltaTime;
      return false;
    }
    this.look_meter = this.look_limit;
    return true;
  }

  void decreaseStatic() {
    if (this.look_meter > 0) {
      this.look_meter -= 1.2f * Time.deltaTime;
      return;
    }
    this.look_meter = 0;
  }

  void kill(Vector3 player_pos) {
    this.jumpscare_sound.Play();
    this.player_controller.caught = true;
    this.transform.LookAt(player_pos);
    Vector3 slender_pos = this.transform.position;
    slender_pos.y = this.player_camera.position.y+0.6f; //Make the camera look slightly up
    this.player_camera.LookAt(slender_pos);

    this.kill_script.enabled = true; //Initiates the game over event
    this.enabled = false;
  }
}
