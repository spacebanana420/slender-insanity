using UnityEngine;

//Slenderman's class controls all of his behavior as well as stats
//Includes the following mechanics: static, chasing, teleportation, teleporting to the player's front, kill event, invisibility
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
  //Some timers change faster (e.g. invisible_meter decrementing at 2x speed)
  private float teleport_meter = 0;
  private float teleport_limit = 60;
  private float tp_forward_meter = 0;
  private float tp_forward_limit = 120;
  private bool can_teleport_forward = false;
  private float look_meter = 0;
  private float look_limit = 5;
  private  float jumpscare_meter = 15;
  private float jumpscare_limit = 15;
  private bool can_be_invisible = true;
  private float invisible_meter = 0;
  private float invisible_limit = 120;

  private float speed = 2f;
  private bool is_seen = false;

  //Set Slenderman's difficulty stats
  public void setTeleportation(float time, bool can_tp_forward, float forward_time) {
    this.teleport_limit = time;
    this.can_teleport_forward = can_tp_forward;
    this.tp_forward_limit = forward_time;
  }

  //Set Slenderman's difficulty stats
  public void setLookDamage(float time) {
    this.look_limit = time;
    //Avoids the player dying suddenly for making progress (e.g look_meter at 4.5 but look_limit changes from 5 to 4)
    if (this.look_meter >= time) {this.look_meter = time*0.7f;}
  }

  //Set Slenderman's difficulty stats
  public void setInvisibility(float time, bool can_be_invisible) {
    this.invisible_limit = time;
    this.can_be_invisible = can_be_invisible;
  }

  //Set Slenderman's difficulty stats
  public void setChaseSpeed(float speed) {this.speed = speed;}

  void Start() {this.static_object.active = true;}
  
  void Update() {
    Vector3 player_target = this.player.position; //For slender to look at
    player_target.y = this.transform.position.y;
    float distance = Vector3.Distance(this.transform.position, player_target);
    this.is_seen = this.model.isVisible;

    if (isInvisible()) {
      visibleCheck(distance, player_target);
      return;
    }
    teleport_check(distance, player_target);
    
    //Kill the player by proximity
    if (distance < 1.2f) {
      kill(player_target);
      return;
    }
    if (!this.is_seen) { //When not looking at Slender
      this.transform.LookAt(player_target);
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
        kill(player_target);
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
  void teleport_check(float distance, Vector3 player_target) {
    if (this.is_seen && distance <= 18) {return;}

    //Slender should teleport earlier if the player looks at him from afar
    int increment_speed = this.is_seen ? 2 : 1;
    this.teleport_meter = increment(this.teleport_meter, this.teleport_limit, increment_speed);
    if (this.can_teleport_forward) {this.tp_forward_meter = increment(this.tp_forward_meter, this.tp_forward_limit, increment_speed);}
    
    if (this.teleport_meter != this.teleport_limit) {return;}
    this.teleport_meter = 0;

    if (this.tp_forward_meter == this.tp_forward_limit) {
      this.tp_forward_meter = 0;
      teleport(distance, player_target, true); //Teleport to the player's front
      return;
    }
    //Teleporting behind the player isn't worth it if Slender is close to the player
    if (distance < 8) {return;}
    teleport(distance, player_target, false); //Teleport to the player's back
  }

  void teleport(float distance, Vector3 player_target, bool forward) {
    if (forward) { //Teleports to the player's front instead, not used yet
      this.controller.enabled = false; //Needed for manual position changes
      Vector3 player_pos = this.player.position;
      player_pos.y = this.transform.position.y;
      this.transform.position = player_pos + (this.player.forward * 6);
      this.controller.enabled = true;
    }
    else {controller.Move(transform.forward * (distance-4));}

    this.transform.LookAt(player_target);
  }

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
  void visibleCheck(float distance, Vector3 player_target) {
    this.invisible_meter = decrement(this.invisible_meter, 2);
    if (this.invisible_meter == 0) {return;}

    this.model.enabled = true;
    this.controller.enabled = true;
    //Teleport to keep up with the player after idling
    teleport(distance, player_target, false);
  }

  bool increaseStatic() {
    this.look_meter = increment(this.look_meter, this.look_limit, 1);
    return this.look_meter == this.look_limit;
  }

  void decreaseStatic() {
    this.look_meter = decrement(this.look_meter, 1.2f);
  }

  void kill(Vector3 player_target) {
    this.jumpscare_sound.Play();
    this.player_controller.caught = true;
    this.transform.LookAt(player_target);
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
