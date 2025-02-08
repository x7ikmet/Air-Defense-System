#include <Arduino.h>

#define STEP_PIN 33
#define DIR_PIN 32

void setup() {
  pinMode(STEP_PIN, OUTPUT);
  pinMode(DIR_PIN, OUTPUT);
  
  digitalWrite(DIR_PIN, HIGH); 
}

void loop() {
  // Move exactly 1 full revolution (400 steps)
  for (int i = 0; i < 400; i++) { 
    digitalWrite(STEP_PIN, HIGH);
    delayMicroseconds(1000);  
    digitalWrite(STEP_PIN, LOW);
    delayMicroseconds(1000);
  }

  delay(1000); 


  digitalWrite(DIR_PIN, !digitalRead(DIR_PIN));

  delay(1000);
}
