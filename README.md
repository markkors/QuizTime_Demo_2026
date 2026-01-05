# QuizTime - Quiz Applicatie

Een Windows Desktop applicatie (WPF) voor het maken en afnemen van quizzen, inclusief een demo REST API voor het ophalen van quizvragen.

## Wat is dit project?

QuizTime is een educatief project dat bestaat uit twee delen:

1. **WPF Desktop Applicatie** - Een Windows applicatie gebouwd met C# en .NET 8.0
2. **Demo REST API** - Een eenvoudige PHP REST API die quizvragen levert in JSON formaat

Dit project laat zien hoe je een desktop applicatie kunt koppelen aan een web API om gegevens op te halen.

---

## Technische Vereisten

### Voor de WPF Applicatie:
- Windows 10 of hoger
- Visual Studio 2022 (Community editie is gratis)
- .NET 8.0 SDK

### Voor de Demo REST API:
- Een lokale webserver met PHP ondersteuning
  - XAMPP (aanbevolen voor beginners)
  - WAMP
  - PHP Built-in Server
- PHP 7.4 of hoger

---

## Installatie en Gebruik

### 1. WPF Applicatie Starten

1. Open `QuizTime.sln` in Visual Studio
2. Druk op **F5** of klik op de groene "Start" knop
3. De applicatie wordt gebouwd en gestart

### 2. Demo REST API Opzetten

De demo API bevindt zich in de map `QuizTime/restapi.local/` en bevat voorbeeldcode om quizvragen op te halen.

#### Optie A: Met XAMPP

