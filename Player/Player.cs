using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class Player : MonoBehaviour
{
  public float mouse_sensitivity = 0.5f;
  public int screenshot_scale = 1;
  public bool caught = false;
  public bool paused = false;

  public Transform camera_transform;
  public CharacterController controller;
  public AudioSource footsteps;
  public AudioSource footsteps_running;
  public GameObject pause_menu;
  private float running_tempo = 1.3f; //Currently the running SFX speed is 1.3x of the original

  private float walkSpeed = 3f;
  private float sprintSpeed = 6f;
  private float stamina = 10;
  private float max_stamina = 10;
  private float maxLookAngle = 85f;

  private float verticalRotation = 0f;

  void Awake(){Cursor.lockState = CursorLockMode.Locked;}

  void Update()
  {
    checkPause();
    takeScreenshot();
    if (this.caught || this.paused) {
      this.footsteps.Pause();
      this.footsteps_running.Pause();
      return;
    }
    moveCamera();
    movePlayer();
  }

  //Pause and unpause
  void checkPause() {
    if (this.caught) {return;}
    if (!Keyboard.current.escapeKey.wasPressedThisFrame) {return;}

    if (this.paused) {unpauseGame();}
    else {pauseGame();}
  }

  public void unpauseGame() {
    Cursor.lockState = CursorLockMode.Locked;
    Time.timeScale = 1;
    AudioListener.pause = false;
    this.pause_menu.active = false;
    this.paused = false;
  }

  void pauseGame() {
    Cursor.lockState = CursorLockMode.None;
    Time.timeScale = 0;
    AudioListener.pause = true;
    this.pause_menu.active = true;
    this.paused = true;
  }

  void takeScreenshot() {
    if (!Keyboard.current.pKey.wasPressedThisFrame) {return;}
    long num = 0;
    string filename = "slender-insanity-0.png";
    while (File.Exists(filename)) {
      num+=1;
      filename = "slender-insanity-"+num+".png";
    }
    ScreenCapture.CaptureScreenshot(filename, this.screenshot_scale);
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
    float speed = getWalkSpeed();
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
      float new_time = this.footsteps_running.time * 1.3f;
      if (new_time > this.footsteps.clip.length) {new_time = 0;} //Bug fix for going off-bounds
      this.footsteps.time = new_time;
      if (this.footsteps_running.isPlaying) {return;}
      this.footsteps_running.Play();
      return;
    }
    this.footsteps_running.Pause();
    this.footsteps_running.time = this.footsteps.time / 1.3f;
    if (this.footsteps.isPlaying) {return;}
    this.footsteps.Play();
  }

  float getWalkSpeed() {
    bool is_sprinting = Keyboard.current.leftShiftKey.isPressed;
    if (is_sprinting) {
      if (this.stamina > 0) {
        this.stamina -= 1f * Time.deltaTime;
        return this.sprintSpeed;
      }
      this.stamina = 0;
      return this.walkSpeed;
    }
    else {
      if (this.stamina < this.max_stamina) {this.stamina += 0.5f * Time.deltaTime;}
      else {this.stamina = this.max_stamina;}
      return this.walkSpeed;
    }
  }
}
