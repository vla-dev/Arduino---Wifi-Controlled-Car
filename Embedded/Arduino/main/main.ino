#include <ArduinoJson.h>
#include "LCDIC2.h"
#include<SoftwareSerial.h>

SoftwareSerial UNOSerial(2, 3); // RX - TX
LCDIC2 lcd(0x27, 20, 4);

bool DEBUG = true;

int STATUS_LED = 13;
int MOTOR_SPEED_PIN = 6;
int MOVE_FORWARD_PIN = 7;
int MOVE_BACKWARD_PIN = 8;

struct ACTION {
  String type, payload;
};

String LOCAL_IP;

ACTION ParseResponse(String response) {
  ACTION _action;

  if (!response.indexOf(":") == -1)
    return _action;

  int separator_index = response.indexOf(":");

  _action.type = response.substring(0, separator_index);
  _action.payload = response.substring(separator_index + 1, response.length());

  return _action;
}

String formattedLcdString(String str) {
  return str.substring(0, 19); // LCD CHARACTERS LENGTH
}

void blinkReadySignal(int times, int waitTime) {
  for (int i = 0; i < times; i++) {
    delay(waitTime);
    digitalWrite(STATUS_LED, HIGH);
    delay(waitTime);
    digitalWrite(STATUS_LED, LOW);
  }
}

void DispatchAction(ACTION _action) {
  if (DEBUG) blinkReadySignal(1, 10);

  if (_action.type.equals("STATUS")) {
    lcd.clear();
    lcd.print(formattedLcdString(_action.payload));
  }
  else if (_action.type.equals("LOCAL_IP")) {
    LOCAL_IP = _action.payload;

    lcd.clear();
    lcd.print("Connect to:");
    lcd.setCursor(0, 1);
    lcd.print(formattedLcdString(LOCAL_IP));
  }
  else if (_action.type.equals("RESET")) {
    Serial.println("Resetting...");
    digitalWrite(MOVE_FORWARD_PIN, LOW);
    digitalWrite(MOVE_BACKWARD_PIN, LOW);
    analogWrite(MOTOR_SPEED_PIN, 0);
  }
  else if (_action.type.equals("THROTTLE")) {
    int throttle_input = _action.payload.toInt();

    if (throttle_input > 5) {
      digitalWrite(MOVE_BACKWARD_PIN, LOW);
      digitalWrite(MOVE_FORWARD_PIN, HIGH);
      analogWrite(MOTOR_SPEED_PIN, abs(throttle_input));
    }
    else if (throttle_input < -5) {
      digitalWrite(MOVE_FORWARD_PIN, LOW);
      digitalWrite(MOVE_BACKWARD_PIN, HIGH);
      analogWrite(MOTOR_SPEED_PIN, abs(throttle_input));
    }
    else {
      digitalWrite(MOVE_FORWARD_PIN, LOW);
      digitalWrite(MOVE_BACKWARD_PIN, LOW);
      analogWrite(MOTOR_SPEED_PIN, 0);
    }
  }
}

void setup() {
  pinMode(STATUS_LED, OUTPUT);
  pinMode(MOVE_FORWARD_PIN, OUTPUT);
  pinMode(MOVE_BACKWARD_PIN, OUTPUT);

  Serial.begin(74880);
  UNOSerial.begin(74880);

  Serial.println("Initializing...");

  if (lcd.begin()) lcd.print("Initializing....");

  delay(1000);
  blinkReadySignal(20, 20);
}

void loop() {
  if (UNOSerial.available() > 0) {
    UNOSerial.setTimeout(0);
    String _response = UNOSerial.readString();

    ACTION _action = ParseResponse(_response);
    DispatchAction(_action);
  }
}
