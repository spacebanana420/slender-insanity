using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
  public Slider fps;
  public Slider sensitivity;
  public Slider volume;
  public Slider resolution;
  public SliderDisplay resolution_display;
  public Slider screenshot_scale;
  public TMP_Dropdown quality;
  public Toggle vsync;
  public Toggle fullscreen;
  public TMP_Dropdown difficulty; //Only present in main menu

  public Settings settings;
  
  public void updateUI(SaveOptions options) {
    this.resolution.value = options.resolution_scale;
    if (!options.comment_resolution) {
      this.resolution.interactable = false;
      this.resolution_display.disable();
    }
    this.screenshot_scale.value = options.screenshot_scale;
    this.quality.value = options.quality_int;
    this.vsync.isOn = options.vsync;
    this.fps.value = options.framerate;
    this.volume.value = options.volume;
    this.sensitivity.value = options.sensitivity;
    if (this.difficulty != null) this.difficulty.value = options.difficulty_int;
  }

  //Called by the settings UI
  public void setUIOptions() {
    var opts = new SaveOptions();
    opts.framerate = (int)this.fps.value;
    opts.vsync = this.vsync.isOn;
    opts.sensitivity = this.sensitivity.value;
    opts.volume = this.volume.value;
    opts.setQuality(this.quality.value);
    opts.fullscreen = this.fullscreen.isOn;
    opts.resolution_scale = (int)this.resolution.value;
    opts.width = Screen.width;
    opts.height = Screen.height;
    opts.comment_resolution = this.resolution.interactable;
    opts.screenshot_scale = (int)this.screenshot_scale.value;
    if (this.difficulty != null) opts.setDifficulty(this.difficulty.value);

    this.settings.applyConfiguration(opts);
    Config.deleteConfig();
    Config.createConfig(opts);
  }
}
