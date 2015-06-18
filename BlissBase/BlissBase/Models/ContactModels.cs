using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BlissBase.Models
{
    public class ContactModels{

        [Required(ErrorMessage = "Fornavn må oppgis")]
        [StringLength(50, ErrorMessage = "Maks 50 tegn i fornavn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Etternavn må oppgis")]
        [StringLength(50, ErrorMessage = "Maks 50 tegn i etternavn")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-mailadresse må oppgis")]
        [StringLength(50, ErrorMessage = "Maks 50 tegn E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Spørsmål må oppgis")]
        [StringLength(200, ErrorMessage = "Maks 200 tegn per spørsmål.")]
        public string Comment { get; set; }
    }
}