using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Reads the game's settings from the config file (Config class)
//The settings UI calls functions from here to configure the game as well
public class SettingsManager : MonoBehaviour
{
  public Player player;
  public Screenshooter screenshooter;
  public Config config;

  //Settings UI elements
  public Slider fps;
  public Slider sensitivity;
  public Slider volume;
  public Slider resolution;
  public SliderDisplay resolution_display;
  public Slider screenshot_scale;
  public TMP_Dropdown quality;
  public Toggle vsync;
  public Toggle fullscreen;
  
  //Set the game options on startup based on the config file
  //Todo: implement camera antialiasing
  void Awake() {
    config.readFile(); //Read and parse config.txt
    Resolution[] available_res = Screen.resolutions;
    Resolution max_res = available_res[available_res.Length-1];

    
    bool fullscreen_mode = config.readBool("fullscreen", true);
    int res_scale = (int)config.readFloat("resolution_scale", 100, 25, 100); //Allow parsing of float numbers for resolution but don't use them
    setResolution(res_scale, fullscreen_mode);
    this.resolution.value = res_scale;
    
    int width = config.readInt("width", Screen.width, 640, max_res.width);
    int height = config.readInt("height", Screen.height, 480, max_res.height);
    if (width != Screen.width || height != Screen.height) {
      this.resolution.interactable = false;
      this.resolution_display.disable();
      setResolution(width, height, fullscreen_mode);
    }
    int screenshot_scale = config.readInt("screenshot_scale", 1, 1, 5);
    setScreenshotScale(screenshot_scale);
    this.screenshot_scale.value = screenshot_scale;

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
    setSensitivity(this.sensitivity.value);
    setAudioVolume(this.volume.value);
    setQuality(this.quality.value); //Order of quality levels in dropdown must match the order in config and QualitySettings
    //Option is disabled when width and height settings in config file are used
    if (this.resolution.interactable) setResolution((int)this.resolution.value, this.fullscreen.isOn);
    
    SaveOptions opts = new SaveOptions(); //Part of Config.cs
    string[] quality_levels = {"high", "medium", "low"}; //Duplicate code in Config.cs

    //Save new config.txt file
    opts.framerate = (int)this.fps.value;
    opts.vsync = this.vsync.isOn;
    opts.sensitivity = this.sensitivity.value;
    opts.volume = this.volume.value;
    opts.quality = quality_levels[this.quality.value];
    opts.fullscreen = this.fullscreen.isOn;
    opts.resolution_scale = (int)this.resolution.value;
    opts.width = Screen.width;
    opts.height = Screen.height;
    opts.comment_resolution = this.resolution.interactable;
    opts.screenshot_scale = (int)this.screenshot_scale.value;
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
    if (this.screenshooter == null) return;
    this.screenshooter.scale = scale;
  }
  void setVsync(bool vsync) {QualitySettings.vSyncCount = vsync ? 1 : 0;}
  void setFramerate(int fps) {Application.targetFrameRate = fps;}
  void setResolution(int scale, bool fullscreen) {
    Resolution[] resolutions = Screen.resolutions;
    Resolution max_res = resolutions[resolutions.Length-1]; //Native screen resolution, the maximum supported one
    float scalef = (float)scale/100; //25-100 converted to 0.25-1

    int width = (int)(max_res.width * scalef);
    int height = (int)(max_res.height * scalef);
    Screen.SetResolution(width, height, fullscreen);
  }
  void setResolution(int width, int height, bool fullscreen) {Screen.SetResolution(width, height, fullscreen);}
}
