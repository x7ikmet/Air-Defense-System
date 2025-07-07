#include <Arduino.h>
#include <AccelStepper.h>

#define STEP_PIN 9
#define DIR_PIN 10

const int limitMin = 2;
const int limitMax = 3;

AccelStepper stepper(AccelStepper::DRIVER, STEP_PIN, DIR_PIN);

void setup() {
    stepper.setMaxSpeed(1000);
    
    stepper.runSpeed();
    stepper.setAcceleration(50000);
}

void loop() {
  stepper.move(1600);
//   stepper.runSpeedToPosition();
  stepper.runToPosition();

  delay(1000); 

  stepper.move(-1600);
//   stepper.runSpeedToPosition();
  stepper.runToPosition();

  delay(1000);
}