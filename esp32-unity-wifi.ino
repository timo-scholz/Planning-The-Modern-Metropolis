#include <WiFi.h>
#include <TB6612_ESP32.h>
const char* ssid = "SSID";
const char* password = "PASSWORD";
WiFiServer server(12345);

#define AIN1 13 // ESP32 Pin D13 to TB6612FNG Pin AIN1
#define BIN1 12 // ESP32 Pin D12 to TB6612FNG Pin BIN1
#define AIN2 14 // ESP32 Pin D14 to TB6612FNG Pin AIN2
#define BIN2 27 // ESP32 Pin D27 to TB6612FNG Pin BIN2
#define PWMA 26 // ESP32 Pin D26 to TB6612FNG Pin PWMA
#define PWMB 25 // ESP32 Pin D25 to TB6612FNG Pin PWMB
#define STBY 33 // ESP32 Pin D33 to TB6612FNG Pin STBY

// A 4 Cell battery pack was used to supply 6 Volts of DC current to the robot and to the ESP32
// A wire splitter was used to connect the Positive (+) side of the Battery pack to the VIN pin of the ESP32,
// and to the VM pin of the TB6612FNG Motor Controller.
// Another wire splitter was used to connect the Negative (-) side of the Battery pack to the GND pin of the ESP32,
// and to the GND pin of the TB6612FNG Motor Controller


// these constants are used to allow you to make your motor configuration
// line up with function names like forward.  Value can be 1 or -1
const int offsetA = 1;
const int offsetB = 1;

Motor motor1 = Motor(AIN1, AIN2, PWMA, offsetA, STBY,5000 ,8,1 );
Motor motor2 = Motor(BIN1, BIN2, PWMB, offsetB, STBY,5000 ,8,2 );

void setup() {
  Serial.begin(115200);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }
  Serial.println("Connected to WiFi");
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
  server.begin();
}

void loop() {
  WiFiClient client = server.available();

  if (client) {
    Serial.println("Client connected");
    while (client.connected()) {
      if (client.available()) {
        String data = client.readStringUntil('\n');
        Serial.println("Received: " + data);
        updateRotation(data);
      }
    }
    client.stop();
    Serial.println("Client disconnected");
  }
}

void updateRotation(String dataString) {
  motorSendData(mappedSpeed(abs(dataString.toFloat())), (dataString.toFloat() >= 0));
  
}

float mappedSpeed(float pSpeed){
  Serial.print(pSpeed);
  Serial.print(" mapped to ");
  return ((pSpeed - 0) * (128 - 0) / (0.50 - 0) + 0);
  
  //       MaxMotor Speed~^           ^~MaxReadSpeed
  //Max Speed vorsichtshalber auf 128 also halbe Maximal Geschwindgkeit eingestellt
}

void motorSendData(float speed, bool dir){
  Serial.println(speed);
  if (dir){
    motor1.drive(speed, 10);
  }else{
    motor1.drive(-speed, 10);
  }
}