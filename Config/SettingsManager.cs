using UnityEngine;
using UnityEngine.UI;

//Reads the game's settings from the config file (Config class)
//The settings UI calls functions from here to configure the game as well
public class SettingsManager : MonoBehaviour
{ 
  public Player player;
  public Config config;

  //Settings UI elements
  public Slider fps;
  public Slider sensitivity;
  public Slider volume;
  public TMPro.TMP_Dropdown quality;
  public Toggle vsync;
  public Toggle fullscreen;
  
  //Set the game options on startup based on the config file
  //Todo: implement camera antialiasing
  void Start() {
    config.readFile(); //Read and parse config.txt
    Resolution[] available_res = Screen.resolutions;
    Resolution max_res = available_res[available_res.Length-1];
    int width = config.readInt("width", Screen.width, 640, max_res.width);
    int height = config.readInt("height", Screen.height, 480, max_res.height);
    bool fullscreen_mode = config.readBool("fullscreen", true);
    Screen.SetResolution(width, height, fullscreen_mode);
    
    setQuality(config.readQualityLevel());
    setVsync(config.readBool("vsync", false));
    setFramerate(config.readInt("framerate", 60, 0, 501)); //Min 10, max 500, disable 0
    setAudioVolume(config.readFloat("volume", 1, 0, 1));
    setSensitivity(config.readFloat("sensitivity", 2f, 0.1f, 20));
    setScreenshotScale(config.readInt("screenshot_scale", 1, 1, 5));
  }

  //Called by the settings UI
  public void setUIOptions() {
    setFramerate((int)this.fps.value);
    setVsync(this.vsync.isOn);
    setFullscreen(this.fullscreen.isOn);
    setSensitivity(this.sensitivity.value);
    setAudioVolume(this.volume.value);
    setQuality(this.quality.value); //Order of quality levels in dropdown must match the order in config and QualitySettings
  }

  //Configure the game effectively
  void setQuality(int level) {QualitySettings.SetQualityLevel(level);}
  void setAudioVolume(float volume) {AudioListener.volume = volume;}
  void setSensitivity(float sensitivity) {this.player.mouse_sensitivity = sensitivity * 0.2f;}
  void setScreenshotScale(int scale) {this.player.screenshot_scale = scale;}
  void setFullscreen(bool fullscreen) {Screen.SetResolution(Screen.width, Screen.height, fullscreen);}
  void setVsync(bool vsync) {QualitySettings.vSyncCount = vsync ? 1 : 0;}
  void setFramerate(int fps) {
    if (fps == 501) {fps = 0;} //Unlock it instead
    Application.targetFrameRate = fps;
  }
}
