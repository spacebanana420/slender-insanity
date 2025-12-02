using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

//Config reading class
//Reads the game's settings file and parses the settings written there
//Allows the player to configure the game directly from a text file
public class Config : MonoBehaviour
{ 
  //Todo later, have separate setting variables for floats and ints
  private List<string> settings = new List<string>();
  private List<string> values = new List<string>();
  private string config_path = "config.txt";
  
  //Read the config and set the quality options with safe defaults in case of incorrect configuration
  void Start() {
    string[] config = readConfig();
    if (config == null) {return;}
    foreach (string line in config) {parseLine(line);}
    int quality_level = readInt("quality", 2, 0, 2);
    
    Resolution[] available_res = Screen.resolutions;
    Resolution max_res = available_res[available_res.Length-1];
    int width = readInt("width", Screen.width, 640, max_res.width);
    int height = readInt("height", Screen.height, 480, max_res.height);
    bool fullscreen_mode = readBool("fullscreen", true);

    int frame_rate = readInt("framerate", 60, 0, 501); //Min 10, max 500, disable 0
    if (frame_rate < 10 && frame_rate > 0) {frame_rate = 10;}
    if (frame_rate == 501) {frame_rate = 0;} //Unlock it instead
    
    bool vsync_mode = readBool("vsync", false);
    float audio_volume = readFloat("volume", 0.5f, 0, 1);

    Screen.SetResolution(width, height, fullscreen_mode);
    if (vsync_mode) {QualitySettings.vSyncCount = 1;}
    else {
      Application.targetFrameRate = frame_rate;
      QualitySettings.vSyncCount = 0;
    }
    QualitySettings.SetQualityLevel(quality_level);
    AudioListener.volume = audio_volume;
  }

  int readInt(string setting, int default_val, int min_val, int max_val) {
    string svalue = readSetting(setting);
    try {
      int ival = int.Parse(svalue);
      if (ival < min_val) {return min_val;}
      if (ival > max_val) {return max_val;}
      return ival;
    }
    catch (FormatException) {return default_val;}  
    catch (ArgumentNullException) {return default_val;}
    catch (OverflowException) {return default_val;}
  }

  float readFloat(string setting, float default_val, float min_val, float max_val) {
    string svalue = readSetting(setting);
    try {
      float fval = float.Parse(svalue);
      if (fval < min_val) {return min_val;}
      if (fval > max_val) {return max_val;}
      return fval;
    }
    catch (FormatException) {return default_val;}  
    catch (ArgumentNullException) {return default_val;}
    catch (OverflowException) {return default_val;}
  }

  bool readBool(string setting, bool default_val) {
    string svalue = readSetting(setting);
    if (svalue == null) {return default_val;}
    return svalue.Equals("true") || svalue.Equals("yes");
  }

  string readSetting(string setting) {
    int value_i = -1;
    for (int i = 0; i < this.settings.Count; i++) {
      if (this.settings[i].Equals(setting)) {value_i = i; break;}
    }
    if (value_i == -1) {return null;}
    return this.values[value_i];
  }
  
  //Settings are structured in type NAME=VALUE (e.g framerate=120)
  //Spaces before and/or after = are accepted
  void parseLine(string line) {
    var setting = new StringBuilder();
    var svalue = new StringBuilder();
    int value_start = -1; //Index after the = character

    //Get the setting name
    for (int i = 0; i < line.Length; i++) {
      char c = line[i];
      if (c == '=') {value_start = i+1; break;}
      setting.Append(c);
    }
    if (value_start == -1 || setting.Length == 0) {return;}

    //Get the assigned value to that setting
    for (int i = value_start; i < line.Length; i++) {svalue.Append(line[i]);}
    if (svalue.Length == 0) {return;}

    settings.Add(setting.ToString().Trim().ToLower());
    values.Add(svalue.ToString().Trim().ToLower());
  }
  
  //Read the config file into an array composed of each line
  string[] readConfig() {
    if (!File.Exists(this.config_path)) {
      createDefaultConfig();
      return null;
    }
    
    string file = File.ReadAllText(this.config_path);
    List<string> lines = new List<string>();
    if (file == null) {return null;}
    StringBuilder line = new StringBuilder();
    
    //Separate the string representing the text file into multiple lines, ignore comments
    for (int i = 0; i < file.Length; i++) {
      char c = file[i];
      if (c == '\n') { //End of line
        addLine(line, lines);
        line = new StringBuilder();
        continue;
      }
      if (c == '#') { //Earlier end of line due to comment
        addLine(line, lines);
        line = new StringBuilder();
        i = skipComment(file, i, file.Length);
        continue;
      }
      line.Append(c);
    }
    return lines.ToArray();
  }

  int skipComment(string file, int start, int end) {
    for (int i = start; i < end; i++) {
      if (file[i] == '\n') {return i+1;}
    }
    return end;
  }

  void addLine(StringBuilder line, List<string> lines) {
    if (line.Length == 0) {return;}
    lines.Add(line.ToString().Trim());
  }

  void createDefaultConfig() {
    string default_config =
      "# Slender: Insanity Configuration File"
      + "\n# From this file you can configure the game's graphics and audio volume"
      + "\n# Options are case-insensitive and incorrect configurations are safely handled with default settings" 
      + "\n# Options that start with \"#\" are commented and will not be taken in consideration, remove the character to use them"

      + "\n\n# Sets the audio volume, accepted values range from 0 to 1 (e.g 0.25)"
      + "\nvolume=0.3"

      + "\n\n# Sets the game's window mode, set it to false to play the game in windowed mode"
      + "\nfullscreen=true"

      + "\n\n# Sets the game's resolution width and height, minimum supported resolution: 640x480"
      + "#width=1920"
      + "#height=1080"

      + "\n\n# Sets the game's framerate, supported values range between 10 and 500, set to 0 to disable framerate limit"
      + "\nframerate=60"

      + "\n\n# Toggles vsync, overrides framerate setting, set to true to enable"
      + "\nvsync=false"
    ;
    File.WriteAllText(this.config_path, default_config);
  }
}
