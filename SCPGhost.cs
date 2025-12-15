using UnityEngine;

//Source code for the level 2 enemy behavior, a ghost inspired by SCP-087-1
public class SCPGhost : MonoBehaviour
{
  public Transform player;
  public Transform player_camera;
  public CharacterController controller;
  public MeshRenderer mesh;
  public Material material;

  float teleport_distance;
  float speed = 1;
  
  void Awake() {this.material.color = new Color32(109, 109, 109, 255);}

  void Update() {
    lookAtPlayer();
    if (this.mesh.isVisible) {return;}
    Vector3 motion = this.transform.forward * this.speed;
    motion.y = -10; //Gravity
    this.controller.Move(motion * Time.deltaTime);        
  }

  //Duplicate code from Slenderman.cs
  //Todo: make separate file containing look, kill, move and teleport logic, to share between different enemies
  void lookAtPlayer() {
    Vector3 look_target = this.player.position;
    look_target.y = this.transform.position.y;
    this.transform.LookAt(look_target);
  }

  void teleport(float distance) {
    this.controller.Move(this.transform.forward * (distance-this.teleport_distance));
    this.controller.Move(new Vector3(0, -100, 0)); //Gravity
    lookAtPlayer();
  }
}
