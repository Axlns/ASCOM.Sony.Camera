To user your camera with applications supporting ASCOM cameras (such as SGP Pro, SharpCap, MaximDL, etc) follow these steps

1) Set your camera USB Connection settings to PC remote
2) Connect your camera to computer using USB cable
3) Run Sony Imaging Edge Remote app, select your camera and make sure the app connected to your camera successfully
4) Select ASCOM Sony Camera driver in your app
5) Set settings for your camera in ASCOM driver settings dialog.
	i) Make sure you selected your camera model, default ISO settings and Image Format
	ii) If your camera model is not in the list you still can try to use the driver with your camera, click "Setup My Camera" button and fill-in various parameters for your camera, such as sensor size, resolution, available ISO values, available shutter speeds, etc.
	iii) Driver will try to set automatically exposure time and ISO settings in Imaging Edge Remote app, but it cannot set File Format settings matching your selected Image Format. 
	     Therefore make sure that Image Format in driver settings matching File Format in Imaging Edge Remote app. Select RAW file format for CFA and Debayered image formats, and select JPEG for JPG image format
6) Now you can try to use your camera with ASCOM compatible imaging app.


If you have issues using this driver - please post detailed description of the probem as well as driver log on cloudynights.com here - 
Source codes of the driver can be found here -

Known issues:
1) Sequence Generator Pro can only work with CFA image format, do not try to use other image formats with. Overall CFA image format (pure RAW data) is most tested and you should try to use it in most cases. Other image formats are experimental at this stage of driver development.