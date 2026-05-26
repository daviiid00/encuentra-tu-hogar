using System.ComponentModel.DataAnnotations;

namespace EncuentraTuHogar.Domain.ValueObjects;

public enum PropertyType
{
    [Display(Name = "Apartamento")]
    Apartment = 1,
    [Display(Name = "Casa")]
    House = 2,
    [Display(Name = "Estudio")]
    Studio = 3,
    [Display(Name = "Habitación")]
    Room = 4,
    [Display(Name = "Loft")]
    Loft = 5,
    [Display(Name = "Townhouse")]
    Townhouse = 6
}
