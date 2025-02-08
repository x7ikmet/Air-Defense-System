#ifndef Stepper_h
#define Stepper_h

class Stepper{
    public:
    Stepper(int Steps,int DirectionPin, int StepPin);

    void setDirection(bool direction);
    void setSpeed(int speed);
    void move();





    private:
    bool direction;
    int speed;
    int steps;
    int directionPin;
    int stepPin;

    int maxSteps = 150;
    int minSteps = -150;
    int currentStep = 0;

};



#endif