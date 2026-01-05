using UnityEngine;

//Slenderman's class controls all of his behavior as well as stats
//Includes the following mechanics: static, chasing, teleportation, teleporting to the player's front, kill event, invisibility
public class Slenderman : MonoBehaviour
{
  public EnemyAPI api;
  
  public AudioSource jumpscare_sound;
  public StaticEffect static_script; //Handles the static effect
  public StaticKill kill_script; //Rapidly increases static, for the death screen

  //Terrain for outdoor levels, floor for interior ones
  //Defines the teleportation method
  public Terrain terrain;
  public Transform floor;

  public GameObject[] other_enemies; //Disale all of them when Slender kills the player to avoid conflict

  //Variables used for counting timers
  //e.g jumpscare_limit=15 means that it takes 15 seconds for the counter to end
  //Some timers count faster (e.g. teleport_meter counting faster when Slender is seen from afar)
  private float teleport_meter = 0;
  private float teleport_limit = 60;
  private float tp_forward_meter = 0;
  private float tp_forward_limit = 120;
  private bool can_teleport_forward = false;
  private float look_meter = 0;
  private float jumpscare_meter = 15;
  private float jumpscare_limit = 15;
  private bool can_be_invisible = true;
  private float invisible_meter = 0;
  private float invisible_limit = 120;
  private float invisible_countdown = 18;

  private float speed = 2f;
  private float teleport_distance = 4;
  private bool looking_at = false;
  private bool is_seen = false;
  private bool usewaypoints = false;
  private float static_distance = 15; //Beyond this distance, he cannot attack with static interference

  //Set Slenderman's difficulty stats
  public void setTeleportation(float time, bool can_tp_forward, float forward_time) {
    this.teleport_limit = time;
    this.can_teleport_forward = can_tp_forward;
    this.tp_forward_limit = forward_time;
  }
  public void setInvisibility(float time, bool can_be_invisible) {
    this.invisible_limit = time;
    this.can_be_invisible = can_be_invisible;
  }
  public void setChaseSpeed(float speed) {this.speed = speed;}
  public void setTeleportDistance(float dist) {this.teleport_distance = dist;}

  void Start() {
    this.static_script.gameObject.active = true;
    this.usewaypoints = this.terrain == null;
  }

  //Slender's primary function, handles all logic from a high level
  void Update() {
    float distance = this.api.getDistance();
    this.looking_at = this.api.isLookedAt(); //Slenderman in player's field of view
    this.is_seen = this.api.isSeen(); //Slenderman visible (not blocked by any object)

    if (isInvisible()) {
      adjustStatic(distance);
      visibleCheck(distance);
      return;
    }
    //Kill the player by proximity
    if (distance < 1.7f) {
      kill();
      return;
    }
    bool killed = adjustStatic(distance);
    if (killed) { //Kill the player for staring for too long
      this.api.lookAtPlayer();
      this.api.move(distance-1.7f);
      kill();
      return;
    }

    jumpscareCheck(distance);
    teleportCheck(distance);

    if (!this.looking_at) {
      this.api.lookAtPlayer();
      this.api.move(this.speed);
      invisibleCheck();
    }
  }

  void jumpscareCheck(float distance) {
    if (!this.is_seen) {
      this.jumpscare_meter = increment(this.jumpscare_meter, this.jumpscare_limit, 1);
      return;
    }
    if (this.jumpscare_meter < this.jumpscare_limit || distance > 8f) {return;}
    this.jumpscare_meter = 0;
    this.jumpscare_sound.Play();
    return;
  }

  //Teleportation logic, count the timer and eventually teleport
  void teleportCheck(float distance) {
    if (this.is_seen && distance <= this.static_distance) return;
    int increment_speed = this.is_seen ? 3 : 1; //Slender should teleport earlier if the player looks at him from afar
    this.teleport_meter = increment(this.teleport_meter, this.teleport_limit, increment_speed);
    if (this.can_teleport_forward) {this.tp_forward_meter = increment(this.tp_forward_meter, this.tp_forward_limit, increment_speed);}
    
    if (this.teleport_meter != this.teleport_limit) {return;}
    this.teleport_meter = 0;
    
    if (this.usewaypoints) teleportCheck_waypoints(distance);
    else teleportCheck_terrain(distance);
  }

  //Outdoor map teleportation, move instantly or go to the player's view
  void teleportCheck_terrain(float distance) {
    if (this.tp_forward_meter == this.tp_forward_limit) {
      this.tp_forward_meter = 0;
      teleport(distance, true); //Teleport to the player's front
      return;
    }
    //Teleporting behind the player isn't worth it if Slender is close to the player
    if (distance < this.teleport_distance) return;
    teleport(distance, false); //Teleport to the player's back
  }

  //Indoor map teleportation, teleport to the nearest waypoint to the player
  void teleportCheck_waypoints(float distance) {
    if (distance < this.teleport_distance) return;
    this.api.teleportWaypoint(this.floor, this.teleport_distance);
    this.jumpscare_meter = increment(this.jumpscare_meter, this.jumpscare_limit, 7); //Advance jumpscare meter a bit
  }

  //Slenderman can teleport to the player's front or not
  //If he creeps up behind the player, he simply moves very fast, following collisions properly
  //In forward teleportation, he is manually positioned and then gravity is applied so he doesn't stand mid-air
  void teleport(float distance, bool forward) {
    if (forward) { //Teleports to the player's front instead
      this.api.teleportForward(this.terrain);
      this.jumpscare_meter = this.jumpscare_limit;
      return;
    }
    this.api.teleport(distance, this.teleport_distance);
    this.jumpscare_meter = increment(this.jumpscare_meter, this.jumpscare_limit, 7); //Advance jumpscare meter a bit
  }

  bool isInvisible() {return !this.api.isMeshEnabled();}

  //Count the timer for invisibility or turn invisible
  void invisibleCheck() {
    if (!this.can_be_invisible) {return;}
    this.invisible_meter = increment(this.invisible_meter, this.invisible_limit, 1);

    if (this.invisible_meter != this.invisible_limit) {return;}
    this.is_seen = false;
    this.looking_at = false;
    this.api.toggleController(false);
    this.api.toggleMesh(false);
  }

  //Invisibility duration countdown, then become visible
  void visibleCheck(float distance) {
    this.invisible_countdown = decrement(this.invisible_countdown, 1);
    if (this.invisible_countdown != 0) {return;}
    this.invisible_countdown = 18;
    this.invisible_meter = 0;

    this.api.toggleController(true);
    this.api.toggleMesh(true);
    teleport(distance, false);
  }

  //Increases or decreases static, also returns true if the player dies for staring at Slender
  //Static is weaker the farther Slender is
  bool adjustStatic(float distance) {
    if (this.is_seen && distance <= this.static_distance) {
      float static_speed = 22/distance; //Takes 10/22 seconds to kill the player at 1 distance
      this.look_meter = increment(this.look_meter, 10, static_speed);
    }
    else this.look_meter = decrement(this.look_meter, 2.2f);
    
    this.static_script.setStatic(this.look_meter/10);
    return this.look_meter == 10;
  }

  void kill() {
    foreach (GameObject enemy in this.other_enemies) enemy.active = false; //Avoid conflicts
    this.jumpscare_sound.Play();
    this.api.killPlayer();
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
