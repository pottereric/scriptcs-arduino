var arduino = Require<Arduino>();
arduino.Debug = true;
arduino.OnBoardReady = () =>
{
    Console.WriteLine("Let's begin!");
    var led = new Led(arduino, 13);
    led.Strobe();
    Thread.Sleep(2000);
    led.Stop();
};