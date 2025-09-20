using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LedBlinker.Data;
using LedBlinker.Models;
using System.Globalization;

namespace LedBlinker.Controllers
{
    public class LedViewController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LedViewController(ApplicationDbContext context)
        {
            _db = context;
        }

        private LedStateViewModel GetFullViewModel()
        {
            var led = _db.Leds.FirstOrDefault();
            var logs = _db.Logs.ToList();
            var blinkRate = _db.Configurations.Select(x => x.BlinkRate).FirstOrDefault();

            return new LedStateViewModel
            {
                CurrentState = led?.State ?? LedState.Off,
                SelectedState = led?.State ?? LedState.Off,
                Logs = logs,
                CurrentBlinkRate = blinkRate,
                NewBlinkRate = blinkRate
            };
        }

        // GET: Zobrazí formulář i aktuální stav
        public IActionResult State()
        {
            Console.WriteLine("GET: State metoda spuštěna");

            var model = GetFullViewModel();
            Console.WriteLine($" current blink rate {model.CurrentBlinkRate}");
            Console.WriteLine($" current blink rate {model.NewBlinkRate}");
            return View("State", model);
        }

        [HttpPost]
        public IActionResult State([Bind("SelectedState")] LedStateViewModel model)
        {
            Console.WriteLine("POST: State metoda spuštěna");

            if (!Enum.IsDefined(typeof(LedState), model.SelectedState))
            {
                ModelState.AddModelError("", "Zadej on, off nebo blinking");
                var fullModel = GetFullViewModel();
                fullModel.SelectedState = model.SelectedState;
                return View("State", fullModel);
            }

            var led = _db.Leds.FirstOrDefault();
            if (led == null)
                return NotFound("Žádná LED není vytvořená");

            
            if (led.State != model.SelectedState)
            {
                led.State = model.SelectedState;

                _db.Logs.Add(new Logs
                {
                    Date = DateTime.Now,
                    State = led.State.ToString()
                });

                _db.SaveChanges();
            }

            return RedirectToAction("State");
        }

        [HttpPost]
        public IActionResult DeleteLogs()
        {
            Console.WriteLine("POST: DeleteLogs metoda spuštěna");

            _db.Logs.RemoveRange(_db.Logs);
            _db.SaveChanges();

            return RedirectToAction("State");
        }

        [HttpGet]
        public IActionResult Blink()
        {
            Console.WriteLine("GET: Blink metoda spuštěna");

            var config = _db.Configurations.FirstOrDefault();
            var model = new LedStateViewModel
            {
                NewBlinkRate = config?.BlinkRate ?? 1
            };
            return View("Blink", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostBlinkRate(float NewBlinkRate)
        {
            Console.WriteLine("POST: PostBlinkRate metoda spuštěna, hodnota: " + NewBlinkRate);

            if (NewBlinkRate < 0.1f || NewBlinkRate > 10f)
            {
                ModelState.AddModelError("NewBlinkRate", "Zadej hodnotu v rozmezí 0.1 až 10.");
                var fullModel = GetFullViewModel();
                fullModel.NewBlinkRate = NewBlinkRate;
                return View("State", fullModel);
            }

            var config = _db.Configurations.FirstOrDefault();
            if (config == null)
            {
                config = new Configuration { BlinkRate = NewBlinkRate, ConfigurationLed = new() };
                _db.Configurations.Add(config);
            }
            else
            {
                config.BlinkRate = NewBlinkRate;
            }
            _db.SaveChanges();

            var updatedModel = GetFullViewModel();
            return View("State", updatedModel);
        }
    }
}
