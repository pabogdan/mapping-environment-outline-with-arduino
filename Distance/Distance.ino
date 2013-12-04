#include<Servo.h>
Servo myServo;
int distancePin = A0;
int pos=0;
int lightPin = A1;
void setup()
{
myServo.attach(9);
Serial.begin(9600);
myServo.write(0);
delay(200);
}
int control=-10;
void loop()
{
pos = 0;
int go=0;
while(pos<170)
{ 
  if(Serial.available()>0){
    go = Serial.read() -48;  
}
  if(go==1)
  {
  myServo.write(pos);
  int val = analogRead(distancePin);
  Serial.println(control);
  Serial.println((1024 - val)/4, DEC);
  Serial.println(control);
  Serial.println(pos, DEC);
  Serial.println(control);
  int light=analogRead(lightPin);
  Serial.println(light/4, DEC);
  pos ++;
  go=0;
  }
   delay(30);
}

pos = 170;
go=0;
while(pos >=0 )
{
  if(Serial.available()>0)
    go = Serial.read() - 48;
  if(go==1)
  {
  myServo.write(pos);
  int val = analogRead(distancePin);
  Serial.println(control);
  Serial.println((1024 - val)/4, DEC);
  Serial.println(control);
  Serial.println(pos, DEC);
  Serial.println(control);
  int light=analogRead(lightPin);
  Serial.println(light/4, DEC);
  go = 0;
  pos --;
  }
  delay(30);
}
}
