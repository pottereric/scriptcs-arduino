var ctx = Require<ArduinoContext>();
var board = ctx.CreateBoard();
var random = new Random();

var led = new Led(board, 9);
Action wait = () => Thread.Sleep(2.Seconds());
led.StrobeOn();
wait();
led.StrobeOff();
led.Off();

board.Close();