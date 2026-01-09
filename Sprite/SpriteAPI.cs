using UnityEngine;

//Library code for functions which are useful for 2D sprites inside a 3D scene
public class SpriteAPI : MonoBehaviour
{
  public Transform player;
  public MeshRenderer mesh;
  public bool movesInScene = false;
  
  private Transform sprite_t;
  private CharacterController sprite_ctrl;
  
  private Color original_color;
  private Material material;
  
  void Awake() {
    this.original_color = this.mesh.material.color;
    this.material = this.mesh.material;
    
    if (!this.movesInScene) return;
    this.sprite_t = this.gameObject.GetComponent<Transform>();
    this.sprite_ctrl = this.gameObject.GetComponent<CharacterController>();
  }
  
  public float getDistance() {return Vector3.Distance(this.transform.position, this.player.position);}

  //Whether the enemy is inside the player's field of view, even if hidden behind an object
  public bool isLookedAt() {return this.mesh.isVisible;}
  
  public void lookAtPlayer() {
    Vector3 look_target = this.player.position;
    look_target.y = this.transform.position.y; //Do not rotate vertically
    this.transform.LookAt(look_target);
  }

  public void toggleMesh(bool toggle) {this.mesh.enabled = toggle;}
  
  public Color getColor() {return this.material.color;}
  public float getAlpha() {return this.material.color.a;}
  
  public void setColor(Color c) {this.material.color = c;}
  public void setColor32(Color32 c) {this.material.color = c;}
  public void setAlpha(float alpha) {
    Color c = this.material.color;
    c.a = alpha;
    this.material.color = c;
  }

  //Motion-based functions, requires movesInScene set to true and requires a Transform and CharacterController components
  public void move(float speed, float gravity = -10) {    
    Vector3 motion = this.sprite_t.forward * speed;
    motion.y = gravity;
    this.sprite_ctrl.Move(motion * Time.deltaTime);
  }

  public void teleportToPlayer(float distance, bool forward, Terrain terrain = null, float height_correction = 1.5f) {
    Vector3 direction = this.player.forward * distance;
    if (!forward) direction *= -1;
    
    Vector3 new_position = this.player.position + direction;
    if (terrain == null) new_position.y += height_correction;
    else new_position.y = terrain.SampleHeight(new_position)+height_correction;

    this.sprite_ctrl.enabled = false; //For manual position changes
    this.sprite_t.position = new_position;
    this.sprite_ctrl.enabled = true;
    this.sprite_ctrl.Move(new Vector3(0, -100, 0)); //Gravity
    lookAtPlayer();
  }
}
