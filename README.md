# TrafficJamAnalyzer 🚦

TrafficJamAnalyzer is an advanced tool designed to help monitor and analyze traffic conditions by processing images from CCTV cameras around the roads of Tenerife. By utilizing artificial intelligence (AI) with Semantic Kernel and OpenAI, the application accurately assesses traffic density and identifies locations with potential traffic jams.

## Table of Contents
- [Features](#features)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Project Structure](#project-structure)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features
- Real-time analysis of traffic conditions.
- Integration with CCTV camera feeds.
- AI-based detection for traffic jams using Semantic Kernel and OpenAI.
- User-friendly interface for monitoring traffic.
- Configurable settings for specific roads and regions.

## System Requirements
Before installing TrafficJamAnalyzer, ensure your system meets the following requirements:

- **Operating System**: Windows 10 or later
- **Runtime Environment**: .NET 8
- **Processor**: Intel i5 or equivalent
- **Memory**: 8 GB RAM or higher
- **Storage**: 2 GB of available space
- **Additional Software**:
  - Access to OpenAI API (requires API key)
  - Access to the camera feeds from local CCTV systems

## Installation
Follow these steps to install TrafficJamAnalyzer:

1. **Clone the Repository**:
    ```sh
    git clone https://github.com/emimontesdeoca/TrafficJamAnalyzer.git
    cd TrafficJamAnalyzer
    ```

2. **Set Up the Environment**:
   Ensure .NET 8 SDK is installed on your machine. Download it [here](https://dotnet.microsoft.com/download).

3. **Install Dependencies**:
   Install the necessary packages using NuGet Package Manager:
   ```sh
   dotnet restore
   ```

4. **Configure AI Integration**:
   - Download the Semantic Kernel.
   - Set up your OpenAI API key and configure the appsettings in the `AiApiService`.

5. **Build the Application**:
   ```sh
   dotnet build
   ```

6. **Run the Application**:
   ```sh
   dotnet run
   ```

## Project Structure
The project is structured as follows:

```
C:\DEVELOPMENT\SPEAKS\AI\TRAFFICJAMANALYZER
├───TrafficJamAnalyzer.ApiService
├───TrafficJamAnalyzer.AppHost
├───TrafficJamAnalyzer.ServiceDefaults
├───TrafficJamAnalyzer.Services.AiApiService
├───TrafficJamAnalyzer.Services.ScraperApiService
├───TrafficJamAnalyzer.Services.TrafficService
├───TrafficJamAnalyzer.Shared.Clients
├───TrafficJamAnalyzer.Shared.Models
├───TrafficJamAnalyzer.Web
└───TrafficJamAnalyzer.Workers.Analyzer
```

## Configuration
Edit the `appsettings.json` file in the `TrafficJamAnalyzer.Services.AiApiService` project to include your OpenAI API integration settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "OpenAI": {
    "Endpoint": "",
    "DeploymentName": "",
    "ApiKey": "",
    "Prompt": "The image I'm going to provide you is an image from a CCTV that shows a road, I need you to give me a JSON object with 'Title'' which is title in the top left and 'Traffic' which is an integer from 0 to 100 which shows the amount of traffic jam and the 'Date' that is on the bottom right, please only provide the JSON result and nothing else. Return only the json object without any markdown. If you a lot of lanes, please focus on the one that is busy when checking for the traffic, so, if you see 4 lanes and only 2 are full, it means that the traffic is jammed."
  }
}
```

## Usage
1. **Launch the Application**:
   Start the application via the command line or your chosen IDE.

2. **Configure Settings**:
   - Set up the CCTV camera feeds.
   - Configure your Semantic Kernel settings as per your requirements.
   - Ensure your OpenAI API key is correctly set in `appsettings.json`.

3. **Start Analysis**:
   Begin the real-time traffic analysis to monitor and evaluate traffic conditions.

4. **Review Results**:
   Traffic conditions will be displayed on the dashboard, highlighting areas with potential traffic jams.

## Contributing
We welcome contributions to TrafficJamAnalyzer!

1. **Fork the Repository**.
2. **Create a Branch**: 
   ```sh
   git checkout -b feature/your-feature-name
   ```
3. **Commit Your Changes**: 
   ```sh
   git commit -m 'Add some feature'
   ```
4. **Push to the Branch**:
   ```sh
   git push origin feature/your-feature-name
   ```
5. **Open a Pull Request**.

## License
Distributed under the MIT License. See `LICENSE` for more information.

Thank you for using TrafficJamAnalyzer! We hope it helps you keep the roads of Tenerife traffic-free.
