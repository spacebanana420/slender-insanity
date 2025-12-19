using UnityEngine;

//Very unfinished, generic API for in-game sprites and portraits
public class SpriteAPI : MonoBehaviour
{
  public Transform player;
  public MeshRenderer mesh;
  
  private Color original_color;
  private Material material;
  
  void Awake() {
    this.original_color = this.mesh.material.color;
    this.material = this.mesh.material;
  }
  
  public float getDistance() {return Vector3.Distance(this.transform.position, this.player.position);}

  //Whether the enemy is inside the player's field of view, even if hidden behind an object
  public bool isLookedAt() {return this.mesh.isVisible;}
  
  public void lookAtPlayer() {
    Vector3 look_target = this.player.position;
    look_target.y = this.transform.position.y; //Do not rotate vertically
    this.transform.LookAt(look_target);
  }

  public Color getColor() {return this.material.color;}
  public float getAlpha() {return this.material.color.a;}
  
  public void setColor(Color c) {this.material.color = c;}
  public void setColor32(Color32 c) {this.material.color = c;}
  public void setAlpha(float alpha) {
    Color c = this.material.color;
    c.a = alpha;
    this.material.color = c;
  }
}
