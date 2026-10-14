using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Reads the game's settings from the config file and apply them
public class Settings : MonoBehaviour
{
  public Player player;
  public Screenshooter screenshooter;
  public Slenderman slender;
  public SCPGhost[] ghosts;

  public SettingsUI ui; //Optional, for updating the settings menu

  //Read the config file and apply its settings and update the settings UI
  void Awake() {
    Config conf = new Config();
    SaveOptions options = new SaveOptions();
    conf.readFile(); //Read and parse config.txt
    
    Resolution[] available_res = Screen.resolutions;
    Resolution max_res = available_res[available_res.Length-1];

    options.fullscreen = conf.readBool("fullscreen", true);
    options.resolution_scale = (int)conf.readFloat("resolution_scale", 100, 25, 100); //Allow parsing of float numbers for resolution but don't use them
    
    int width = conf.readInt("width", Screen.width, 640, max_res.width);
    int height = conf.readInt("height", Screen.height, 480, max_res.height);
    if (width != Screen.width || height != Screen.height) {
      options.width = width;
      options.height = height;
      options.comment_resolution = false;
    }
    options.screenshot_scale = conf.readInt("screenshot_scale", 1, 1, 5);
    options.setQuality(conf.readQualityLevel());
    options.vsync = conf.readBool("vsync", false);
    options.framerate = conf.readInt("framerate", 60, 0, 300); //Min 10, max 300, disable 0
    options.volume = conf.readFloat("volume", 1, 0, 1);
    options.sensitivity = conf.readFloat("sensitivity", 2f, 0.1f, 20);
    options.setDifficulty(conf.readDifficultyLevel());

    applyConfiguration(options);
    if (this.ui != null) this.ui.updateUI(options);
  }
  
  public void applyConfiguration(SaveOptions options) {
    if (options.comment_resolution) setResolution(options.resolution_scale, options.fullscreen);
    else setResolution(options.width, options.height, options.fullscreen);

    setScreenshotScale(options.screenshot_scale);
    setQuality(options.quality_int);
    setVsync(options.vsync);
    setFramerate(options.framerate);
    setAudioVolume(options.volume);
    setSensitivity(options.sensitivity);
    setDifficulty(options.difficulty_int);
  }
  
  private void setQuality(int level) {QualitySettings.SetQualityLevel(level);}
  private void setAudioVolume(float volume) {AudioListener.volume = volume;}
  private void setSensitivity(float sensitivity) {
    if (this.player == null) return;
    this.player.mouse_sensitivity = sensitivity * 0.1f;
  }
  private void setScreenshotScale(int scale) {
    if (this.screenshooter == null) return;
    this.screenshooter.scale = scale;
  }
  private void setVsync(bool vsync) {QualitySettings.vSyncCount = vsync ? 1 : 0;}
  private void setFramerate(int fps) {Application.targetFrameRate = fps;}
  private void setResolution(int scale, bool fullscreen) {
    Resolution[] resolutions = Screen.resolutions;
    Resolution max_res = resolutions[resolutions.Length-1]; //Native screen resolution, the maximum supported one
    float scalef = (float)scale/100; //25-100 converted to 0.25-1

    int width = (int)(max_res.width * scalef);
    int height = (int)(max_res.height * scalef);
    Screen.SetResolution(width, height, fullscreen);
  }
  private void setResolution(int width, int height, bool fullscreen) {Screen.SetResolution(width, height, fullscreen);}

  private void setDifficulty(int value) {
    bool easy = value == 0;
    bool hard = value == 2;
    if (this.slender != null) {
      if (easy) this.slender.setEasyMode();
      else if (hard) this.slender.setHardMode();
    }

    if (this.ghosts == null) return;
    foreach (SCPGhost ghost in this.ghosts) {
      if (easy) ghost.setEasyMode();
      else if (hard) ghost.setHardMode();
    }
  }
}
