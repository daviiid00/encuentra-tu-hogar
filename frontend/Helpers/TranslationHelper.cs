namespace EncuentraTuHogar.Frontend.Helpers;

public static class TranslationHelper
{
    public static string TranslatePropertyType(string type)
    {
        return type switch
        {
            "Apartment" => "Apartamento",
            "House" => "Casa",
            "Studio" => "Estudio",
            "Room" => "Habitación",
            "Loft" => "Loft",
            "Townhouse" => "Townhouse",
            _ => type
        };
    }

    public static string TranslateTransactionType(string transaction)
    {
        return transaction switch
        {
            "Rent" => "Arriendo",
            "Sale" => "Venta",
            _ => transaction
        };
    }
}
