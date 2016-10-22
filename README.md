# PhotoLapse

Timelapse videos are a popular way to display the passing of time. Such timelapses can also be composed to a into a single photo. Using PhotoLapse, you can create single frame timelapses (also known as HDTR images) from photos taken in a long interval of time.

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
* Run *PhotoLapseConsole.exe* with the following arguments:
  * `-t`: type: `stripes` or `gradient`,
  * `-i`: list of input files separated by space,
  * `-o`: output file name,
  * `-w`: (optional) weights.
    * In stripes mode each image has a weight.
    * In gradient mode each transition between two images has a weight, so there are one less weights than images.
* Example: `PhotoLapseConsole.exe -t stripes -i img1.jpg img2.jpg img3.jpg -o out.jpg -w 1 2 3`.

## Requirements
* GUI application requires .NET Framework 4.5
* Console application requires .NET Framework 3.5

## Notes
The source images must be aligned perfectly for a nice result. Take the photos from a tripod, or align them with a software.

## Samples
<p align="center">
<a href="https://www.flickr.com/photos/sonic182/15690712193"><img height="90" src="https://farm9.staticflickr.com/8586/15690712193_ea954f8999_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/15798165450"><img height="90" src="https://farm8.staticflickr.com/7528/15798165450_dd29beb2ac_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/15689060644"><img height="90" src="https://farm8.staticflickr.com/7559/15689060644_d8ce20f8fd_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/16124095160"><img height="90" src="https://farm8.staticflickr.com/7565/16124095160_8ba10e6150_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/16211668457"><img height="90" src="https://farm9.staticflickr.com/8627/16211668457_200debd27a_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/16210184030"><img height="90" src="https://farm8.staticflickr.com/7446/16210184030_3d5628d5fa_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/16638589340"><img height="90" src="https://farm8.staticflickr.com/7600/16638589340_8ee9d2ac52_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/16824852532"><img height="90" src="https://farm8.staticflickr.com/7601/16824852532_6933106d0b_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/16836335602"><img height="90" src="https://farm8.staticflickr.com/7654/16836335602_75ae77a8b8_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/16215114034"><img height="90" src="https://farm8.staticflickr.com/7598/16215114034_fc50319105_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/22536836951"><img height="90" src="https://farm1.staticflickr.com/575/22536836951_3f36869bd8_m_d.jpg"/></a>
<a href="https://www.flickr.com/photos/sonic182/22525691595"><img height="90" src="https://farm6.staticflickr.com/5632/22525691595_2ab88d03b4_m_d.jpg"/></a>
</p>
