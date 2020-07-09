# Wifi controlled Car

[![N|Solid](https://cdn.iconscout.com/icon/free/png-256/arduino-1-226076.png)](https://nodesource.com/products/nsolid)

Controlled via a custom module using:

 - ESP8266-07
 - Atmega328p
 - Some electronic parts to make that thing works..

# How it works?

Simply turn on the power switch, connect to the Wifi module via your Mobile Phone.
After the connection is established tap on the wifi hotspot that you're connected to and there you'll see an IP address like: 'http://192.168.1.4' ... smash it with your big finger and navigate to the Wifi Manager portal...
Here you'll have a list with all of the available networks on your area. Select one which you want to connect to and if it's necessary, input your network credentials.

When the car is connected to your local network, the green LED will blink for some time that indicate that the connection has been successfully established.

Now the car is connected to your local network... but how can we control it?

- Knock, knock!
- Who's there?
- Unity
- Which unity?
- Unity that's a cross platform game engine and user friendly for beginners

To control the car you need to build the unity project and to install the final build file on your mobile phone.
In the main screen of the application you need to input the local IP of the WIFI module that's mounted on your car..

ok... but where can I found it?
That's a good question...

I didn't find any way to expose the local IP of the module on your phone after the car is connected to the local network.. BUT!!

You can use a simple LCD.
Wiring:
VCC to Arduino 5V
GND to Arduino GND
SDA to Arduino SDA ( Analog pin A04 )
SCL to Arduino SCL ( Analog pin A05 )

All right!
After you connect the LCD, restart the module and on the LCD will be shown different messages like: "Initializing...", "Connecting..." and finally you'll see the local IP of the module.
Something like: "192.168.100.15" and the UDP Transport Protocol Port ( 8888 ).

So far so good! 
Now enter that IP on your mobile app build and tap 'Connect!'

Yaai!! It works!

I'll be back with some images and videos to demonstrate the functionality.
