using System.ComponentModel.DataAnnotations;

namespace EncuentraTuHogar.Domain.ValueObjects;

public enum TransactionType
{
    [Display(Name = "Arriendo")]
    Rent = 1,
    [Display(Name = "Venta")]
    Sale = 2
}
