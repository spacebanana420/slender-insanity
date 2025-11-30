using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
  public float mouseSensitivity = 0.5f;
  public bool caught = false;
  public bool paused = false;

  public Transform camera_transform;
  public CharacterController controller;

  private float walkSpeed = 3f;
  private float sprintSpeed = 6f;
  private float stamina = 10;
  private float max_stamina = 10;
  private float maxLookAngle = 85f;

  private float verticalRotation = 0f;

  void Awake(){
    setFramerate(120, false);
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Update()
  {
    if (caught || paused) return;
    moveCamera();
    movePlayer();
  }

  void moveCamera() {
    Vector2 mouseDelta = Mouse.current.delta.ReadValue() * this.mouseSensitivity;
    this.transform.Rotate(Vector3.up * mouseDelta.x);
    this.verticalRotation -= mouseDelta.y;
    this.verticalRotation = Mathf.Clamp(this.verticalRotation, -this.maxLookAngle, this.maxLookAngle);
    this.camera_transform.localRotation = Quaternion.Euler(this.verticalRotation, 0f, 0f);
  }

  void movePlayer() {
    Vector2 moveInput = new Vector2(
      (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f),
      (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f)
    ).normalized;
    Vector3 motion = this.transform.right * moveInput.x + this.transform.forward * moveInput.y;

    bool is_sprinting = Keyboard.current.leftShiftKey.isPressed;
    float speed = getWalkSpeed();
    this.controller.Move(motion * speed * Time.deltaTime);
    this.controller.Move(new Vector3(0, -5 * Time.deltaTime, 0)); //Gravity
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
  
  void setFramerate(int fps, bool vsync) {
    if (vsync) {
      QualitySettings.vSyncCount = 1;
      return;
    }
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = fps;
  }
}
