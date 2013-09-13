var arduino = Require<Arduino>();
var buttonPin = 2;
var ledPin = 13;

arduino.Setup();
arduino.Open();
arduino.PinMode(2, Arduino.INPUT);
arduino.PinMode(ledPin, Arduino.OUTPUT);

while(!Console.KeyAvailable)
{
	var buttonPosition = arduino.DigitalRead(buttonPin);
	arduino.DigitalWrite(ledPin, buttonPosition);
}

arduino.Close();