# LedBlinker

Malý demonstrační projekt pro ovládání LEDky (On, Off, Blinking) přes webové rozhraní. Používá ASP.NET Core MVC.

## 💡 Požadavky

- [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
  > ⚠️ Upozornění: .NET 6.0 je **End of Support** (Microsoft jej již oficiálně nepodporuje). Pokud používáš novější verzi (.NET 8.0+), může být nutné projekt ručně přenastavit nebo migrovat.
- Git
- Webový prohlížeč

## 🔧 Spuštění projektu

1. Klonuj repozitář:
   ```bash
   git clone https://github.com/VeronikaBendlova1/LedBlinker.git
````

2. Přejdi do složky projektu:

   ```bash
   cd LedBlinker
   ```

3. Spusť aplikaci:

   ```bash
   dotnet run
   ```

4. Otevři v prohlížeči: `https://localhost:5001`

## 📦 Nestandardní závislosti

Projekt běží na .NET 6.0, který již není oficiálně podporovaný. Pro spuštění je nutné mít tento runtime lokálně nainstalovaný.

### Hlavní použité balíčky:

- `Microsoft.AspNetCore.Mvc` – Webový framework MVC
- `Microsoft.EntityFrameworkCore.Sqlite` a `SqlServer` – Podpora pro databáze
- `Microsoft.EntityFrameworkCore.Tools` a `Design` – EF Core tooling (např. migrace)
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` – Podpora pro scaffolding
- `Swashbuckle.AspNetCore.*` – Integrovaný **Swagger UI** pro REST API dokumentaci
- `Newtonsoft.Json` – Práce s JSON
- `System.ComponentModel.Annotations` – Validace modelů
- `System.Net.Http` – HTTP klient (většinou automaticky v .NET)

> Pokud budeš projekt předělávat na novější .NET (např. 8.0), doporučuji zkontrolovat kompatibilitu jednotlivých balíčků.

## 📁 Struktura projektu

* `Controllers/` – Logika ovládání LED
* `Models/` – Enum a datové modely (např. `LedState`)
* `Views/` – Razor stránky pro UI

## 📝 Licence

Projekt je určen pro studijní a výukové účely. Není určen pro produkční nasazení.
