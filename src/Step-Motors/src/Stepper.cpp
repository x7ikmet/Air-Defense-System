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


void Stepper::step(){
    if (currentStep < maxSteps && direction == true){
        digitalWrite(stepPin, HIGH);
        delayMicroseconds(speed);
        digitalWrite(stepPin, LOW);
        delayMicroseconds(speed);
        currentStep++;
    }
    else if (currentStep > minSteps && direction == false){
        digitalWrite(stepPin, HIGH);
        delayMicroseconds(speed);
        digitalWrite(stepPin, LOW);
        delayMicroseconds(speed);
        currentStep--;
    }
}
