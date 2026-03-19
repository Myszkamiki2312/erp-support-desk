# ERP Support Desk

ERP Support Desk to aplikacja webowa napisana w C# i .NET 8, przygotowana jako projekt portfolio pod stanowisko laczace helpdesk, serwis techniczny, obsluge klienta i rozwoj rozwiazan ERP.

Projekt symuluje codzienna prace firmy wdrozeniowej:

- przyjmowanie i obsluge zgloszen serwisowych
- prace z klientami korzystajacymi z systemow ERP
- monitoring integracji z systemami zewnetrznymi
- raportowanie backlogu i ryzyk operacyjnych

## Opis projektu

Aplikacja pelni role wewnetrznego panelu operacyjnego dla zespolu wsparcia technicznego. Pokazuje zgloszenia, klientow, integracje oraz podstawowy raport techniczny. Projekt zostal zaprojektowany tak, aby pokazywac praktyczne wykorzystanie C#, .NET, pracy z baza danych i logiki biznesowej w realiach wsparcia systemow ERP.

Projekt jest inspirowany realiami pracy przy systemach klasy ERP, w tym wdrozeniach i utrzymaniu srodowisk podobnych do Comarch ERP XL i Optima.

Repo zawiera dwa interfejsy:

- klasyczna aplikacje `ASP.NET Core MVC`
- dodatkowy klient `Angular` oparty o wlasny, lekki interfejs formularzy i tabel

## Stack

- C#
- .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- Angular 21
- Entity Framework Core
- SQLite
- SQL Server (konfiguracja opcjonalna)
- Bootstrap 5
- wlasne style CSS

## Najwazniejsze funkcje

- dashboard z KPI i kolejka priorytetowa
- lista zgloszen z filtrowaniem
- dodawanie i edycja zgloszen
- export listy zgloszen do CSV
- karta klienta z aktywnymi tematami i integracjami
- monitoring integracji i synchronizacji
- raport operacyjny z rekomendacjami
- automatyczne seedowanie danych demo po pierwszym uruchomieniu
- API dla klienta Angular
- widok zgłoszen w Angularze z filtrowaniem i formularzem dodawania danych

## Struktura projektu

- `Controllers` - kontrolery MVC i API
- `Data` - kontekst bazy i seed danych
- `Models` - encje domenowe
- `Services` - logika biznesowa i agregacja danych
- `ViewModels` - modele pod widoki
- `Views` - warstwa interfejsu MVC
- `wwwroot` - statyczne zasoby frontendu MVC
- `angular-client` - klient Angular korzystajacy z API backendu

## Jak uruchomic backend .NET

### Wymagania

- zainstalowany .NET SDK 8

### Kroki

```bash
cd erp-support-desk
dotnet restore
dotnet build ErpSupportDesk.sln
dotnet run --project ErpSupportDesk.csproj --urls http://localhost:5080
```

### Po uruchomieniu backendu

- MVC demo: `http://localhost:5080`
- API dashboardu: `http://localhost:5080/api/dashboard`
- API zgłoszen: `http://localhost:5080/api/tickets`

Przy pierwszym starcie projekt automatycznie utworzy lokalna baze SQLite w katalogu `App_Data` i zaladuje dane demo.

## Jak uruchomic klient Angular

```bash
cd angular-client
npm install
npm start -- --host 127.0.0.1
```

Po uruchomieniu otworz:

- `http://127.0.0.1:4200`

Klient Angular korzysta z proxy do backendu dzialajacego na `http://localhost:5080`.

Jesli po zmianach kodu endpointy `/api` zwracaja `404`, zatrzymaj stare procesy `dotnet` i uruchom backend ponownie na `5080`, zeby Angular polaczyl sie z aktualna wersja aplikacji.

## Jak wlaczyc SQL Server zamiast SQLite

Domyslnie projekt startuje na `SQLite`, bo to najszybsza wersja demo. Backend obsluguje tez `SQL Server` przez `Microsoft.EntityFrameworkCore.SqlServer`.

Przyklad uruchomienia z `SQL Server`:

```bash
Database__Provider=SqlServer \
ConnectionStrings__SqlServerConnection="Server=localhost,1433;Database=ErpSupportDesk;User Id=sa;Password=Your_strong_password123;TrustServerCertificate=True;" \
dotnet run --project ErpSupportDesk.csproj --urls http://localhost:5080
```

W tym trybie aplikacja dalej seeduje dane demo, ale zapisuje je do bazy `SQL Server`.

## Jak sprawdzic, czy dziala

### MVC

- `http://localhost:5080`
- dashboard z metrykami i kolejka priorytetowa
- zakladke `Zgloszenia`
- zakladke `Klienci`
- zakladke `Integracje`
- zakladke `Raport`

### Angular

- `http://127.0.0.1:4200`
- `Dashboard`
- `Zgloszenia`
- filtry i formularz Angular
- formularz dodawania nowego zgłoszenia

### Szybki test manualny

1. Wejdz w `Zgloszenia`
2. Otworz dowolne zgloszenie
3. Kliknij `Edytuj`
4. Zmien status albo liczbe godzin
5. Zapisz zmiany i sprawdz, czy dane sie odswiezyly

Mozesz tez dodac nowe zgloszenie w MVC albo w Angularze i sprawdzic, czy pojawi sie na liscie backlogu.

## Co pokazuje ten projekt

Projekt ma pokazac, ze potrafie:

- pracowac w C# i .NET
- budowac aplikacje webowe MVC i SPA
- korzystac z bazy danych i warstwy ORM
- przygotowac projekt pod SQLite i SQL Server
- budowac frontend Angular komunikujacy sie z API .NET
- organizowac logike biznesowa w czytelnej strukturze
- projektowac interfejs pod narzedzie biznesowe
- laczyc obszar supportu technicznego z programowaniem
