using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
  public TMP_Dropdown quality;
  public Toggle vsync;
  public Toggle fullscreen;

  //Setting stored to be later saved in config overrides by UI
  private int screenshot_scale;
  
  //Set the game options on startup based on the config file
  //Todo: implement camera antialiasing
  void Awake() {
    config.readFile(); //Read and parse config.txt
    Resolution[] available_res = Screen.resolutions;
    Resolution max_res = available_res[available_res.Length-1];
    int width = config.readInt("width", Screen.width, 640, max_res.width);
    int height = config.readInt("height", Screen.height, 480, max_res.height);
    bool fullscreen_mode = config.readBool("fullscreen", true);
    Screen.SetResolution(width, height, fullscreen_mode);

    this.screenshot_scale = config.readInt("screenshot_scale", 1, 1, 5);
    setScreenshotScale(this.screenshot_scale);

    //Apply settings and add them to the settings UI as well
    int quality = config.readQualityLevel();
    setQuality(quality);
    this.quality.value = quality;
    
    bool vsync = config.readBool("vsync", false);
    setVsync(vsync);
    this.vsync.isOn = vsync;

    int fps = config.readInt("framerate", 60, 0, 300); //Min 10, max 300, disable 0
    setFramerate(fps);
    this.fps.value = fps;

    float volume = config.readFloat("volume", 1, 0, 1);
    setAudioVolume(volume);
    this.volume.value = volume;

    float sensitivity = config.readFloat("sensitivity", 2f, 0.1f, 20);
    setSensitivity(sensitivity);
    this.sensitivity.value = sensitivity;
  }

  //Called by the settings UI
  public void setUIOptions() {
    setFramerate((int)this.fps.value);
    setVsync(this.vsync.isOn);
    setFullscreen(this.fullscreen.isOn);
    setSensitivity(this.sensitivity.value);
    setAudioVolume(this.volume.value);
    setQuality(this.quality.value); //Order of quality levels in dropdown must match the order in config and QualitySettings

    SaveOptions opts = new SaveOptions(); //Part of Config.cs
    string[] quality_levels = {"high", "medium", "low"}; //Duplicate code in Config.cs

    //Save new config.txt file
    opts.framerate = (int)this.fps.value;
    opts.vsync = this.vsync.isOn;
    opts.fullscreen = this.fullscreen.isOn;
    opts.sensitivity = this.sensitivity.value;
    opts.volume = this.volume.value;
    opts.quality = quality_levels[this.quality.value];
    opts.width = Screen.width;
    opts.height = Screen.height;
    opts.comment_resolution = false;
    opts.screenshot_scale = this.screenshot_scale;
    Config.deleteConfig();
    Config.createConfig(opts);
  }

  //Configure the game effectively
  void setQuality(int level) {QualitySettings.SetQualityLevel(level);}
  void setAudioVolume(float volume) {AudioListener.volume = volume;}
  void setSensitivity(float sensitivity) {
    if (this.player == null) return;
    this.player.mouse_sensitivity = sensitivity * 0.1f;
  }
  void setScreenshotScale(int scale) {
    if (this.player == null) return;
    this.player.screenshot_scale = scale;
  }
  void setFullscreen(bool fullscreen) {Screen.SetResolution(Screen.width, Screen.height, fullscreen);}
  void setVsync(bool vsync) {QualitySettings.vSyncCount = vsync ? 1 : 0;}
  void setFramerate(int fps) {Application.targetFrameRate = fps;}
}
