Issues:
0) After allowing Resharper to tune all the variable names and minor optimizations, I was getting warnings during the build process.of the dll:

MSB3391 and MSB3214 saying that the dll didn't contain any types thta could be registered for COM Interop. It turned out that Resharper had changed the constructor for the driver:

Public Sub New() the proper one

to something like:

Public Sub New(imagedata as byval...) ...

So the driver couldn't even start.



1) Couldn't load the COM object error 80004002.  Right after clickinh on the chooser on the property button to open the setup form: The issue was with the profile reading\writing. I didn't specify an initila value for the ExposureTypeDefault value.
1.1) Issue was in the write profile sub, I used the same variable twice:
 MPixelHeightUm = driverProfile.GetValue(DriverId, PixelHeighthUmProfileName, String.Empty, PixelHeightUmDefault)
 MPixelHeightUm = driverProfile.GetValue(DriverId, MExposureTypeProfileName, String.Empty, MExposureTypeDefault)
 
 as opposed to the correct way:
 
 MPixelHeightUm = driverProfile.GetValue(DriverId, PixelHeighthUmProfileName, String.Empty, PixelHeightUmDefault)
 MExposureType = driverProfile.GetValue(DriverId, MExposureTypeProfileName, String.Empty, MExposureTypeDefault)

1.2) I had the definition of exposureprofiledefault left with no initial value:
  Friend Shared MExposureTypeProfileName As String = "Exposure Type"
  
    Friend Shared MExposureTypeProfileName As String
    as opposed to
    Friend Shared MExposureTypeProfileName As String = "Exposure Type"
    
    So the first time the program ran it had an undefined variable to write.
    
Both issues showed as something wrong with the regstry so that I ended up changing the calss guid in the driver.vb. It had nothing really to do with it though.
Funny because the solution will compile just fine and the problem will be noticed at runtime with a COM registration error!

2) Installation update didn't seem to update the ddl in the C:\Program Files (x86)\Common Files\ASCOM\Camera.
So I had to manually copy the new dll over the old one



