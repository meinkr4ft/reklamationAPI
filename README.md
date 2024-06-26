# ReklamationAPI
RESTful API zur Verwaltung von Produktreklamationen mit Unit-Tests und End-to-End-Tests.

## Inhaltsverzeichnis
1. [Projektstruktur](#project_structure)
2. [Installation](#installation)
3. [Frameworks / Features / Designentscheidungen](#frameworks)
    - [API mit ASP.NET Core](#api)
    - [Datenbank mit SQLite / Entity Framework Code First Migration](#datenbank)
    - [Datenbankschema](#schema)
    - [Authentifizierung und Authorisierung mit JSON Web Token Bearer und UserIdentity](#auth)
    - [Benachrichtigungen mit Outbox-Tabelle und dediziertem Service](#outbox)
    - [Integrierte Swagger zur Dokumentation der API mit Swashbuckle](#swagger)
    - [Unit Tests mit MSTest, Entity Framework InMemory Datenbank und Moq](#tests)
    - [End-to-End Test mit Python Skript](#e2e)
4. [API Dokumentation](#api-dokumentation)
     - [Login](#login)
     - [Alle Reklamationen anzeigen](#getall)
     - [Eine Reklamation anzeigen](#get)
     - [Reklamationen erstellen](#post)
     - [Reklamationen aktualisieren](#put)
     - [Reklamationen abbrechen](#delete)
     - [Reklamationen suchen](#search)
     - [Reklamationen filtern](#filter)
6. [Testbeschreibung](#testfaelle)
     - [Reklamationen anzeigen](#gettest)
     - [Reklamation erstellen](#posttest)
     - [Reklamation aktualisieren](#puttest)
     - [Reklamation abbrechen](#deletetest)
     - [Reklamationen suchen](#searchtest)
7. [Verbesserungsmöglichkeiten](#verbesserungsmoeglichkeiten)

## Projektstruktur <a name="project_structure"></a>
### ReklamationAPI
=> Hauptprojekt, das die RESTful API enthält.

### ReklamationAPI.Tests
=> Testprojekt mit Unit-Tests für die API.

### Python E2E Test
=> Python Skript als E2E-Blackbox-Test.


## Installation <a name="installation"></a>

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

## Frameworks / Features / Designentscheidungen <a name="frameworks"></a>
### API mit ASP.NET Core <a name="api"></a>
Die Wahl fiel für mich auf ASP.NET Core, da ich zwar schon ein paar Erfahrungen damit gemacht habe, bisher jedoch noch keine RESTful API implementiert habe. Dadurch stellt das Projekt für mich eine gute Herausforderung und Übung zugleich dar. 

### Datenbank mit SQLite / Entity Framework Code First Migration <a name="datenbank"></a>
Im Rahmen des Projekts erschien mir eine SQLite Datenbank passend, die man ohne großen Aufwand einrichten und direkt nutzen kann.\
Das Entity Framework habe ich genutzt, da keine besonderen Ansprüche an die Datenbank bestehen und der Microsoft Standard vollkommen ausreichend sein sollte.\
Außerdem ist die Integration der Datenbank durch Code First Migration erleichtert.\
Die Datenbank liegt unter ReklamationAPI/Database/app.db

### Datenbankschema <a name="schema"></a>
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
- Id (Primärschlüssel, automatisch generiert)
- Body
- Date
- Processed ( = Bool: Nachricht wurde schon verarbeitet?)
- Subject
- To

Zusätzlich noch Tables, die durch das Framework zur Authentifizierung erstellt wurden.

### Authentifizierung und Autorisierung mit JSON Web Token Bearer und UserIdentity <a name="auth"></a>
Mit dem JSON Web Token ließ sich clientseitig eine simple Authentifizierung umsetzen. Nach dem Login muss das generierte Token im Header nachfolgender Requests angegeben werden.\
Es gibt die Rollen "Admin" und "User", wobei nur die Admin-Rolle für schreibende Requests berechtigt ist.\
Die Benutzerverwaltung in der Datenbank ist mit dem Entity Framework Identity Kontext umgesetzt, da diese auch direkt out of the box nutzbar ist und den Anforderungen genügt.\
Logindaten Admin: admin Admin!123\
Logindaten User:  user  User!123

### Benachrichtigungen mit Outbox-Tabelle und dediziertem Service <a name="outbox"></a>
Bei Daten verändernden Zugriffen auf die API sollen die betroffenen Nutzer benachrichtigt werden, was durch einen Eintrag in die Outbox-Tabelle und einen entsprechenden verarbeitenden Service umgesetzt wird.\
Konzeptuell sind die Benachrichtigungen als Email gedacht, jedoch werden diese aufgrund fehlender Anbindung an ein SMTP-Server lediglich als Textdatei im Verzeichnis ReklamationAPI/emails prototypisch abgelegt.\
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/727a8160-c519-4577-a42a-587d88753554)



### Integrierte Swagger zur Dokumentation der API mit Swashbuckle <a name="swagger"></a>
Für die OpenAPI Dokumentation wird ein Swagger UI generiert, dass Informationen zu den Endpunkten bereitstellt.\
Zusätzlich wird mit der Swashbuckle Bibliothek noch Zusatzinformationen bereitgestellt (über Annotationen, XML Kommentare und Example Klassen:\
- Beschreibung der einzelnen API Aufrufe.
- Beispiel Request Bodies / Schema
- Beispiel Response / Schema
- Beschreibung der Request Parameter und Standardwerte

### Unit Tests mit MSTest, Entity Framework InMemory Datenbank und Moq <a name="tests"></a>
Unit Tests wurden mit dem Microsoft Standard Framework MSTest implementiert.\
Als Testdatenbank wird eine Entity Framework InMemory-Datenbank verwendet, die vor dem Test Beispieldaten erhält.\
Abhängigkeiten zu Services (Authentification, Usermanager) wurden mit Mock Implementierung der enstprechenden Interfaces und den Controllern bereitgestellt.\
Dafür habe ich das Moq Framework genutzt, welches das gängigste Tool ist.\
Aktuell sind 36 Unit Tests implementiert, die hauptsächlich die möglichen Szenarien bei Controller Aufrufen simulieren.

### End-to-End Test mit Python Skript <a name="e2e"></a>
Für einen E2E Test wollte ich einen Blackbox-Test umsetzen, der aus Sicht eines Anwenders geschehen soll.\
Da Python ohne großen Overhead sich dafür nutzen lässt, habe ich eine Reihe von aufeinanderefolgenden API Calls implementiert, die mit den entsprechenden GETs überprüft, ob die Daten richtig angelegt, aktualisiert und zurückgegeben werden.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/ed31e2ba-dade-4a20-8cc7-781344872259)


## API Dokumentation <a name="api-dokumentation"></a>
Die Basis-URL lautet:
https://localhost:7069/

### Swagger Dokumentation unter: <a name="swagger-dokumentation"></a>
https://localhost:7069/swagger/index.html
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/668a4695-222b-4066-a5d3-cac451526328)


Das beim Login erhaltene Token, kann unter Authorize eingegeben werden, um es bei folgenden Requests im Header zu inkludieren.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/5ae6d696-cd78-4478-b1a4-a905f409b8b2)
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/257253a0-24ae-47f4-b93e-404ffc46a591)

Im folgenden werden nur Anfragen beschrieben, die dem normalen Programmfluss entsprechen.\
Fehlerhafte oder unberechtigte Anfragen und Responses sind der Swagger Dokumentation zu entnehmen.\

### 1. Login <a name="login"></a>
Beschreibung: Endpunkt zum Authentifizieren mit Logindaten, um ein Authentication Token zu erhalten.\
Method: **POST**\
URL: **/api/Auth/login**\
Body Request:
```json
{
  "username": "{username}",
  "password": "{password}"
}
```

Body Response (200 OK):
```json
{
  "token": "{token}"
}
```

Beispiel Request:
```curl
curl -X 'POST' \
  'https://localhost:7069/api/Auth/login' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "admin",
  "password": "Admin!123"
}'
```

Beispiel Response Body:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzE3Mjk5NTI4LCJleHAiOjE3MTc5MDQzMjgsImlhdCI6MTcxNzI5OTUyOCwiaXNzIjoiUmVrbGFtYXRpb25BUElJc3N1ZXIiLCJhdWQiOiJSZWtsYW1hdGlvbkFQSUF1ZGllbmNlIn0.BDtMjf0PvuZPHKV07e45uQLHkxpZvcssnyW8_0LgdhY"
}
```

### 2. Alle Reklamationen anzeigen <a name="getall"></a>
Beschreibung: Endpunkt zur Anzeige aller Reklamationen.\
Method: **GET**\
URL: **/api/Complaints**\
Berechtigung: Jeder\
Keine URL Parameter oder Body notwendig.

Body Response (200 OK):
```json
[
  {
    "{complaint_response}",
    "{complaint_response}",
    "{...}"
  }
]
```

[Schema zu complaint_response](#complaint_response)


Beispiel Request:
```curl
curl -X 'GET' \
  'https://localhost:7069/api/Complaints' \
  -H 'accept: text/plain'
```

Beispiel Response Body:
```json
[
  {
    "id": 1,
    "productId": 101,
    "customer": {
      "email": "john.doe@example.com",
      "name": "John Doe"
    },
    "date": "2023-05-28",
    "description": "Das Produkt funktioniert nicht wie erwartet.",
    "status": "Open"
  },
  {
    "id": 2,
    "productId": 54,
    "customer": {
      "email": "max.mustermann@example.com",
      "name": "Max Mustermann"
    },
    "date": "2023-04-21",
    "description": "Die Lieferung kam nicht an.",
    "status": "Accepted"
  }
]
```

### 3. Eine Reklamation anzeigen <a name="get"></a>
Beschreibung: Endpunkt zur Anzeige einer einzelnen Reklamation anhand ihrer id.\
Method: **GET**\
URL: **/api/Complaints/{id}**\
Berechtigung: Jeder\
Die ID ist Teil der URL.\
Es gibt keinen Body im Request.

Body Response (200 OK):
```json
{
  "{complaint_response}"
}
```

[Schema zu complaint_response](#complaint_response)



Beispiel Request:
```curl
curl -X 'GET' \
  'https://localhost:7069/api/Complaints/1' \
  -H 'accept: text/plain'
```

Beispiel Response Body:
```json
[
{
  "id": 1,
  "productId": 101,
  "customer": {
    "email": "john.doe@example.com",
    "name": "John Doe"
  },
  "date": "2023-05-28",
  "description": "Das Produkt funktioniert nicht wie erwartet.",
  "status": "Open"
}
]
```

### 4. Reklamationen erstellen <a name="post"></a>
Beschreibung: Endpunkt zum Erstellen einer Reklamation, der nach der Erstellung die Reklamation (inkl. id) zurückgibt.\
Method: **POST**\
URL: **/api/Complaints**\
Berechtigung: **Nur Admin**\
Body Request:
```json
{
  "{complaint_dto}"
}
```

[Schema zu complaint_dto](#complaint_dto)

Body Response (201 Created):
```json
{
  "{complaint_response}"
}
```

[Schema zu complaint_response](#complaint_response)

Beispiel Request:
```curl
curl -X 'POST' \
  'https://localhost:7069/api/Complaints' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "productId": 101,
  "customer": {
    "email": "john.doe@example.com",
    "name": "John Doe"
  },
  "date": "2023-05-28",
  "description": "Das Produkt funktioniert nicht wie erwartet.",
  "status": "Open"
}'
```

Beispiel Response Body:
```json
{
  "id": 1,
  "productId": 101,
  "customer": {
    "email": "john.doe@example.com",
    "name": "John Doe"
  },
  "date": "2023-05-28",
  "description": "Das Produkt funktioniert nicht wie erwartet.",
  "status": "Open"
}
```

### 5. Reklamationen aktualisieren <a name="put"></a>
Beschreibung: Endpunkt zum Aktualisieren einer Reklamation anhand ihrer ID.\
**Es können nur Reklamationen mit dem Status "Open" oder "InProgress" aktualisiert werden.**\
Method: **PUT**\
URL: **/api/Complaint/{id}**\
Berechtigung: **Nur Admin**\
Die ID ist Teil der URL.\
Body Request: 
```json
{
  "{complaint_dto}"
}
```

[Schema zu complaint_dto](#complaint_dto)\
**Hinweis:** Nach aktueller Implementierung sind alle Felder beim PUT required, auch wenn deren Werte sich nicht ändern.

Body Response: 204 No Content

[Schema zu complaint_response](#complaint_response)

Beispiel Request:
```curl
curl -X 'PUT' \
  'https://localhost:7069/api/Complaints/1' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "productId": 101,
  "customer": {
    "email": "john.doe@example.com",
    "name": "John Doe"
  },
  "date": "2023-05-28",
  "description": "Das Produkt funktioniert nicht wie erwartet.",
  "status": "Open"
}'
```

### 6. Reklamationen abbrechen <a name="delete"></a>
Beschreibung: Endpunkt zum Abbrechen einer Reklamation anhand ihrer ID. Hierbei wird der Status wird dabei auf "Canceled" geändert.\
Method: **DELETE**\
URL: **/api/Complaint/{id}**\
Berechtigung: **Nur Admin**\
Die ID ist Teil der URL.\
Es ist kein Body notwendig.

Body Response: 204 No Content

Beispiel Request:
```curl
curl -X 'DELETE' \
  'https://localhost:7069/api/Complaints/1' \
  -H 'accept: */*' \
  -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzE3MzAzMzQxLCJleHAiOjE3MTc5MDgxNDEsImlhdCI6MTcxNzMwMzM0MSwiaXNzIjoiUmVrbGFtYXRpb25BUElJc3N1ZXIiLCJhdWQiOiJSZWtsYW1hdGlvbkFQSUF1ZGllbmNlIn0.0b3djRHCxgz1PSkFef0pR8vFfGyVxe73OZyXjgwHdYk'
```

### 7. Reklamationen suchen <a name="search"></a>
Beschreibung: Endpunkt zur Suche von Reklamationen. Werden mehrere Parameter angegeben, so sind in der Antwort alle Reklamationen erhalten, die **mindestens ein Kriterium** erfüllen.\
Method: **GET**\
URL: **/api/Complaints/search**\
Berechtigung: Jeder\
Die Parameter werden als URL Parameter angegeben. Alle Parameter sind **optional**.\
Kein Body notwendig.

Mögliche Parameter:
- productId
- customerName
- customerEmail
- date
- description
- status
- ignoreCase

Body Response (200 OK):
```json
{
  {searchDto}
  },
  "complaints": [
    "{complaint}",
    "{complaint}",
    "{...}"
  ]
}
```
[Schema zu search_dto](#search_dto)
Das Search-DTO kapselt die Suchparameter.
[Schema zu complaint_response](#complaint_response)


Beispiel Request:
```curl
curl -X 'GET' \
  'https://localhost:7069/api/Complaints/search?customerName=Max&status=Open&ignoreCase=true' \
  -H 'accept: text/plain'
```

Beispiel Response Body:
```json
{
  "searchDto": {
    "operation": "search",
    "productId": null,
    "customerName": "Max",
    "customerEmail": null,
    "date": null,
    "description": null,
    "status": "Open",
    "ignoreCase": true
  },
  "complaints": [
    {
      "id": 1,
      "productId": 101,
      "customer": {
        "email": "max.schmidt@gmail.com",
        "name": "Max Schmidt"
      },
      "date": "2023-05-28",
      "description": "Das Produkt funktioniert nicht wie erwartet.",
      "status": "Open"
    },
    {
      "id": 2,
      "productId": 54,
      "customer": {
        "email": "maximilian.weber@web.de",
        "name": "Maximilian Weber"
      },
      "date": "2023-04-21",
      "description": "Die Lieferung kam nicht an.",
      "status": "Rejected"
    },
    {
      "id": 3,
      "productId": 20,
      "customer": {
        "email": "Sabine.Wagner@yahoo.de",
        "name": "Sabine Wagner"
      },
      "date": "2023-03-12",
      "description": "Falsches Produkt wurde geliefert.",
      "status": "Open"
    }
  ]
}
```

### 8. Reklamationen filtern <a name="filter"></a>
Beschreibung: Endpunkt zum Filtern von Reklamationen. Werden mehrere Parameter angegeben, so sind in der Antwort alle Reklamationen erhalten, die **alle Kriterien** erfüllen.\
Method: **GET**\
URL: **/api/Complaints/filter**\
Berechtigung: Jeder\
Die Parameter werden als URL Parameter angegeben. Alle Parameter sind **optional**.\
Kein Body notwendig.

Mögliche Parameter:
- productId
- customerName
- customerEmail
- date
- description
- status
- ignoreCase

Body Response (200 OK):
```json
{
  {searchDto}
  },
  "complaints": [
    "{complaint}",
    "{complaint}",
    "{...}"
  ]
}
```
[Schema zu search_dto](#search_dto)
Das Search-DTO kapselt die Filterparameter.
[Schema zu complaint_response](#complaint_response)


Beispiel Request:
```curl
curl -X 'GET' \
  'https://localhost:7069/api/Complaints/filter?customerName=Max&status=Open&ignoreCase=true' \
  -H 'accept: text/plain'
```

Beispiel Response Body:
```json
{
  "searchDto": {
    "operation": "filter",
    "productId": null,
    "customerName": "Alex",
    "customerEmail": null,
    "date": null,
    "description": "liefer",
    "status": null,
    "ignoreCase": true
  },
  "complaints": [
    {
      "id": 1,
      "productId": 101,
      "customer": {
        "email": "alex.schmidt@gmail.com",
        "name": "Alex Schmidt"
      },
      "date": "2023-05-28",
      "description": "Falsches Produkt wurde geliefert.",
      "status": "Open"
    },
    {
      "id": 2,
      "productId": 54,
      "customer": {
        "email": "alexander.weber@web.de",
        "name": "Alexander Weber"
      },
      "date": "2023-04-21",
      "description": "Die Lieferung kam nicht an.",
      "status": "Rejected"
    },
    {
      "id": 3,
      "productId": 20,
      "customer": {
        "email": "Alexandra.Wagner@yahoo.de",
        "name": "Alexandra Wagner"
      },
      "date": "2023-03-18",
      "description": "Das Produkt wurde zwei Mal geliefert.",
      "status": "Open"
    }
  ]
}
```



### Schema zu Complaint response: <a name="complaint_response"></a>
```json
{
    "id": "{id}",
    "productId": "{product_id}",
    "customer": {
      "email": "{email}",
      "name": "{name}"
    },
    "date": "{date: YY-MM-DD}",
    "description": "{description}",
    "status": "{status: 'Open', 'InProgress', 'Accepted', 'Rejected' oder 'Canceled'}"
}
```

### Schema zu Complaint dto: <a name="complaint_dto"></a>
```json
{
    "productId": "{product_id}",
    "customer": {
      "email": "{email}",
      "name": "{name}"
    },
    "date": "{date: YY-MM-DD}",
    "description": "{description}",
    "status": "{status: 'Open', 'InProgress', 'Accepted', 'Rejected' oder 'Canceled'}"
}
```

### Schema zu Search/Filter dto: <a name="complaint_dto"></a>
```json
{
    "operation": "{operation: 'search' oder 'filter'}",
    "productId": "{product_id}",
    "customerName": "{customer_name}",
    "customerEmail": "{customer_email}",
    "date": "{date: YY-MM-DD}",
    "description": "{description}",
    "status": "{status: 'Open', 'InProgress', 'Accepted', 'Rejected' oder 'Canceled'}"
    "ignoreCase": "{ignore_case: true oder false}"
}
```


## Testbeschreibung <a name="testfaelle"></a>
Neben automatisierten Unit oder E2E Tests, lassen sich auf Testfälle definieren, die beispielsweise über das Swagger UI durchgeführt werden können.\
Exemplarisch werden ein paar mögliche Testfälle beschrieben.

### 1. Reklamationen anzeigen <a name="gettest"></a>
Vorbedingung: Mindestens ein Eintrag in der Complaints Tabelle in der Datenbank\
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/b2f73c77-dd3d-4256-aa38-f167bf39083c)


1. GET auf /api/Complaints/\
-> 200 OK. Es werden die gleichen Reklamationen zurückgegeben, die sich in der Datenbank einsehen lassen.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/114b790b-214b-4476-92c1-0de28965372f)

2. GET auf /api/Complaints/350 (eine ID, die in der Datenbank existiert\
-> 200 OK. Es wird nur eine Reklamation zurückgegeben. Die Daten decken sich mit denen aus der Datenbank.
   ![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/386342c1-941a-40b7-a4e8-4c225316a9b8)

3. GET auf /api/Complaints/250 (eine ID, die in der Datenbank nicht existiert)\
-> 404 Not found. Eine entsprechende Nachricht ist in der Response enthalten.
   ![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/5c701ae3-9ee3-47a3-9e00-a9e9717f2ec5)


### 2. Reklamation erstellen <a name="posttest"></a>
Vorbedingung: Logindaten mit Rolle "admin" sind vorhanden.
1. POST auf /api/auth/login mit Nutzerdaten im Body (Schema: siehe. [API Dokumentation](#api-dokumentation))\
-> 200 OK. Es wird ein Authentifizierungstoken zurückgesendet.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/a689bd83-cf29-412f-bd1f-3638fbac85ae)

2. Eingabe des Tokens in das Swagger Authorize-Feld und Klick auf Authorize
-> Das Schloss-Icon oben rechts wird geschlossen.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/53ad0111-66e5-48c4-8f81-9bb5b4bded0b)
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/a8290510-940b-43dc-b39b-2d8642d78a19)

3. POST auf /api/Complaints mit Complaint Objekt im Body (Schema: siehe. [API Dokumentation](#api-dokumentation))\
-> 201 Created. In der Response ist das Objekt mit ID angegeben. Die Erstellung lässt sich in der Datenbank verifizieren.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/c8360105-d7d8-43b3-81de-3c4c907d6452)
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/028f8af5-6428-4a47-a651-fcb0f8cbef4f)


### 3. Reklamation aktualisieren <a name="puttest"></a>
Vorbedingung: Logindaten mit Rolle "admin" sind vorhanden. Mindestens eine Reklamation mit Status "Open" oder "InProgress" ist in der Datenbank vorhanden.\
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/028f8af5-6428-4a47-a651-fcb0f8cbef4f)

1. POST auf /api/auth/login mit Nutzerdaten im Body (Schema: siehe. [API Dokumentation](#api-dokumentation))\
-> 200 OK. Es wird ein Authentifizierungstoken zurückgesendet.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/a689bd83-cf29-412f-bd1f-3638fbac85ae)

2. Eingabe des Tokens in das Swagger Authorize-Feld und Klick auf Authorize
-> Das Schloss-Icon oben rechts wird geschlossen.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/53ad0111-66e5-48c4-8f81-9bb5b4bded0b)
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/a8290510-940b-43dc-b39b-2d8642d78a19)

3. PUT auf /api/Complaints/359 (Id der Reklamation aus der Vorbedingung) mit Complaint Objekt im Body (Schema: siehe. [API Dokumentation](#api-dokumentation))\
-> 204 No Content. Die Aktualisierung lässt sich in der Datenbank verifizieren.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/15376bd2-2739-4bb6-9e9f-8b5c92620733)
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/9e3c5c2b-d70c-4ab3-af63-60a93a23cdca)

### 4. Reklamation abbrechen <a name="deletetest"></a>
Vorbedingung: Logindaten mit Rolle "admin" sind vorhanden. Mindestens eine Reklamation mit Status anders als "Canceled" ist in der Datenbank vorhanden\.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/9e3c5c2b-d70c-4ab3-af63-60a93a23cdca)

1. POST auf /api/auth/login mit Nutzerdaten im Body (Schema: siehe. [API Dokumentation](#api-dokumentation))\
-> 200 OK. Es wird ein Authentifizierungstoken zurückgesendet.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/a689bd83-cf29-412f-bd1f-3638fbac85ae)

2. Eingabe des Tokens in das Swagger Authorize-Feld und Klick auf Authorize
-> Das Schloss-Icon oben rechts wird geschlossen.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/53ad0111-66e5-48c4-8f81-9bb5b4bded0b)
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/a8290510-940b-43dc-b39b-2d8642d78a19)

3. DELETE auf /api/Complaints/359 (Id der Reklamation aus der Vorbedingung) ohne Body.\
-> 204 No Content. Der Status der Reklamation in der Datenbank wurde auf Canceled geändert.
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/f12ed1a8-b0ea-4de5-8fb6-cfaa5f52c81c)
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/decdc163-f8e0-4349-b59c-b63591f781e6)

### 5. Reklamationen suchen <a name="searchtest"></a>
Vorbedingung: Mindestens ein Eintrag in der Complaints Tabelle in der Datenbank\
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/7d5dca3e-baed-46f4-9e00-e83aaa908450)

1. GET auf /api/Complaints/search?status=open (Mit entsprechenden Parametern, die auf mindestens eine Reklamation zutreffen) (Schema: siehe. [API Dokumentation](#api-dokumentation))\
-> 200 OK. Es werden die Suchkriterien sowie die Reklamationen zurückgegeben, die den Suchkriterien entsprechen..
![image](https://github.com/meinkr4ft/reklamationAPI/assets/32766044/09eea387-11aa-4c7c-86f2-5dc65f66a935)


## Verbesserungsmöglichkeiten <a name="verbesserungsmoeglichkeiten"></a>
- Überlegung, ob die Properties beim PUT optional sein sollten oder nicht.
- Suche verfeinern, indem numerische Parameter auch eine Range darstellen können
- Eigene Datenbank für den Test Einrichten
- Parametrisierung der Konfiguration (z.B. Port)
- Anbindung eines SMTP-Service für Email Benachrichtigungen
- Refactoring des Codes, Umstrukturierung, Best Practices etc.
- Testfälle, bei denen DB Fehler auftreten (Datenbank ist locked, keine Verbindung)
- Lösung für Get / Suche / Filter bei sehr vielen Datne in der Datenbank (Netzbelastung)
- 2 kleinere Probleme mit Swagger, zu denen ich noch keine Lösung gefunden habe.
    1. Bei Responses wird immer der Code 200 angezeigt, auch wenn ich explizit nur andere Codes angebe
    https://github.com/swagger-api/swagger-core/issues/4044
    2. Die Parameter Beispiele lassen sich nicht als Placeholder definieren, sodass sie bei der Eingabe verschwinden.\
    So muss man bei der Suche erst alle Parameter löschen, die man nicht braucht.
    https://github.com/swagger-api/swagger-ui/issues/3920
