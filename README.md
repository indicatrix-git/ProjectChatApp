# Real-Time Chat (WPF + ASP.NET Core + SignalR + MySQL)

Jednoduchá real-time chat aplikácia bežiaca lokálne. Skladá sa z dvoch projektov:

- **ChatApi** – ASP.NET Core Web API + SignalR Hub + Entity Framework Core (MySQL)
- **ChatClient** – WPF desktop klient (MVVM, CommunityToolkit.Mvvm)

```
WPF Client 1
        \
         ASP.NET Core API + SignalR  ->  MySQL (ChatApp)
        /
WPF Client 2
```

---

## Čo potrebuješ

- **.NET 8 SDK** (https://dotnet.microsoft.com/download)
- **Visual Studio 2022** (s workloadmi „.NET desktop development" a „ASP.NET")
- **MySQL** alebo **MariaDB** (napr. XAMPP, MySQL Server, alebo MariaDB)
- Windows (WPF beží len na Windows)

---

## 1) Databáza

Databázu **nemusíš vytvárať ručne** – backend ju pri prvom spustení vytvorí sám
(`db.Database.EnsureCreated()` v `Program.cs`), vrátane tabuľky `Messages`.

Stačí mať bežiaci MySQL/MariaDB server a správny connection string.

Uprav `ChatApi/appsettings.json` podľa svojho prostredia:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=ChatApp;User=root;Password=;"
}
```

- `User` / `Password` nastav podľa svojho MySQL účtu.
- Pri XAMPP býva používateľ `root` s prázdnym heslom (ako vyššie).

### Tabuľka Messages

| Stĺpec  | Typ          |
|---------|--------------|
| Id      | int (PK)     |
| Sender  | varchar(50)  |
| Text    | varchar(500) |
| SentAt  | datetime     |

> Voliteľne môžeš použiť EF migrácie namiesto `EnsureCreated`:
> `dotnet ef migrations add InitialCreate` a v `Program.cs` zmeniť na `db.Database.Migrate();`
> (potrebný je nainštalovaný nástroj `dotnet-ef`).

---

## 2) Spustenie backendu (ChatApi)

V termináli:

```bash
cd ChatApi
dotnet run
```

Server pobeží na **http://localhost:5000** (nastavené v `Properties/launchSettings.json`).

Overenie histórie v prehliadači: `http://localhost:5000/api/messages`
→ vráti JSON pole posledných 50 správ.

---

## 3) Spustenie WPF klienta (ChatClient)

V druhom termináli:

```bash
cd ChatClient
dotnet run
```

Alebo vo Visual Studiu nastav `ChatClient` ako startup projekt a spusti (F5).

Adresa servera je v `ChatClient/App.xaml.cs`:

```csharp
private const string BaseUrl = "http://localhost:5000";
private const string HubUrl  = "http://localhost:5000/chathub";
```

---

## 4) Viac klientov naraz

Aby si videl real-time komunikáciu, spusti klienta viackrát:

```bash
cd ChatClient
dotnet run        # okno 1
dotnet run        # okno 2 (nový terminál)
```

Každý zadá iný nickname, pripojí sa a správy chodia okamžite všetkým.

---

## Ako to celé funguje

**Pripojenie:**
1. Užívateľ zadá nickname (`NicknameWindow`).
2. Klient načíta históriu cez REST `GET /api/messages` (`ApiService`).
3. Klient sa pripojí na SignalR hub `/chathub` (`SignalRService`).

**Odoslanie správy:**
1. `Send` zavolá `SendCommand` → `SignalRService.SendMessageAsync(nickname, text)`.
2. Hub `ChatHub.SendMessage` správu zvaliduje, uloží do MySQL (EF Core)
   a broadcastne ju metódou `ReceiveMessage` všetkým klientom.
3. Každý klient ju dostane cez event `MessageReceived` a pridá do zoznamu
   (`ObservableCollection`), zoznam sa automaticky odscrolluje na poslednú správu.

---

## Štruktúra projektu

```
ChatApp.sln
ChatApi/                         (backend)
  Program.cs                     - konfigurácia DI, EF, SignalR, CORS
  appsettings.json               - connection string
  Models/Message.cs              - DB entita
  Data/ChatContext.cs            - EF Core DbContext
  Controllers/MessagesController - GET /api/messages
  Hubs/ChatHub.cs                - SendMessage / ReceiveMessage
  Dtos/MessageDto.cs             - DTO pre REST odpoveď
ChatClient/                      (WPF, MVVM)
  App.xaml(.cs)                  - štart appky, prepojenie okien
  Models/Message.cs              - model správy + Header ("Peter (14:22)")
  ViewModels/MainViewModel.cs    - logika chatu (CommunityToolkit.Mvvm)
  Views/NicknameWindow.xaml      - zadanie prezývky
  Views/ChatWindow.xaml          - hlavné chat okno
  Services/ApiService.cs         - REST volania
  Services/SignalRService.cs     - SignalR pripojenie
```

---

## Časté problémy

- **„Could not connect to the server"** – beží backend na `http://localhost:5000`?
  Spusti najprv `ChatApi`, až potom klienta.
- **Chyba pripojenia k DB** – skontroluj, že MySQL/MariaDB beží a connection string
  v `appsettings.json` má správneho používateľa a heslo.
- **Iný port** – ak zmeníš port servera, uprav aj `BaseUrl`/`HubUrl` v `App.xaml.cs`.
