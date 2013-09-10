var arduino = Require<Arduino>();
arduino.Setup();
arduino.Open();
arduino.PinMode(13, Arduino.OUTPUT);
for (int i = 0; i < 10; i++) {
	arduino.DigitalWrite(13, Arduino.HIGH);
	Thread.Sleep(150);
	arduino.DigitalWrite(13, Arduino.LOW);
	Thread.Sleep(150);    
}

arduino.Close();