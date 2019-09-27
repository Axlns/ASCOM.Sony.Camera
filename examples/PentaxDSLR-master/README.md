# PentaxDSLR
ASCOM driver for Pentax - GNU GPL3

I wrote this code to control my Pentax camera from an ASCOM application, using the ASCOM camera interface.
It is actually a generic driver with two essential features:

1) Trigger the shutter control by means of RS232
2) read images from a monitored folder (camera connected via USB)

The main need for this derived from the need to be able to focus a pentax IR-modified camera attached to a telescope. 
In order to use software like FocusMax I needed an ASCOM driver...

However, this is not a Pentax only driver as is, in fact any camera that can connect via USB being seen as a folder by a PC 
and that can be triggered by an RS232 cable can be setup with this.

Future possibilities:
This could become a truly Pentax only driver if a USB connection could also activate the shutter. 
There are a few apps that are now capable of doing that like PKtriggercord (http://pktriggercord.melda.info), PKTether (http://www.pktether.com/)
or WIFI controlling WIFI enabled Pentax cameras like https://www.pentaxforums.com/forums/6-pentax-dslr-discussion/346566-my-pentax-wifi-remote-software.html.
It would require some work but a true Pentax ASCOM driver is a real possibility with what's available out there.

Ciao!

Vincenzo
