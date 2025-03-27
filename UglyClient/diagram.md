@startuml Environment Control System

' Interfaces
interface IEnvironmentService {
  +GetTemperature(int sensorId): Task<double>
  +SetFanState(int fanId, bool state): Task
  +SetHeaterLevel(int heaterId, int level): Task
}

' Enums
enum DeviceType {
  Sensor
  Heater
  Fan
}

' Data Models
class Device {
  +Id: int
  +Name: string
  +Type: DeviceType
}

class Temperature {
  +Value: double
  +Unit: string
}

class FanDTO {
  +Id: int
  +IsOn: bool
}

' Current Implementation
class Program {
  -NUM_SENSORS: const int
  -NUM_HEATERS: const int
  -NUM_FANS: const int
  +Main(args: string[]): Task
  -Reset(client: HttpClient): Task
  -RunTemperatureControlLoop(client: HttpClient): Task
  -AdjustTemperature(client: HttpClient, currentTemp: double, targetTemp: double, duration: int): Task<double>
  -HoldTemperature(client: HttpClient, currentTemp: double, targetTemp: double, duration: int): Task<double>
  -GetAverageTemperature(client: HttpClient): Task<double>
  -SetAllHeaters(client: HttpClient, level: int): Task
  -SetAllFans(client: HttpClient, state: bool): Task
  -GetSensor1Temperature(client: HttpClient): Task<string>
  -GetSensor2Temperature(client: HttpClient): Task<int>
  -GetSensor3Temperature(client: HttpClient): Task<decimal>
  -GetSensorTemperature(client: HttpClient, sensorId: int): Task<double>
  -SetHeaterLevel(client: HttpClient, heaterId: int, level: int): Task
  -SetFanState(client: HttpClient, fanId: int, isOn: bool): Task
  -DisplayAllSensorTemperatures(client: HttpClient): Task
}

' Proposed Implementation
class ApiSettings {
  +BaseUrl: string
  +ApiKey: string
  +NumSensors: int
  +NumHeaters: int
  +NumFans: int
}

class HttpEnvironmentService {
  -_client: HttpClient
  -_settings: ApiSettings
  +GetTemperature(sensorId: int): Task<double>
  +SetFanState(fanId: int, state: bool): Task
  +SetHeaterLevel(heaterId: int, level: int): Task
}

class TemperatureController {
  -_service: IEnvironmentService
  +AdjustTemperature(currentTemp: double, targetTemp: double, duration: int): Task<double>
  +HoldTemperature(currentTemp: double, targetTemp: double, duration: int): Task<double>
  +GetAverageTemperature(): Task<double>
}

class ConsoleUI {
  -_controller: TemperatureController
  -_service: IEnvironmentService
  +ShowMenu(): void
  +ProcessUserInput(input: string): Task
  +DisplayAllDevices(): Task
}

' Relationships
IEnvironmentService <|.. HttpEnvironmentService
HttpEnvironmentService --> ApiSettings
TemperatureController --> IEnvironmentService
ConsoleUI --> TemperatureController
ConsoleUI --> IEnvironmentService

Program --> FanDTO

' Not currently implemented relationships, but should be
HttpEnvironmentService ..> Temperature: returns
HttpEnvironmentService ..> Device: manages

@enduml