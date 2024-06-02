# ReklamationAPI
RESTful API zur Verwaltung von Produktreklamationen mit Unit- und End-to-End-Tests.

## Projektstruktur
### ReklamationAPI
=> Hauptprojekt, das die RESTful API enthält

### ReklamationAPI.Tests
=> Testprojekt mit Unit-Tests für die API

### Python E2E Test
=> Python Skript als E2E-Blackbox-Test


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

### Starten der Anwendung
Ausführen der API oder des Testprojekts in Visual Studio "Release" oder "Debug.
ReklamationAPI:
- Doppelklick auf Projekt "ReklamationAPI" im Solution Explorer
- Klick auf den grünen Run Button mit "https"
- ![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/424905f2-2e6b-40d3-bd81-3ccb7ad3130b)

ReklamationAPI.Tests
- Rechtsklick auf das Projekt "ReklamationAPI.Tests" im Solution Explorer
- Klick auf Run Tests
- ![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/10237d55-cff9-42b7-96ab-6241a14bfb12)
 


### Python E2E Test
Nach Start der API von der command line:
```console
cd Python E2E Test
python testReklamationAPI.py
```

## Frameworks / Features / Designentscheidungen
### API mit ASP.NET Core
Die Wahl fiel für mich auf ASP.NET Core, da ich zwar schon ein paar Erfahrungen damit gemacht habe, bisher jedoch noch keine RESTful API damit implementiert habe. Dadurch stellt das Projekt für mich eine gute Herausforderung und Übung zugleich dar. 

### Datenbank mit Sqlite / Entity Framework Code First Migration
Im Rahmen des Projekts erschien mir eine Sqlite Datenbank passend, die ohne großen Aufwand einrichten und direkt nutzen kann.
Das Entity Framework habe genutzt, da keine bsonderen Ansprüche an die Datenbank bestehen und der Microsoft Standard volkommen ausreichend sein sollte.
Außerdem ist die Integration der Datenbank durch Code First Migration erleichtert.

### Datenbankschema
Implementierung mit einer 1 zu n Beziehung zwischen Customer und Complaint.
Customers Table:
- Email (Primärschlüssel)
- Name

Complaint Table:
- Id (Primärschlüssel, automatisch generiert)
- CustomerEmail (Fremdschlüssel von "Customers")
- Date
- Description
- ProductId
- Status

OutboxMessages Table:
Id (Primärschlüssel, automatisch generiert)
Body
Date
Processed ( = Bool: Nachricht wurde schon verarbeitet?)
Subject
To

Zusätzlich noch Tables, die durch das Framework zur Authentifizierung erstellt wurden.

### Authentifizierung und Authorisierung mit JSON Web Token Bearer und UserIdentity
Mit dem JSON Web Token ließ sich clientseitig eine simple Authentifizierung umsetzen. Nach dem Login muss das generierte Token im Header nachfolgender Requests angegeben werden.
Es gibt die Rollen "Admin" und "User", wobei nur die Admin-Rolle für schreibende Requests berechtigt ist.
Die Benutzerverwaltung in der Datenbank ist mit dem Entity Framework Identity Kontext umgesetzt, da diese auch direkt out of the box nutzbar ist und den Anforderungen genügt.

### Benachrichtigungen mit Outbox-Tabelle und dediziertem Service
Bei gewissen verändernden Zugriffen auf die API sollen die betroffenen Nutzer benachrichtigt werden, was durch einen Eintrag in die Outbox-Tabelle und einen entsprechenden verarbeitenden Service umgesetzt wird.
Konzeptuell sind die Benachrichtigungen als Email gedacht, jedoch werden diese aufgrund fehlender Anbindung an ein SMTP-Server lediglich als Textdatei im Verzeichnis ReklamationAPI/emails prototypisch abgelegt.

### Integrierte Swagger zur Dokumentation der API mit Swashbuckle
Für die OpenAPI Dokumentation wird ein Swagger UI generiert, dass Informationen zu den Endpunkten bereitstellt.
Zusätzlich wird mit der Swashbuckle Bibliothek noch Zusatzinformationen bereitgestellt (über Annotationen, XML Kommentare und Example Klassen:
- Beschreibung der einzelnen API Aufrufe.
- Beispiel Request Bodies / Schema
- Beispiel Response / Schema
- Beschreibung der Request Parameter und Standardwerte

### Unit Tests mit MSTest, EF InMemory und Moq
Unit Tests wurden mit dem Microsoft Standard Framework MSTest implementiert.
Als Testdatenbank wird eine Entity Framework InMemory-Datenbank verwendet, die vor dem Test Beispieldaten erhält.
Abhängigkeiten zu Services (Authentification, Usermanager) wurden mit Mock Implementierung der enstprechenden Interfaces und den Controllern bereitgestellt.
Dafür habe ich das Moq Framework genutzt, da das wohl das gängigste Tool ist.
Aktuell sind 36 Unit Tests implementiert, die hauptsächlich die möglichen Szenarien bei Controller Aufrufen simulieren.

### Ent-to-End Test mit Python Skript

## API Dokumentation
## Testbeschreibung
