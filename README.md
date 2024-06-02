# reklamationAPI
RESTful API zur Verwaltung von Produktreklamationen mit ASP.NET Core

### Voraussetzungen
C# Web API (Haupt- und Testprojekt)
- .NET 8.0 Runtime

Python (End-to-End-Test)
- Python >= 3.8
- Modules
  - requests
  - urllib3
```console
pip install requests
pip install urllib3
```

### Clone
```console
git clone https://github.com/meinkr4ft/reklamationAPI.git
```

### Build
C# mit Visual Studio 2022 oder von der command line:
```console
msbuild ReklamationAPI.sln /p:Configuration=Release
```

### Start
C# Hauptprojekt:
Entweder in Visual Studio 2022 oder con der command line:
```console
cd ReklamationAPI\bin\Release\net8.0
dotnet ReklamationAPI.dll
```

C# Testprojekt:
Entweder in Visual Studio 2022 oder con der command line:
```console
cd ReklamationAPI.Tests\bin\Release\net8.0
dotnet ReklamationAPI.Tests.dll
```

# Python E2E Test
Nach Start der API mit folgendem Befehl:
```console
cd Python E2E Test
python testReklamationAPI.py
```

