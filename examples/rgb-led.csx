var arduino = Require<Arduino>();
var random = new System.Random();
var pins = new[] { 9, 10, 11 };
var brightness = 0;
var increment = 5;
arduino.Setup();
arduino.Open();
for (int j = 0; j < 5; j++)
{
	for(var i = 0; i < 255; i++)
	{
		foreach(var pin in pins)
			arduino.AnalogWrite(pin, i);
	}
	for(var i = 255; i > 0; i--)
	{
		foreach(var pin in pins)
			arduino.AnalogWrite(pin, i);
	}
}
foreach(var pin in pins)
	arduino.AnalogWrite(pin, 0);
arduino.Close();