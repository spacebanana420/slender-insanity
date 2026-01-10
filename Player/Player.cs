using UnityEngine;
using UnityEngine.InputSystem;

//Player's base code file
//Controls mouse and keyboard movement and pausing the game
public class Player : MonoBehaviour
{
  public float mouse_sensitivity = 0.5f;
  public bool caught = false; //Freeze player controls, change the variable name later to a more appropriate one

  public Transform camera_transform;
  public CharacterController controller;
  public AudioSource footsteps;
  public AudioSource footsteps_running;
  
  private float running_tempo = 1.3f; //Currently the running SFX speed is 1.3x of the original
  private float walkSpeed = 3f;
  private float sprintSpeed = 6f;
  private float stamina = 12;
  private float max_stamina = 12;
  private float maxLookAngle = 85f;

  private float verticalRotation = 0f;

  void Awake() {Cursor.lockState = CursorLockMode.Locked;}

  void Update() {
    if (this.caught) {
      this.footsteps.Pause();
      this.footsteps_running.Pause();
      return;
    }
    moveCamera();
    movePlayer();
  }

  void moveCamera() {
    Vector2 mouseDelta = Mouse.current.delta.ReadValue() * this.mouse_sensitivity;
    this.transform.Rotate(Vector3.up * mouseDelta.x);
    this.verticalRotation -= mouseDelta.y;
    this.verticalRotation = Mathf.Clamp(this.verticalRotation, -this.maxLookAngle, this.maxLookAngle);
    this.camera_transform.localRotation = Quaternion.Euler(this.verticalRotation, 0f, 0f);
  }

  void movePlayer() {
    bool forward = Keyboard.current.wKey.isPressed;
    bool backward = Keyboard.current.sKey.isPressed;
    bool left = Keyboard.current.aKey.isPressed;
    bool right = Keyboard.current.dKey.isPressed;
    bool moving = forward || backward || left || right;
    
    Vector2 moveInput = new Vector2(
      (right ? 1f : 0f) - (left ? 1f : 0f),
      (forward ? 1f : 0f) - (backward ? 1f : 0f)
    ).normalized;
    Vector3 motion = this.transform.right * moveInput.x + this.transform.forward * moveInput.y;

    bool is_sprinting = Keyboard.current.leftShiftKey.isPressed;
    float speed = getWalkSpeed(moving);
    motion.y = -5; //Gravity
    this.controller.Move(motion * speed * Time.deltaTime);

    playFootsteps(moving, is_sprinting && this.stamina > 0);
  }

  //Whole audio file contains the footstep sounds, instead of relying on one file per footstep
  //More convenient but less flexible
  void playFootsteps(bool moving, bool running) {
    if (!moving) {
      this.footsteps.Pause();
      this.footsteps_running.Pause();
      return;
    }
    if (running) {
      this.footsteps.Pause();
      this.footsteps.time = getClipTime(false);
      if (!this.footsteps_running.isPlaying) this.footsteps_running.Play();
      return;
    }
    this.footsteps_running.Pause();
    this.footsteps_running.time = getClipTime(true);
    if (!this.footsteps.isPlaying) this.footsteps.Play();
  }

  //Walking and running footstep audio files differ in their speed
  //When switching between clips, they should play at roughly the same point in the audio file, but without overflowing
  float getClipTime(bool run) {
    float new_time;
    if (run) {
      new_time = this.footsteps.time / 1.3f;
      if (new_time > this.footsteps_running.clip.length) {new_time = 0;}
    }
    else {
      new_time = this.footsteps_running.time * 1.3f;
      if (new_time > this.footsteps.clip.length) {new_time = 0;}
    }
    return new_time;
  }

  float getWalkSpeed(bool is_moving) {
    bool is_sprinting = Keyboard.current.leftShiftKey.isPressed;
    if (is_sprinting) {
      if (this.stamina > 0) { //Run
        this.stamina -= 1f * Time.deltaTime;
        return this.sprintSpeed;
      }
      this.stamina = 0; //Depleted, walk
      return this.walkSpeed;
    }
    else { //Walk and regenerate stamina
      float regenerate_speed = is_moving ? 0.6f : 0.75f;
      if (this.stamina < this.max_stamina) this.stamina += regenerate_speed * Time.deltaTime;
      else this.stamina = this.max_stamina;
      return this.walkSpeed;
    }
  }
}
