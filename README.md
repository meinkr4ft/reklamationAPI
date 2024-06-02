# reklamationAPI
RESTful API zur Verwaltung von Produktreklamationen mit Unit- und End-to-End-Tests.

## Projektstruktur
### ReklamationAPI
-> Hauptprojekt, das die RESTful API enthält

### ReklamationAPI.Tests
-> Testprojekt mit Unit-Tests für die API

### Python E2E Test
-> Python Skript als E2E-Blackbox-Test

## Installation

### Voraussetzungen
C# Web API (Haupt- und Testprojekt)
- .NET 8.0 Runtime
- NuGet-Packages installieren mit Visual Studio 2022 in der Package Manager Console:
```console
Update-Package -reinstall
```
Update-Package -reinstall

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
ReklamationAPI und Testprojekt jeweils mit Visual Studio 2022.

### Start
Ausführen der API oder des Testprojekts in Visual Studio "Release" oder "Debug.

### Python E2E Test
Nach Start der API von der command line:
```console
cd Python E2E Test
python testReklamationAPI.py
```

