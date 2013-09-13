var arduino = Require<Arduino>();
var temperaturePin = 0;
var ledPin = 13;

arduino.Setup();
arduino.Open();
arduino.PinMode(ledPin, Arduino.OUTPUT);

while(!Console.KeyAvailable)
{
	var temperatureValue = arduino.AnalogRead(temperaturePin);
	if(temperatureValue < 500)
	{
		arduino.DigitalWrite(ledPin, 1);	
	}
	else 
	{
		arduino.DigitalWrite(ledPin, 0);		
	}
	
	Thread.Sleep(500);
}

arduino.Close();