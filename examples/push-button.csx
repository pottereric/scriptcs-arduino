var ctx = Require<ArduinoContext>();
var arduino = ctx.CreateBoard();
var buttonPin = 2;
var ledPin = 13;

arduino.PinMode(buttonPin, PinMode.Input);
arduino.PinMode(ledPin, PinMode.Output);

while(!Console.KeyAvailable)
{
	var buttonPosition = arduino.DigitalRead(buttonPin);
	DigitalPin mode = buttonPosition == 1 ? DigitalPin.High : DigitalPin.Low;
	arduino.DigitalWrite(ledPin, mode);
}

arduino.Close();
