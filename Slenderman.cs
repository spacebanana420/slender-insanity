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
  private float teleport_meter = 0;
  public float teleport_limit = 60;
  public float look_meter = 0;
  public float look_limit = 5;
  private  float jumpscare_meter = 15;
  public float jumpscare_limit = 15;

  public bool can_be_invisible = true;
  private float invisible_meter = 0;
  public float invisible_limit = 120;

  public float speed = 2f;
  private bool is_seen = false;

  void Start() {this.static_object.active = true;}
  
  void Update() {
    Vector3 player_pos = this.player.position;
    player_pos.y = this.transform.position.y;
    float distance = Vector3.Distance(this.transform.position, player_pos);
    this.is_seen = this.model.isVisible;

    if (isInvisible()) {
      visibleCheck(distance, player_pos);
      return;
    }
    teleport_check(distance, player_pos);
    
    //Kill the player by proximity
    if (distance < 1.2f) {
      kill(player_pos);
      return;
    }
    if (!this.is_seen) { //When not looking at Slender
      this.transform.LookAt(player_pos);
      this.controller.Move(transform.forward * speed * Time.deltaTime);
      jumpscareCount();
      decreaseStatic();
      invisibleCheck();
    }
    else { //When looking at Slender
      bool dead = distance > 18 ? false : increaseStatic();
      //Kill the player for staring for too long
      if (dead) {
        controller.Move(transform.forward * (distance-1.2f)); //Get close to the player to be identical to death by proximity
        kill(player_pos);
        return;
      }
      jumpscareCheck(distance);
    }
    //Sets both static transparency and volume
    this.static_script.setStatic(this.look_meter/this.look_limit);
  }

  void jumpscareCount() {
    this.jumpscare_meter = increment(this.jumpscare_meter, this.jumpscare_limit, 1);
  }

  void jumpscareCheck(float distance) {
    if (this.jumpscare_meter < this.jumpscare_limit || distance > 7f) {return;}
    this.jumpscare_meter = 0;
    this.jumpscare_sound.Play();
    return;
  }

  //Teleportation logic, count the timer or teleport
  void teleport_check(float distance, Vector3 player_pos) {
    if (this.is_seen && distance <= 18) {return;}

    //Slender should teleport earlier if the player looks at him from afar, for balancing
    int increment_speed = this.is_seen ? 2 : 1;
    this.teleport_meter = increment(this.teleport_meter, this.teleport_limit, increment_speed);
    if (this.teleport_meter != this.teleport_limit) {return;}
    this.teleport_meter = 0;
    
    //Teleporting isn't worth it if Slender is close to the player
    if (distance < 8) {return;}
    teleport(distance, player_pos, false);
  }

  void teleport(float distance, Vector3 player_pos, bool forward) {
    if (forward) { //Teleports forward instead of backwards, not used yet
      this.controller.enabled = false; //Needed for manual position changes
      this.transform.position = this.player.position + (this.player.forward * 4);
      this.controller.enabled = true;
    }
    else {controller.Move(transform.forward * (distance-4));}

    this.transform.LookAt(player_pos);
  }

  //Implement later
  // void teleportForward(float distance, Vector3 player_pos, Vector3 player_direction) {
  //   this.controller.enabled = false; //Needed for manual position changes
  //   this.transform.position = this.player.position + (player_direction * 4);
  //   this.controller.enabled = true;
  //   this.transform.lookAt(player_pos);
  // }

  bool isInvisible() {return !model.enabled;}

  //Count the timer for invisibility or turn invisible
  void invisibleCheck() {
    if (!this.can_be_invisible) {return;}
    this.invisible_meter = increment(this.invisible_meter, this.invisible_limit, 1);

    if (this.invisible_meter != this.invisible_limit) {return;}
    this.model.enabled = false;
    this.controller.enabled = false;
  }

  //Count the timer to revert invisibility or become visible
  void visibleCheck(float distance, Vector3 player_pos) {
    this.invisible_meter = decrement(this.invisible_meter, 2);
    if (this.invisible_meter == 0) {return;}

    this.model.enabled = true;
    this.controller.enabled = true;
    //Teleport to keep up with the player after idling
    teleport(distance, player_pos);
  }

  bool increaseStatic() {
    this.look_meter = increment(this.look_meter, this.look_limit, 1);
    return this.look_meter == this.look_limit;
  }

  void decreaseStatic() {
    this.look_meter = decrement(this.look_meter, 1.2f);
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

  //Generic functions for incrementing and decrementing counters with clamp
  float increment(float counter, float max_value, float step) {
    if (counter < max_value) {
      return counter + step * Time.deltaTime;
    }
    return max_value;
  }

  float decrement(float counter, float step) {
    if (counter > 0) {
      return counter - step * Time.deltaTime;
    }
    return 0;
  }
}
