var arduino = Require<Arduino>();
var echoPin = 7;
var triggerPin = 8;

arduino.Setup();
arduino.Open();
arduino.PinMode(echoPin, Arduino.INPUT);
arduino.PinMode(triggerPin, Arduino.OUTPUT);

while(!Console.KeyAvailable)
{
	arduino.DigitalWrite(triggerPin, Arduino.LOW);
	Thread.Sleep(2);
	arduino.DigitalWrite(triggerPin, Arduino.HIGH);
	Thread.Sleep(10);
	arduino.DigitalWrite(triggerPin, Arduino.HIGH);

	
	Thread.Sleep(500);
}

arduino.Close();
  
void loop()  
{  
  //seta o pino 12 com um pulso baixo "LOW" ou desligado ou ainda 0  
    digitalWrite(trigPin, LOW);  
  // delay de 2 microssegundos  
    delayMicroseconds(2);  
  //seta o pino 12 com pulso alto "HIGH" ou ligado ou ainda 1  
    digitalWrite(trigPin, HIGH);  
  //delay de 10 microssegundos  
    delayMicroseconds(10);  
  //seta o pino 12 com pulso baixo novamente  
    digitalWrite(trigPin, LOW);  
  //pulseInt lê o tempo entre a chamada e o pino entrar em high  
    long duration = pulseIn(echoPin,HIGH);  
  //Esse calculo é baseado em s = v . t, lembrando que o tempo vem dobrado  
  //porque é o tempo de ida e volta do ultrassom  
    long distancia = duration /29 / 2 ;  
  
Serial.print("Distancia em CM: ");  
Serial.println(distancia);  
delay(1000); //espera 1 segundo para fazer a leitura novamente  
}  