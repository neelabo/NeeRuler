## How to operate

Drag with the mouse to the line of text you want to read.  

|Action|Description|
|----|----
|Left button|Move on to the next line
|Right button|Context menu
|Mouse wheel|Increase or decrease the margin above the text line

Check `KeyConfig.json` in the profile folder for shortcut keys.

## About Auto Stride

Automatically detects text lines as you move panels.

The text line detection mechanism is a simple detection method that scans pixel lines and considers those with a large increase or decrease in brightness to be text.  
Due to the way it works, if the panel is opaque, it will not function.
In addition, if a picture or other element is included in addition to the text, it will not be judged correctly.

## Settings

Configuration is done by editing the JSON file directly.  
You will find the configuration files in the context menu "Open Profiles Folder". Usually, this is the `Profiles` folder.

|File name|Description|
|--|-----
|Settings.json  |Settings
|KeyConfig.json |Shortcut key setting
|Profile-*.json |Profile

### Settings.json

Configuration file.  
It is loaded when the application starts and the status is saved to this file when it exits.

|Group|Name|Type|Description|
|-|--|-|-----
||StorageLocationX|Number|X-coordinate of storage location
||StorageLocationY|Number|Y-coordinate of storage location
||Stride|Number|Basic movement amount (pixel)
||AdjustStride|Number|Adjustment amount (pixel)
||IsAutoStride|Boolean|Enable auto stride
|AutoStride|MinTextHeight|Number|Parameters for auto stride. Minimum text line height (pixel).Used to prevent recognition of ruby, etc.
|AutoStride|TextLineComplexityThreshold|Number|Parameters for auto stride. Value indicating the complexity of the line considered to be a text line (0.0-1.0)
|Layout|IsVertical|Boolean|Place panels vertically
|Layout|IsFlatPanel|Boolean|Make the background color the same as the color of the text lines to create a flat panel
|Layout|Width|Number|Panel width (pixel). Value when placed horizontally
|Layout|Height|Number|Panel height (pixel). Value when placed horizontally
|Layout|TextLineHeight|Number|Text line height (pixel)
|Layout|TextLineTopMargin|Number|Margin above text line (pixel)
|Layout|TextLineBottomMargin|Number|Margin below text line (pixel)
|Layout|BaseLine|Number|Baseline position (pixel). The length from the top of the panel to the baseline
|Layout|BaseLineHeight|Number|Baseline height (pixel)
|Layout|BackgroundColor|Color|Background color
|Layout|TextLineColor|Color|Text line color
|Layout|BaseLineColor|Color|Baseline color

### KeyConfig.json

This file defines shortcut keys for commands.  
See [.NET Key Enum](https://learn.microsoft.com/dotnet/api/system.windows.input.key) for available key names.
Can be modified with `Alt` `Shift` `Ctrl`.

|Command|Default|Description|
|--|--|-----
|StoreLocation|`Shift+Q`|Memorize location
|RestoreLocation|`Q`|Restore location
|ToggleIsVertical|`V`|Switching between vertical and horizontal orientation
|ToggleIsAutoStride|`E`|Auto stride switching
|ToggleIsFlatPanel|`F`|Flat panel switching
|MoveUp|`W` `Up`|Move up
|MoveDown|`S` `Down`|Move down
|MoveLeft|`A` `Left`|Move left
|MoveRight|`D` `Right`|Move right
|AdjustUp|`Shift+W` `Shift+Up`|Adjust up
|AdjustDown|`Shift+S` `Shift+Down`|Adjust down
|AdjustLeft|`Shift+A` `Shift+Left`|Adjust left
|AdjustRight|`Shift+D` `Shift+Right`|Adjust right
|Exit||Exit application
|OpenProfilesFolder||Open profiles folder
|Profile-1|`1`|Apply `Profile-1.json`
|Profile-2|`2`|Apply `Profile-2.json`
|Profile-3|`3`|Apply `Profile-2.json`
|Profile-*||Apply `Profile-*.json`. `*` depends on the filename

### Profile-*.json

Definition file for switching some of the settings.
Multiple profiles can be held, where `*` is an arbitrary string.  
The format is the same as `Settings.json`.
Omitted properties remain unchanged in their current state.
