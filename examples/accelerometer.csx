var arduino = Require<Arduino>();

arduino.Setup();
arduino.Open();

while(!Console.KeyAvailable)
{
}

arduino.Close();