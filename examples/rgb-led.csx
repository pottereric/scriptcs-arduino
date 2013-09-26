var ctx = Require<ArduinoContext>();
var board = ctx.CreateBoard();
var random = new Random();

var led = new LedRGB(board, 9, 10, 11);
Action wait = () => Thread.Sleep(2.Seconds());
Action hold = () => Thread.Sleep(50.Milliseconds());
for (int i = 0; i < 20; i++)
{
    led.Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
    hold();
}
led.Off();

board.Close();