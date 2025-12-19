using UnityEngine;

//Very unfinished, generic API for in-game sprites and portraits
public class SpriteAPI : MonoBehaviour
{
  public Transform player;
  public MeshRenderer mesh;
  public Material material;
  
  public float getDistance() {return Vector3.Distance(this.transform.position, this.player.position);}

  //Whether the enemy is inside the player's field of view, even if hidden behind an object
  public bool isLookedAt() {return this.mesh.isVisible;}
  
  public void lookAtPlayer() {
    Vector3 look_target = this.player.position;
    look_target.y = this.transform.position.y; //Do not rotate vertically
    this.transform.LookAt(look_target);
  }
}
