using UnityEngine;

//Slenderman's class controls all of his behavior as well as stats
//Includes the following mechanics: static, chasing, teleportation, teleporting to the player's front, kill event, invisibility
public class Slenderman : MonoBehaviour
{
  //Slender's components
  public CharacterController controller;
  public MeshRenderer model;

  //Player's components
  public Transform player;
  public Transform player_camera;
  public Player player_controller;
  
  public AudioSource jumpscare_sound;
  public GameObject static_object;
  public StaticEffect static_script; //Handles the static effect
  public StaticKill kill_script; //Rapidly increases static, for the death screen
  public Terrain terrain;

  //Variables used for counting timers
  //e.g jumpscare_limit=15 means that it takes 15 seconds for the counter to end
  //Some timers count faster (e.g. teleport_meter counting faster when Slender is seen from afar)
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
  private float invisible_countdown = 30;

  private float speed = 2f;
  private float teleport_distance = 4;
  private bool looking_at = false;
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

  //Set Slenderman's difficulty stats
  public void setTeleportDistance(float dist) {this.teleport_distance = dist;}

  void Start() {this.static_object.active = true;}

  //Slender's primary function, handles all logic from a high level
  //Most functions it calls check for whether he is visible to the player or not
  void Update() {
    float distance = Vector3.Distance(this.transform.position, this.player.position);
    this.looking_at = this.model.isVisible; //Slenderman in player's field of view
    this.is_seen = isSeenByPlayer(); //Slenderman visible from player's field of view

    if (isInvisible()) {
      adjustStatic(distance);
      visibleCheck(distance);
      return;
    }
    //Kill the player by proximity
    if (distance < 1.5f) {
      kill();
      return;
    }
    bool stare_death = adjustStatic(distance);
    if (stare_death) { //Kill the player for staring for too long
      controller.Move(transform.forward * (distance-1.2f)); //Get close to the player to be identical to death by proximity
      controller.Move(new Vector3(0, -100, 0));
      kill();
      return;
    }

    jumpscareCheck(distance);
    teleportCheck(distance);
    
    if (!this.looking_at) {
      lookAtPlayer();
      Vector3 motion = transform.forward * speed;
      motion.y = -10; //Gravity
      this.controller.Move(motion * Time.deltaTime);
      invisibleCheck();
    }
  }

  bool isSeenByPlayer() {
    if (!this.model.isVisible) {return false;}
    RaycastHit hit_info;
    bool collided = Physics.Raycast(this.player_camera.position, this.transform.position-this.player_camera.position, out hit_info, 30);
    if (!collided) {return false;}
    return hit_info.collider.gameObject == this.gameObject;
  }

  void lookAtPlayer() {
    Vector3 look_target = this.player.position;
    look_target.y = this.transform.position.y;
    this.transform.LookAt(look_target);
  }

  void jumpscareCheck(float distance) {
    if (!this.is_seen) {
      this.jumpscare_meter = increment(this.jumpscare_meter, this.jumpscare_limit, 1);
      return;
    }
    if (this.jumpscare_meter < this.jumpscare_limit || distance > 7f) {return;}
    this.jumpscare_meter = 0;
    this.jumpscare_sound.Play();
    return;
  }

  //Teleportation logic, count the timer or teleport
  void teleportCheck(float distance) {
    if (this.is_seen && distance <= 18) {return;}

    //Slender should teleport earlier if the player looks at him from afar
    int increment_speed = this.is_seen ? 2 : 1;
    this.teleport_meter = increment(this.teleport_meter, this.teleport_limit, increment_speed);
    if (this.can_teleport_forward) {this.tp_forward_meter = increment(this.tp_forward_meter, this.tp_forward_limit, increment_speed);}
    
    if (this.teleport_meter != this.teleport_limit) {return;}
    this.teleport_meter = 0;

    if (this.tp_forward_meter == this.tp_forward_limit) {
      this.tp_forward_meter = 0;
      teleport(distance, true); //Teleport to the player's front
      return;
    }
    //Teleporting behind the player isn't worth it if Slender is close to the player
    if (distance < 8) {return;}
    teleport(distance, false); //Teleport to the player's back
  }

  //Slenderman can teleport to the player's front or not
  //If he creeps up behind the player, he simply moves very fast, following collisions properly
  //In forward teleportation, he is manually positioned and then gravity is applied so he doesn't stand mid-air
  void teleport(float distance, bool forward) {
    //Teleports to the player's front instead
    if (forward) {
      this.controller.enabled = false;
      Vector3 new_position = this.player.position + (this.player.forward * 6);
      new_position.y = this.terrain.SampleHeight(new_position)+1.5f; //Prevents the possibility of teleporting below ground
      this.transform.position = new_position;
      this.controller.enabled = true;
      this.jumpscare_meter = this.jumpscare_limit;
    }
    //Teleport behind the player instead
    else {this.controller.Move(transform.forward * (distance-this.teleport_distance));}
    this.controller.Move(new Vector3(0, -100, 0)); //Gravity
    lookAtPlayer();
  }

  bool isInvisible() {return !model.enabled;}

  //Count the timer for invisibility or turn invisible
  void invisibleCheck() {
    if (!this.can_be_invisible) {return;}
    this.invisible_meter = increment(this.invisible_meter, this.invisible_limit, 1);

    if (this.invisible_meter != this.invisible_limit) {return;}
    this.is_seen = false;
    this.looking_at = false;
    this.model.enabled = false;
    this.controller.enabled = false;
  }

  //Invisibility duration countdown, then become visible
  void visibleCheck(float distance) {
    this.invisible_countdown = decrement(this.invisible_countdown, 1);
    if (this.invisible_countdown != 0) {return;}
    this.invisible_countdown = 30;
    this.invisible_meter = 0;

    this.model.enabled = true;
    this.controller.enabled = true;
    teleport(distance, false);
  }

  //Increases or decreases static, also returns true if the player dies for staring at Slender
  bool adjustStatic(float distance) {
    if (this.is_seen && distance <= 18) {
      this.look_meter = increment(this.look_meter, this.look_limit, 1);
    }
    else {
      this.look_meter = decrement(this.look_meter, 0.9f);
    }
    this.static_script.setStatic(this.look_meter/this.look_limit);
    return this.look_meter == this.look_limit;
  }

  void kill() {
    this.jumpscare_sound.Play();
    this.player_controller.caught = true;
    Vector3 slender_pos = this.transform.position;
    slender_pos.y = this.player_camera.position.y+0.6f; //Make the camera look slightly up
    this.player_camera.LookAt(slender_pos);
    lookAtPlayer();

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