1. Installeer [XAMPP](https://www.apachefriends.org/)
2. Kopieer de map `restapi.local` naar `C:\xampp\htdocs\`
3. Start Apache via het XAMPP Control Panel
4. Open je browser en ga naar: `http://localhost/restapi.local/all`
5. Je zou nu JSON data met quizvragen moeten zien

#### Optie B: Met PHP Built-in Server

1. Open een Command Prompt of PowerShell
2. Navigeer naar de `restapi.local` map:
   ```bash
   cd C:\Users\markk\source\repos\QuizTime\QuizTime\restapi.local
   ```
3. Start de PHP server:
   ```bash
   php -S localhost:8000
   ```
4. Open je browser en ga naar: `http://localhost:8000/all`

---

## WPF Applicatie Uitleg

De WPF (Windows Presentation Foundation) applicatie is het hart van dit project. Het is een Windows desktop applicatie die een quiz interface biedt en communiceert met de REST API.

### Projectstructuur

```
QuizTime/
├── Models/
│   └── oQuestion.cs           # Data model voor een quizvraag
├── ViewModels/
│   ├── MainViewmodel.cs       # Logica voor het hoofdvenster
│   └── SecondWindowViewModel.cs
├── Views/
│   ├── MainWindow.xaml        # UI definitie van hoofdvenster
│   ├── MainWindow.xaml.cs     # Code-behind voor hoofdvenster
│   ├── SecondWindow.xaml
│   └── SecondWindow.xaml.cs
├── ValueConverters/
│   └── BoolToColorConverter.cs # Converter voor visuele feedback
├── App.xaml                    # Applicatie configuratie
└── App.xaml.cs                 # Applicatie startpunt
```

### MVVM Patroon

Deze applicatie gebruikt het **MVVM (Model-View-ViewModel)** patroon, een populair design pattern voor WPF applicaties.

#### Wat is MVVM?

MVVM scheidt je code in drie lagen:

1. **Model** - De data en business logica
2. **View** - De gebruikersinterface (XAML)
3. **ViewModel** - De verbinding tussen Model en View

**Voordelen:**
- Scheiding van UI en logica
- Makkelijker te testen
- Overzichtelijke code
- Herbruikbare componenten

**Schema:**
```
View (XAML)
    ↕ Data Binding
ViewModel (C# Properties/Commands)
    ↕
Model (Data Classes)
    ↕
API / Database
```

### Onderdelen Uitleg

#### 1. Model - `oQuestion.cs`

Dit is de data klasse die een quizvraag voorstelt:

```csharp
public class oQuestion
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("question")]
    public string? Question { get; set; }

    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }

    [JsonPropertyName("answer")]
    public int Answer { get; set; }
}
```

**Belangrijke onderdelen:**
- `[JsonPropertyName]` - Koppelt de C# property aan de JSON veldnaam
- `Id` - Uniek nummer van de vraag
- `Question` - De vraagtekst
- `Options` - Lijst met antwoordmogelijkheden
- `Answer` - Index van het correcte antwoord (0-based)

**Voorbeeld:**
Als `Answer = 0` en `Options = ["Parijs", "Lyon", "Marseille"]`, dan is "Parijs" het juiste antwoord.

#### 2. ViewModel - `MainViewmodel.cs`

De ViewModel bevat alle logica en data voor de View. Deze klasse implementeert `INotifyPropertyChanged` om de UI automatisch te updaten.

**Belangrijke properties:**

```csharp
// De titel van de applicatie
public string SuperTitel { get; set; }

// Alle quizvragen opgehaald van de API
public List<oQuestion> Questions { get; set; }

// De momenteel geselecteerde vraag
public oQuestion CurrentQuestion { get; set; }

// Het geselecteerde antwoord
public string SelectedOption { get; set; }

// Of het antwoord correct is (voor visuele feedback)
public bool IsAnswerCorrect { get; set; }
```

**API Communicatie:**

De ViewModel haalt data op van de REST API met `HttpClient`:

```csharp
private async Task getQuestions()
{
    var httpClient = new HttpClient();
    var URL = "http://restapi.local/all";

    try
    {
        var response = await httpClient.GetAsync(URL);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            Questions = JsonSerializer.Deserialize<List<oQuestion>>(json);
        }
    }
    catch (Exception ex)
    {
        // Handle exceptions
    }
}
```

**Uitleg stappenplan:**
1. Maak een `HttpClient` aan
2. Stuur een GET request naar `http://restapi.local/all`
3. Wacht op de response met `await`
4. Lees de JSON response uit
5. Converteer de JSON naar C# objecten met `JsonSerializer.Deserialize`
6. Sla de vragen op in de `Questions` property

**INotifyPropertyChanged - Waarom?**

Zonder `INotifyPropertyChanged` weet de UI niet dat data is veranderd. Met dit interface krijgt de UI automatisch updates:

```csharp
public class MainViewmodel : INotifyPropertyChanged
{
    private string _title;

    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title)); // UI wordt geüpdatet!
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

**Antwoord Validatie:**

De `CheckAnswer()` methode controleert of een antwoord correct is:

```csharp
private void CheckAnswer()
{
    if (CurrentQuestion != null && SelectedOption != null)
    {
        int selectedIndex = CurrentQuestion.Options.IndexOf(SelectedOption);
        IsAnswerCorrect = selectedIndex == CurrentQuestion.Answer;
    }
}
```

#### 3. View - `MainWindow.xaml`

De View definieert de gebruikersinterface in XAML (een XML-achtige taal).

**Layout structuur:**

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />  <!-- Linker kolom -->
        <ColumnDefinition Width="*" />  <!-- Rechter kolom -->
    </Grid.ColumnDefinitions>
    <Grid.RowDefinitions>
        <RowDefinition Height="*" />    <!-- Bovenste rij -->
        <RowDefinition Height="*" />    <!-- Onderste rij -->
    </Grid.RowDefinitions>
</Grid>
```

**Belangrijke UI elementen:**

1. **Titel TextBlock:**
```xml
<TextBlock Text="{Binding Title}"
           FontSize="24"
           HorizontalAlignment="Center" />
```
- `{Binding Title}` - Koppelt aan de `Title` property in de ViewModel
- Updates automatisch wanneer `Title` verandert

2. **Vraag Selector (ComboBox):**
```xml
<ComboBox ItemsSource="{Binding Questions}"
          SelectedItem="{Binding CurrentQuestion}"
          DisplayMemberPath="Question" />
```
- `ItemsSource` - Lijst met alle vragen
- `SelectedItem` - De geselecteerde vraag
- `DisplayMemberPath` - Welke property moet worden getoond ("Question")

3. **Antwoorden Lijst (ListView):**
```xml
<ListView ItemsSource="{Binding CurrentQuestion.Options}"
          SelectedItem="{Binding SelectedOption}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding}" />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```
- Toont alle antwoordopties van de geselecteerde vraag
- Wanneer je een antwoord selecteert, wordt `SelectedOption` in de ViewModel geupdatet

4. **Visuele Feedback (Background Converter):**
```xml
<StackPanel Background="{Binding IsAnswerCorrect,
                Converter={StaticResource BoolToColorConverter}}">
```
- De achtergrond verandert op basis van `IsAnswerCorrect`
- Gebruikt de `BoolToColorConverter` om `true/false` om te zetten naar kleuren

#### 4. Value Converter - `BoolToColorConverter.cs`

Converters zetten data om naar een formaat dat de UI kan gebruiken.

```csharp
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType,
                         object parameter, CultureInfo culture)
    {
        if (value is bool isCorrect)
        {
            return isCorrect ? Brushes.Green : Brushes.Red;
        }
        return Brushes.Gray;
    }
}
```

**Hoe werkt het?**
- `IsAnswerCorrect = true` → Groene achtergrond
- `IsAnswerCorrect = false` → Rode achtergrond
- Anders → Grijze achtergrond

**Gebruik in XAML:**

1. **Registreer de converter in Window.Resources:**
```xml
<Window.Resources>
    <converters:BoolToColorConverter x:Key="BoolToColorConverter" />
</Window.Resources>
```

2. **Gebruik de converter in een binding:**
```xml
Background="{Binding IsAnswerCorrect, Converter={StaticResource BoolToColorConverter}}"
```

### Data Binding Uitgelegd

**Data Binding** is de kern van WPF en verbindt de UI met je code zonder handmatige updates.

#### Soorten Bindings:

1. **OneWay** (standaard voor de meeste controls):
```xml
<TextBlock Text="{Binding Title}" />
```
- Data vloeit van ViewModel → View
- View wordt automatisch geupdatet

2. **TwoWay** (standaard voor input controls):
```xml
<TextBox Text="{Binding UserInput, Mode=TwoWay}" />
```
- Data vloeit beide kanten op: ViewModel ↔ View
- Wijzigingen in de TextBox updaten automatisch de ViewModel

3. **OneTime:**
```xml
<TextBlock Text="{Binding Title, Mode=OneTime}" />
```
- Data wordt één keer gezet bij het laden
- Geen updates daarna

#### Binding Path Uitleg:

```xml
<!-- Directe property -->
<TextBlock Text="{Binding Title}" />

<!-- Geneste property -->
<TextBlock Text="{Binding CurrentQuestion.Question}" />

<!-- Property van een item in een lijst -->
<ListView ItemsSource="{Binding Questions}">
    <TextBlock Text="{Binding Question}" />  <!-- Bindt aan Question property van elk item -->
</ListView>
```

### Hoe de Applicatie Werkt - Stap voor Stap

1. **Applicatie start** (`App.xaml`)
   - `StartupUri="Views/MainWindow.xaml"` opent het hoofdvenster
   - MainViewModel wordt aangemaakt als resource

2. **MainWindow laadt**
   - Constructor wordt aangeroepen
   - `DataContext = vm` koppelt de ViewModel aan de View
   - `getQuestions()` wordt aangeroepen

3. **API Call**
   - HttpClient haalt data op van `http://restapi.local/all`
   - JSON wordt gedeserialiseerd naar `List<oQuestion>`
   - `Questions` property wordt geset
   - UI wordt automatisch geupdatet (dankzij INotifyPropertyChanged)

4. **Gebruiker selecteert een vraag**
   - ComboBox `SelectedItem` verandert
   - `CurrentQuestion` in ViewModel wordt geupdatet
   - ListView toont de antwoordopties van deze vraag

5. **Gebruiker kiest een antwoord**
   - ListView `SelectedItem` verandert
   - `SelectedOption` in ViewModel wordt geupdatet
   - `CheckAnswer()` wordt automatisch aangeroepen
   - `IsAnswerCorrect` wordt berekend
   - `BoolToColorConverter` zet de waarde om naar een kleur
   - Achtergrond wordt groen (correct) of rood (fout)

### Belangrijke C# Concepten

#### 1. Async/Await

```csharp
private async Task getQuestions()
{
    var response = await httpClient.GetAsync(URL);
    // Wacht hier tot de response binnen is
    // De UI blijft responsive!
}
```

**Waarom async/await?**
- Voorkomt dat de UI "bevriest" tijdens langdurige operaties
- De applicatie blijft responsive
- Andere taken kunnen ondertussen doorgaan

**Keywords:**
- `async` - Markeert een methode als asynchroon
- `await` - Wacht op het resultaat zonder de UI te blokkeren
- `Task` - Representeert een asynchrone operatie

#### 2. Properties met Getters/Setters

```csharp
private string _title;  // Private field (backing field)

public string Title     // Public property
{
    get { return _title; }
    set
    {
        _title = value;
        OnPropertyChanged(nameof(Title));
    }
}
```

**Waarom properties?**
- Je kunt logica toevoegen bij get/set
- Nodig voor data binding
- Betere encapsulatie dan public fields

#### 3. Null-able Types

```csharp
public string? Question { get; set; }
```

Het `?` betekent dat deze property `null` mag zijn. Dit voorkomt null reference warnings.

#### 4. JSON Deserialization

```csharp
Questions = JsonSerializer.Deserialize<List<oQuestion>>(json);
```

Zet JSON tekst automatisch om naar C# objecten:

**JSON:**
```json
[{"id":1,"question":"Hoofdstad?","options":["A","B"],"answer":0}]
```

**Wordt:**
```csharp
List<oQuestion> {
    new oQuestion { Id = 1, Question = "Hoofdstad?", ... }
}
```

### Uitbreidingsmogelijkheden WPF App

#### Voor Beginners:
1. Voeg een Label toe die het aantal correcte antwoorden bijhoudt
2. Verander de kleuren in de `BoolToColorConverter`
3. Voeg een "Volgende Vraag" knop toe
4. Toon het correcte antwoord na een foute keuze

#### Voor Gevorderden:
1. Implementeer een timer per vraag
2. Voeg een score systeem toe
3. Maak een resultaten pagina met statistieken
4. Implementeer een "Herstart Quiz" functie
5. Voeg animaties toe bij correcte/foute antwoorden

#### Challenge:
1. Implementeer Commands (ICommand) in plaats van event handlers
2. Voeg een tweede Window toe voor instellingen
3. Sla de highscores op in een lokaal bestand (JSON of XML)
4. Implementeer POST requests om nieuwe vragen toe te voegen aan de API
5. Maak de applicatie meertalig (Nederlands/Engels)

---

## Demo REST API Uitleg

De demo API in de map `restapi.local/` laat zien hoe een simpele REST API werkt.

### Bestandsstructuur

```
restapi.local/
├── .htaccess              # Apache configuratie voor URL rewriting
├── index.php              # Hoofdbestand - routeert verzoeken
├── demo_questions.php     # Bevat de quizvragen en logica
└── fetch_test.html        # Testpagina om de API te testen
```

### Hoe werkt de API?

#### 1. `.htaccess` - URL Rewriting

Dit bestand zorgt ervoor dat alle verzoeken naar niet-bestaande bestanden worden doorgestuurd naar `index.php`.

**Voorbeeld:**
- Je vraagt: `http://localhost/restapi.local/all`
- Apache stuurt dit door naar: `index.php` (met `/all` als pad)

#### 2. `index.php` - Router

Dit bestand bepaalt welke code moet worden uitgevoerd op basis van de URL:

```php
/all           → Geeft alle quizvragen terug
/id/1          → Geeft quizvraag met ID 1 terug
/id/2          → Geeft quizvraag met ID 2 terug
```

**Code uitleg:**
```php
if ($path === '/all') {
    include 'demo_questions.php';  // Alle vragen ophalen
} elseif (preg_match('#^/id/(\d+)$#', $path, $matches)) {
    $id = $matches[1];             // ID uit URL halen
    include 'demo_questions.php';  // Specifieke vraag ophalen
}
```

#### 3. `demo_questions.php` - Data & Logica

Dit bestand bevat:
- **Demo data** - Twee voorbeeldvragen over geografie en astronomie
- **GET logica** - Code om vragen op te halen en als JSON terug te geven

**Demo data structuur:**
```php
[
    "id" => 1,
    "question" => "Wat is de hoofdstad van Frankrijk?",
    "options" => ["Parijs", "Lyon", "Marseille", "Nice"],
    "answer" => 0  // Index van het juiste antwoord (0 = "Parijs")
]
```

**Headers:**
```php
header("Access-Control-Allow-Origin: *");        // Staat requests toe vanaf andere domeinen
header("Content-Type: application/json");         // Geeft aan dat we JSON terugsturen
```

#### 4. `fetch_test.html` - Test Pagina

Een simpele HTML pagina met JavaScript om de API te testen:

```javascript
fetch('/all')
    .then(response => response.json())
    .then(data => console.log(data))  // Quizvragen worden getoond in de console
```

**Hoe te gebruiken:**
1. Open de API in je browser: `http://localhost:8000/all`
2. Open de Developer Tools (F12)
3. Ga naar de Console tab
4. Je ziet de quizvragen als JSON

---

## API Endpoints

### Alle quizvragen ophalen
```
GET /all
```

**Response (200 OK):**
```json
[
    {
        "id": 1,
        "question": "Wat is de hoofdstad van Frankrijk?",
        "options": ["Parijs", "Lyon", "Marseille", "Nice"],
        "answer": 0
    },
    {
        "id": 2,
        "question": "Welke planeet staat het dichtst bij de zon?",
        "options": ["Aarde", "Venus", "Mercurius", "Mars"],
        "answer": 2
    }
]
```

### Specifieke quizvraag ophalen
```
GET /id/{id}
```

**Voorbeeld:**
```
GET /id/1
```

**Response (200 OK):**
```json
[
    {
        "id": 1,
        "question": "Wat is de hoofdstad van Frankrijk?",
        "options": ["Parijs", "Lyon", "Marseille", "Nice"],
        "answer": 0
    }
]
```

**Response als vraag niet bestaat (404 Not Found):**
```json
{
    "message": "Quiz question not found"
}
```

---

## REST API Concepten voor Beginners

### Wat is een REST API?

Een **REST API** is een manier om applicaties met elkaar te laten communiceren via het internet. Het werkt volgens vaste regels (HTTP protocollen).

**Simpel uitgelegd:**
- Je stuurt een **verzoek** (request) naar een URL
- De server verwerkt dit verzoek
- Je krijgt een **antwoord** (response) terug, meestal in JSON formaat

### HTTP Methods

Deze demo gebruikt alleen **GET** requests:

| Method | Doel | Voorbeeld |
|--------|------|-----------|
| GET | Data ophalen | `/all` geeft alle vragen |
| POST | Data toevoegen | Nieuwe vraag aanmaken (niet geïmplementeerd) |
| PUT | Data updaten | Vraag wijzigen (niet geïmplementeerd) |
| DELETE | Data verwijderen | Vraag verwijderen (niet geïmplementeerd) |

### HTTP Status Codes

De API gebruikt verschillende statuscodes:

| Code | Betekenis | Wanneer |
|------|-----------|---------|
| 200 | OK | Verzoek geslaagd |
| 404 | Not Found | Vraag met dit ID bestaat niet |
| 405 | Method Not Allowed | Verkeerde HTTP method gebruikt |

### JSON Formaat

JSON (JavaScript Object Notation) is een populair dataformaat voor API's.

**Voorbeeld:**
```json
{
    "id": 1,
    "question": "Wat is de hoofdstad van Frankrijk?",
    "options": ["Parijs", "Lyon", "Marseille", "Nice"],
    "answer": 0
}
```

**Uitleg:**
- `"id": 1` - Een nummer
- `"question": "..."` - Een tekst (string)
- `"options": [...]` - Een lijst (array)
- `"answer": 0` - Index van het juiste antwoord in de options array

---

## De API Testen

### Met de Browser
Open een van deze URLs in je browser:
- `http://localhost:8000/all` - Alle vragen
- `http://localhost:8000/id/1` - Vraag met ID 1
- `http://localhost:8000/id/2` - Vraag met ID 2
- `http://localhost:8000/id/999` - Niet bestaande vraag (geeft 404)

### Met de Developer Tools (F12)

1. Open de browser en druk op **F12**
2. Ga naar de **Network** tab
3. Bezoek een van de URLs hierboven
4. Klik op het request in de Network tab
5. Bekijk de **Response** tab om de JSON data te zien

### Met JavaScript (fetch)

```javascript
fetch('http://localhost:8000/all')
    .then(response => response.json())
    .then(data => {
        console.log(data);
        // Doe iets met de data
    })
    .catch(error => {
        console.error('Fout:', error);
    });
```

### Met Postman of Insomnia

Download een API testing tool zoals [Postman](https://www.postman.com/) of [Insomnia](https://insomnia.rest/):

1. Open de tool
2. Maak een nieuw GET request
3. Voer de URL in: `http://localhost:8000/all`
4. Klik op "Send"
5. Bekijk de response

---

## Uitbreidingen en Oefeningen

### Voor Beginners:
1. Voeg een nieuwe quizvraag toe aan de array in `demo_questions.php`
2. Test of je de nieuwe vraag kunt ophalen via de API
3. Wijzig de vraag of antwoorden van een bestaande quiz

### Voor Gevorderden:
1. Voeg een nieuw endpoint toe: `/random` - geeft een willekeurige vraag terug
2. Implementeer een `/count` endpoint - geeft het totaal aantal vragen terug
3. Voeg een `category` veld toe aan elke vraag en maak een filter: `/category/{naam}`

### Challenge:
1. Verbind de WPF applicatie met de demo API
2. Toon de vragen in de interface
3. Laat gebruikers antwoorden selecteren en geef feedback

---

## Problemen Oplossen

### "404 - Pagina niet gevonden" bij /all

**Oplossing:**
- Controleer of de webserver draait (XAMPP Apache of PHP built-in server)
- Controleer of `.htaccess` correct is (alleen voor Apache/XAMPP)
- Voor PHP built-in server: gebruik `http://localhost:8000/all` (met poortnummer)

### JSON wordt niet goed weergegeven

**Oplossing:**
- Installeer een JSON Viewer extensie in je browser (bijv. "JSON Formatter" voor Chrome)
- Of gebruik de Network tab in Developer Tools (F12)

### CORS Errors bij fetch requests

**Oplossing:**
- Dit is al opgelost met de header in `demo_questions.php`:
  ```php
  header("Access-Control-Allow-Origin: *");
  ```
- Als het nog steeds niet werkt, zorg dat je de API draait op dezelfde server

### WPF Applicatie start niet

**Oplossing:**
- Controleer of .NET 8.0 SDK is geïnstalleerd
- Rebuild de solution (Ctrl + Shift + B)
- Check de Error List in Visual Studio voor specifieke fouten

---

## Leerdoelen

Na het bestuderen van dit project zou je moeten begrijpen:

1. **Wat een REST API is** en hoe het werkt
2. **Hoe HTTP requests en responses werken**
3. **JSON data structuren** lezen en begrijpen
4. **URL routing** en hoe paden worden verwerkt
5. **HTTP status codes** en wat ze betekenen
6. **CORS** (Cross-Origin Resource Sharing) basics
7. **Hoe een WPF applicatie werkt** met .NET

---

## Handige Links

### Documentatie:
- [PHP Documentatie](https://www.php.net/manual/nl/)
- [.NET WPF Documentatie](https://learn.microsoft.com/nl-nl/dotnet/desktop/wpf/)
- [MDN - HTTP Methods](https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods)
- [JSON Introductie](https://www.json.org/json-nl.html)

### Tools:
- [XAMPP Download](https://www.apachefriends.org/)
- [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
- [Postman](https://www.postman.com/)
- [Visual Studio Code](https://code.visualstudio.com/)

---

## Licentie

Dit is een educatief project voor leerdoeleinden.

## Vragen?

Heb je vragen over dit project? Vraag het aan je docent of medestudenten!

**Veel succes met leren!**
