#include <WiFiManager.h> // https://github.com/tzapu/WiFiManager
#include <WiFiUdp.h>
#include<SoftwareSerial.h>
#include<Servo.h>

unsigned int localPort = 8888;

char packetBuffer[UDP_TX_PACKET_MAX_SIZE + 1];
char  ReplyBuffer[] = "acknowledged\r\n";

int GREEN_LED = 4;
int STEERING_PIN = 14;
int THROTTLE_PIN = 16;
int THROTTLE_PIN2 = 5;

bool DEBUG = true;

String STATUS_PREFIX = "STATUS:";
String LOCAL_IP_PREFIX = "LOCAL_IP:";

Servo STEERING_SERVO;
SoftwareSerial ESPSerial(12, 13); // RX - TX
WiFiUDP Udp;

struct ACTION {
  String type, payload;
};


void blinkReadySignal(int times, int waitTime) {
  for (int i = 0; i < times; i++) {
    delay(waitTime);
    digitalWrite(GREEN_LED, HIGH);
    delay(waitTime);
    digitalWrite(GREEN_LED, LOW);
  }
}

void setup() {
  delay(2000); // START ESP MODULE AFTER ARDUINO IS INITIALIZED

  pinMode(GREEN_LED, OUTPUT);
  pinMode(THROTTLE_PIN, OUTPUT);
  pinMode(THROTTLE_PIN2, OUTPUT);
  STEERING_SERVO.attach(STEERING_PIN);  //PIN FROM ESP8266 TO AVOID DATA LOSS THROUGH SERIAL PORT ACCORDING TO PACKETS AMOUNT AND BAUD_RATE

  digitalWrite(GREEN_LED, LOW);

  WiFi.mode(WIFI_STA);

  // put your setup code here, to run once:
  Serial.begin(74880);
  ESPSerial.begin(74880);

  String _status = STATUS_PREFIX + "Starting...";
  ESPSerial.print(_status);

  WiFiManager wm;
  bool res;
  res = wm.autoConnect("Wifi_Car", "12345678"); // password protected ap

  if (!res) {
    Serial.println("Failed to connect");
  }
  else {
    blinkReadySignal(6, 40);

    String _status = STATUS_PREFIX + "Connected to wifi";
    ESPSerial.print(_status);

    delay(1000);
    String _localIP = LOCAL_IP_PREFIX + WiFi.localIP().toString() + ":" + localPort;
    ESPSerial.print(_localIP);

    STEERING_SERVO.write(90);

    Udp.begin(localPort);

  }
}

ACTION ParseResponse(String response) {
  ACTION _action;

  if (!response.indexOf(":") == -1)
    return _action;

  int separator_index = response.indexOf(":");

  _action.type = response.substring(0, separator_index);
  _action.payload = response.substring(separator_index + 1, response.length());

  return _action;
}

void DispatchAction(ACTION _action) {
  if (DEBUG) blinkReadySignal(1, 10);

  if (_action.type.equals("STEERING_ANGLE")) {
    int angle = _action.payload.toInt();

    int _mappedAngle = map(angle, 0, 180, 140, 40);

    STEERING_SERVO.write(_mappedAngle);

    Serial.println(_mappedAngle);
  }
  else if (_action.type.equals("THROTTLE")) {
    int throttle_input = _action.payload.toInt();
    Serial.println(throttle_input);
  }
}

void loop() {
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    if (DEBUG) blinkReadySignal(1, 1);

    int n = Udp.read(packetBuffer, UDP_TX_PACKET_MAX_SIZE);
    packetBuffer[n] = 0;

    String _packetResponse = (String)packetBuffer;

    ACTION _action = ParseResponse(_packetResponse);
    DispatchAction(_action);

    if (_action.type != "STEERING_ANGLE") {
      ESPSerial.setTimeout(1);
      ESPSerial.print(_packetResponse);
    }

    Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
    Udp.write(ReplyBuffer);
    Udp.endPacket();
  }
}
