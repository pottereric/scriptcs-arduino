var arduino = Require<Arduino>();
arduino.Debug = true;
arduino.OnBoardReady = () =>
{
    Console.WriteLine("Let's begin!");
    var led = new Led(a, 13);
    //led.Strobe();
};