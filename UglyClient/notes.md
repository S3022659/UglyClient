# Structure
- Single class Program containing all functionality
- Uses HTTP client for API communication
- Implements a console-based menu system for user interaction

## Key Components

### Constants and Configuration

- Hardcoded values for number of sensors (3), heaters (3), and fans (3)
- API endpoint configuration and API key management

### Main Control Loop

- Menu-driven interface with 6 options
- Switch statement handling different user actions
- Basic error handling for user inputs

### Device Control Functions

- Individual control of fans (on/off)
- Heater level control (0-5)
- Temperature sensor reading
- Bulk device operations (setting all fans/heaters)

### Temperature Control Algorithm

- Multi-phase temperature control
- Temperature adjustment and holding mechanisms
- Feedback loop using sensor readings

## Issues and Areas for Improvement

### Code Organization

- Lack of proper separation of concerns
- No clear object-oriented design
- Heavy use of static methods

### Error Handling

- Inconsistent error handling patterns
- Duplicate error handling code
- Mixed error reporting strategies

### Configuration

- Hardcoded values throughout
- No configuration file usage
- Magic numbers in temperature control logic

### Architecture

- No interface abstractions
- Tight coupling between components
- Limited testability due to design

### API Communication

- Direct HTTP client usage
- No service layer abstraction
- Repetitive API endpoint handling

## Recommendations
- Implement proper class structure for devices (Sensor, Heater, Fan)
- Create service layer for API communication
- Use dependency injection
- Implement configuration management
- Add proper logging mechanism
- Create interface abstractions for testability
- Implement proper exception handling strategy
- Add input validation layer

## Weaknesses
1. Poor Separation of Concerns
All functionality is contained in a single Program class
No clear separation between UI, business logic, and API communication
Mixed concerns between device control and temperature management
2. Code Duplication
// Multiple places with similar HTTP handling code
    // Similar error handling patterns repeated

3. Inconsistent Error Handling
// Different error handling approaches

4. Hard-coded Values
5. Poor API Communication Design
Direct HttpClient usage throughout the code
No API service abstraction
Hardcoded API endpoints
6. Lack of Models/DTOs
Only one DTO class (FanDTO)
Missing models for Heaters and Sensors
Inconsistent data type handling for temperatures
7. No Configuration Management
8. Poor Testability
Static methods throughout
Direct dependencies
No interface abstractions

# Patterns to use TODO!

factory - http client
facade - api
adapter - sensors
state - controls - fans heaters
command - user input
