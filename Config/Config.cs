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
  private List<string> settings = new List<string>();
  private List<string> values = new List<string>();
  
  //Read config.txt and parse the options into the "settings" and "values" variables
  public void readFile() {
    string[] config = readConfig();
    if (config == null) {return;} //Todo: handle this error
    foreach (string line in config) {parseLine(line);}
  }

  public int readQualityLevel() {
    string[] quality_levels = {"high", "medium", "low"};
    string quality = readSetting("quality");
    if (quality == null) {return 0;}
    for (int i = 0; i < quality_levels.Length; i++) {
      if (quality.Equals(quality_levels[i])) {return i;}
    }
    return 0;
  }

  public int readInt(string setting, int default_val, int min_val, int max_val) {
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

  public float readFloat(string setting, float default_val, float min_val, float max_val) {
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

  public bool readBool(string setting, bool default_val) {
    string svalue = readSetting(setting);
    if (svalue == null) {return default_val;}
    return svalue.Equals("true") || svalue.Equals("yes");
  }

  public string readSetting(string setting) {
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
    if (!File.Exists("config.txt")) {
      SaveOptions opts = new SaveOptions();
      createConfig(opts);
    }
    
    string file = File.ReadAllText("config.txt");
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
    addLine(line, lines);
    return lines.ToArray();
  }

  int skipComment(string file, int start, int end) {
    for (int i = start; i < end; i++) {
      if (file[i] == '\n') {return i;}
    }
    return end;
  }

  void addLine(StringBuilder line, List<string> lines) {
    if (line.Length == 0) {return;}
    lines.Add(line.ToString().Trim());
  }

  public static void deleteConfig() {File.Delete("config.txt");}

  public static void createConfig(SaveOptions opts) {
    string default_config =
      "# Slender: Insanity Configuration File"
      + "\n\n# From this file you can configure the game's settings"
      + "\n# This file is automatically created by the game and it's affected by the UI settings too"
      + "\n# Options are case-insensitive and incorrect configurations are safely handled with default settings" 
      + "\n# Options that start with \"#\" are disabled, remove the character \"#\" to use them"

      + "\n\n# Sets the audio volume, accepted values range from 0 to 1 (e.g 0.25)"
      + "\nvolume="+opts.volume

      + "\n\n# Sets the mouse sensitivity, affecting camera look speed. Supported values range between 0.1 and 20"
      + "\nsensitivity="+opts.sensitivity

      + "\n\n# Sets the game's window mode, set it to false to play the game in windowed mode"
      + "\nfullscreen="+opts.fullscreen

      + "\n\n# Sets the game's resolution width and height, minimum supported resolution: 640x480"
      + "\n"+opts.getWidthLine()
      + "\n"+opts.getHeightLine()

      + "\n\n# Sets the game's quality level. Supported values: low, medium, high"
      + "\nquality="+opts.quality

      + "\n\n# Sets the game's framerate limit. Supported values range between 0 and 300, set to 0 to disable framerate limit"
      + "\nframerate="+opts.framerate

      + "\n\n# Toggles vsync, set to true to enable"
      + "\nvsync="+opts.vsync

      + "\n\n# When taking screenshots, this sets the upscaling factor. Supported values range between 1 and 5"
      + "\nscreenshot_scale="+opts.screenshot_scale
    ;
    File.WriteAllText("config.txt", default_config);
  }
}

//Simple class, only used to contain multiple values inside, used for creating config files
//Default values here are the games's default settings
public class SaveOptions {
  public float volume = 1;
  public float sensitivity = 2;
  public bool fullscreen = true;
  public int width = 1920;
  public int height = 1080;
  public string quality = "high";
  public int framerate = 60;
  public bool vsync = true;
  public int screenshot_scale = 1;

  //Comments the settings for width and height
  public bool comment_resolution = true;

  public string getWidthLine() {
    string line = "width="+this.width;
    return comment_resolution ? '#'+line : line;
  }

  public string getHeightLine() {
    string line = "height="+this.height;
    return comment_resolution ? '#'+line : line;
  }
}
