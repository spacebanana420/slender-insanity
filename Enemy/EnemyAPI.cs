using UnityEngine;

//Library class for various enemy-related mechanics and behavior
//Shared between different enemies in game to not duplicate code
public class EnemyAPI : MonoBehaviour
{
  public Transform player;
  public Transform player_camera;
  public Transform enemy;
  public MeshRenderer enemy_mesh;
  public CharacterController enemy_ctrl;

  public Player player_script;
  public Pause pause_script;

  private GameObject enemy_object;
  
  void Awake() {this.enemy_object = this.enemy.gameObject;}
  
  public float getDistance() {return Vector3.Distance(this.enemy.position, this.player.position);}

  //Whether the enemy is inside the player's field of view, even if hidden behind an object
  public bool isLookedAt() {return this.enemy_mesh.isVisible;}

  //Whether the enemy is seen without being hidden behind an object
  public bool isSeen() {
    if (!isLookedAt()) {return false;}
    Vector3[] ray_pos = {this.enemy.position, this.enemy.position, this.enemy.position};
    ray_pos[1].y -= 0.4f;
    ray_pos[2].y += 0.4f;
    foreach (Vector3 pos in ray_pos) {
      RaycastHit hit_info;
      bool collided = Physics.Raycast(this.player_camera.position, pos-this.player_camera.position, out hit_info, 30);
      if (!collided) {continue;}
      if (hit_info.collider.gameObject == this.enemy_object) {return true;}
    }
    return false;
  }
  
  public void lookAtPlayer() {
    Vector3 look_target = this.player.position;
    look_target.y = this.enemy.position.y; //Do not rotate vertically
    this.enemy.LookAt(look_target);
  }

  public void move(float speed, float gravity = -10) {
    Vector3 motion = this.enemy.forward * speed;
    motion.y = gravity;
    this.enemy_ctrl.Move(motion * Time.deltaTime);  
  }

  public void applyGravity(float gravity = 10) {this.enemy_ctrl.Move(new Vector3(0, -gravity * Time.deltaTime, 0));}

  public void applyInstantGravity(float gravity = 10) {this.enemy_ctrl.Move(new Vector3(0, -gravity, 0));}

  public void teleport(float distance, float teleport_distance) {
    this.enemy_ctrl.Move(this.enemy.forward * (distance-teleport_distance));
    this.enemy_ctrl.Move(new Vector3(0, -100, 0)); //Gravity
    lookAtPlayer();
  }

  public void toggleMesh(bool toggle) {this.enemy_mesh.enabled = toggle;}
  public void toggleController(bool toggle) {this.enemy_ctrl.enabled = toggle;}
  public bool isMeshEnabled() {return this.enemy_mesh.enabled;}
  public bool isControllerEnabled() {return this.enemy_ctrl.enabled;}
  
  //Disables player movement and positions the player to look at the enemy up close
  public void killPlayer(float look_offset = 0.6f) {
    this.pause_script.can_pause = false;
    this.player_script.caught = true;
    Vector3 enemy_pos = this.enemy.position;
    enemy_pos.y = this.player_camera.position.y + look_offset; //Make the camera look slightly up
    this.player_camera.LookAt(enemy_pos);
    lookAtPlayer();
  }
}
