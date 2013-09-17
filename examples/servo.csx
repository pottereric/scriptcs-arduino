var arduino = Require<Arduino>();
var servoPin = 9;
arduino.Setup();
arduino.Open();

arduino.PinMode(servoPin, 0x04);
//center
var position = 70;
var steps = 20;
arduino.AnalogWrite(servoPin, position);
Thread.Sleep(500);

var key = "";
Console.WriteLine("press 'a' to turn left, 'd' to turn right or any other key to exit");
while(new[]{"a", "d"}.Contains(key = Console.ReadKey().KeyChar.ToString()))
{
	if(key == "a")
	{
		position += steps;
	}
	else
	{
		position -= steps;	
	}
	if (position > 140)
	{
		position = 140;
	} else if (position < 0)
	{
		position = 0;
	}

	arduino.AnalogWrite(servoPin, position);
	Thread.Sleep(500);
}

arduino.Close();