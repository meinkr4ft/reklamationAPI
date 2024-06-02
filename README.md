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

### Build
C# mit Visual Studio 2022 oder von der command line:
```console
msbuild ReklamationAPI.sln /p:Configuration=Release
```

### Start
C# Hauptprojekt:
```console
cd ReklamationAPI\bin\Release\net8.0
dotnet ReklamationAPI.dll
```

C# Testprojekt:
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

