using UnityEngine;

//Reads the game's settings from the config file (Config class)
//The settings UI calls functions from here to configure the game as well
public class SettingsManager : MonoBehaviour
{ 
  public Player player;
  public Config config;
  //todo implement camera antialiasing
  
  //Set the game options on startup based on the config file
  void Start() { 
    Resolution[] available_res = Screen.resolutions;
    Resolution max_res = available_res[available_res.Length-1];
    int width = config.readInt("width", Screen.width, 640, max_res.width);
    int height = config.readInt("height", Screen.height, 480, max_res.height);
    bool fullscreen_mode = config.readBool("fullscreen", true);
    Screen.SetResolution(width, height, fullscreen_mode);

    int frame_rate = config.readInt("framerate", 60, 0, 501); //Min 10, max 500, disable 0
    if (frame_rate < 10 && frame_rate > 0) {frame_rate = 10;}
    else if (frame_rate == 501) {frame_rate = 0;} //Unlock it instead
    
    setQuality(config.readQualityLevel());
    setVsync(config.readBool("vsync", false));
    setFramerate(frame_rate);
    setAudioVolume(config.readFloat("volume", 1, 0, 1));
    setSensitivity(config.readFloat("sensitivity", 2f, 0.1f, 20));
    setScreenshotScale(config.readInt("screenshot_scale", 1, 1, 5));
  }

  //Set functions are used both here and by the settings UI
  //If a setting does not have a set function it means it's not available from the UI, only from config.txt
  public void setQuality(int level) {QualitySettings.SetQualityLevel(level);}
  public void setAudioVolume(float volume) {AudioListener.volume = volume;}
  public void setSensitivity(float sensitivity) {this.player.mouse_sensitivity = sensitivity * 0.2f;}
  public void setScreenshotScale(int scale) {this.player.screenshot_scale = scale;}
  public void setFullscreen(bool fullscreen) {Screen.SetResolution(Screen.width, Screen.height, fullscreen);}
  public void setVsync(bool vsync) {QualitySettings.vSyncCount = vsync ? 1 : 0;}
  public void setFramerate(int fps) {Application.targetFrameRate = fps;}
}
