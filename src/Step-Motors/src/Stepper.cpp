#include <Arduino.h>
#include <Stepper.h>


Stepper::Stepper(int Steps, int DirectionPin, int StepPin){
    steps = Steps;
    directionPin = DirectionPin;
    stepPin = StepPin;
    pinMode(directionPin, OUTPUT);
    pinMode(stepPin, OUTPUT);
}

void Stepper::setDirection(bool Direction){
    direction = Direction;
    digitalWrite(directionPin, direction);
}

void Stepper::setSpeed(int Speed){
    speed = Speed;
}

