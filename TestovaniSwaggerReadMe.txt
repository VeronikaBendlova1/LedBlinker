# 🧪 LED Blinker API – Uživatelský manuál k testování

Tento návod ti pomůže krok za krokem otestovat všechny dostupné metody `LedControlleru`. Můžeš testovat pomocí **Postmanu/Swaggeru**, **curl**, nebo jiného HTTP klienta.

## 🌐 Základní informace

* **Base URL:** `https://localhost:5001/api/led` 
* **Formát:** JSON
* **Požadavky:** spuštěná aplikace s `ApplicationDbContext`, databáze inicializovaná

---

## ✅ 1. GET `/api/led/state` – Získání aktuálního stavu LED

📌 *Vrací stav LEDky (např. Off, On, Blinking). Pokud žádná LEDka není, vytvoří se s výchozím stavem.*

### ❓ Jak testovat

```http
GET /api/led/state
```

### ✅ Očekávaný výstup

```json
"Off" nebo 0
```

---

## ✅ 2. POST `/api/led/state` – Nastavení stavu LED

📌 *Změní stav LEDky a uloží log.*

### 🟦 JSON vstup

```json
{
  "state": "Blinking"
}
```

📌 Možné hodnoty: `"Off"`, `"On"`, `"Blinking"`

### ❓ Jak testovat

```http
POST /api/led/state
Content-Type: application/json
```

### ✅ Očekávaný výstup

```json
{
  "id": 1,
  "state": "Blinking"
}
```

---

## ✅ 3. GET `/api/led/logs?from=2025-08-01&to=2025-08-04` – Získání logů

📌 *Vrací seznam změn stavu LEDky mezi zadanými daty. Parametry jsou nepovinné.*

### ❓ Jak testovat

```http
GET /api/led/logs
```

nebo s parametry:

```
GET /api/led/logs?from=2025-08-01&to=2025-08-04
```

### ✅ Očekávaný výstup

```json
[
  {
    "id": 1,
    "date": "2025-08-04T12:34:56",
    "state": "On"
  }
]
```

---

## ✅ 4. POST `/api/led/configuration` – Nastavení blikání

📌 *Nastaví rychlost blikání, pouze pokud LEDka bliká (tj. `state` == `Blinking`).*

### 🟦 JSON vstup

```json
{
  "blinkRate": 5
}
```

📌 Hodnota `blinkRate` musí být > 0 a ≤ 10.

### ❓ Jak testovat

1. Nejprve přepni LEDku na `Blinking` pomocí POST `/api/led/state`
2. Pak zavolej:

```http
POST /api/led/configuration
Content-Type: application/json
```

### ✅ Očekávaný výstup

```json
5
```

---

## ✅ 5. GET `/api/led/configuration` – Získání aktuálního `BlinkRate`

📌 *Vrací nastavenou hodnotu `BlinkRate` (pokud existuje).*

### ❓ Jak testovat

```http
GET /api/led/configuration
```

### ✅ Očekávaný výstup

```json
{
  "blinkRate": 5
}
```



## 🔍 Chyby a co znamenají

| Kód | Význam                                                            |
| --- | ----------------------------------------------------------------- |
| 400 | Špatný vstup – např. neplatná hodnota `state` nebo `blinkRate`    |
| 404 | Nenalezena LEDka – zavolej nejdřív GET `/state`, aby se vytvořila |
| 200 | Vše OK                                                            |

---


