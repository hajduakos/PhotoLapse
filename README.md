# PhotoLapse

Create single frame timelapses from photos taken in a long interval of time.

![Sample image](https://raw.githubusercontent.com/hajduakos/PhotoLapse/master/Other/sample.jpg)

## How to use it?

### GUI application
* Build the project *PhotoLapse* (and also the referenced *PhotoLapseTools* project).
* Run the application *PhotoLapse.exe*.
* Click the Load button to load the photos.
* Select the type of timelapse (gradient or stripes) and click render.
* Export the image by clicking save.

### Console application
* Build the project *PhotoLapseConsole* (and also the referenced *PhotoLapseTools* project).
* Drag and drop the images on *PhotoLapseConsole.exe*.
* Enter the type of timelapse ('g' or 's'). The rendered image is placed next to the source images.

## Requirements
* GUI application requires .NET Framework 4.5
* Console application requires .NET Framework 3.5

## Notes
The source images must be aligned perfectly for a nice result. Take the photos from a tripod, or align them with a software.