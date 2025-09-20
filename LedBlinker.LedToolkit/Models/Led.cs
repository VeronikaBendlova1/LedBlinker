using System.ComponentModel.DataAnnotations;

namespace LedBlinker.LedToolkit.Models
{
    public class Led
    {
        public int Id { get; set; }
        public LedState State { get; set; }
    }

    public enum LedState
    {
        Off,
        On,
        Blinking
    }

    public class Logs
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string State { get; set; }
    }

    public class Configuration
    {
        public int Id { get; set; }

        [Range(0.1, 10, ErrorMessage = "Zadej hodnotu v sekundách mezi 0.1 a 10.")]
        public float BlinkRate { get; set; }

        public Led ConfigurationLed { get; set; } //NV


    }

    // ✅ DTO pro změnu stavu LED
    public class LedStateDto
    {
        [Required]
        public LedState State { get; set; }
    }

    // ✅ DTO pro změnu blikání
    public class ConfigurationDto
    {
        [Required]
        [Range(0.1, 10, ErrorMessage = "BlinkRate musí být mezi 0.1 a 10")]
        public float BlinkRate { get; set; }
    }

    public class LedStateViewModel
    {
        public LedState SelectedState { get; set; }  // pro radio buttony
        public LedState CurrentState { get; set; }   // pro zobrazení aktuálního stavu
        public List<Logs> Logs { get; set; }
        [Range(0.1, 10, ErrorMessage = "Zadej hodnotu v sekundách mezi 0.1 a 10.")]
        public float NewBlinkRate { get; set; }
        public float CurrentBlinkRate { get; set; }
    }
}
