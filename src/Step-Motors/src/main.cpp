#include <Arduino.h>
#include <Stepper.h>

#define STEP_PIN 33
#define DIR_PIN 32


Stepper stepper(400, DIR_PIN, STEP_PIN);

void setup() {

    stepper.setSpeed(1000);
    stepper.setDirection(true);
}

void loop() {

  for (int i = 0; i < 400; i++) { 
    stepper.step();
  }

  delay(1000); 


  stepper.setDirection(!digitalRead(DIR_PIN));

  delay(1000);
}
