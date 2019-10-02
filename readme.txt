This driver will make your Sony camera ASCOM compatible.

To make sure everything works correctly follow these steps:

1)	Install latest version of ASCOM Platform
2)	Set your camera USB Connection settings to PC remote
3)	Connect your camera to computer using USB cable
4)	Run Sony Imaging Edge Remote app, select your camera and make sure the app connected to your camera successfully. Enable this BULB mode settings here - https://prnt.sc/pds6fj
5)	Select ASCOM Sony Camera driver in your app
6)	Set settings for your camera in ASCOM driver settings dialog.
	i)	Make sure you selected your camera model, default ISO settings and Image Format
	ii)	If your camera model is not in the list you still can try to use the driver with your camera, you would need to edit cameramodel.json file located in C:\Program Files (x86)\Common Files\ASCOM\Camera\ASCOM.Sony.Camera, the file is commented and it should clear how to add new camera models
	iii)Driver will try to set automatically exposure time and ISO settings in Imaging Edge Remote app, but it cannot set File Format settings matching your selected Image Format. 
		Therefore make sure that Image Format in driver settings matching File Format in Imaging Edge Remote app. Select RAW file format for CFA and Debayered image formats, and select JPEG for JPG image format
7) Now you can try to use your camera with ASCOM compatible imaging app, such as SharpCap or Sequence Generator Pro


If you have issues using this driver - please post detailed description of the probem as well as driver log on cloudynights.com here - https://www.cloudynights.com/topic/678666-ascom-driver-for-sony-cameras/
Source codes of the driver can be found here - https://github.com/Axlns/ascom.sony.camera

Known issues:
1)	Sequence Generator Pro can only work with CFA image format, do not try to use other image formats. 
	Overall CFA image format (pure RAW data) is most tested and you should try to use it in most cases. Other image formats are experimental at this stage of driver development.

Credits

1)	Thanks to Mark (sharkmelley on cloudynights.com), he created app called MarksAcquisition and I used his idea of communication between PC and Sony cameras. He also was very nice and gave me source codes of his app. Thank you Mark!
2)	Thanks to Robin, creator of SharpCap app (https://www.sharpcap.co.uk/) he created another ASCOM driver called ASCOM.DSLR for Canon and Nikon cameras, I used source codes of ASCOM.DSLR as example and used big chunks of them in my driver
3)	Thanks to everyone who put his efforts into ASCOM platform!