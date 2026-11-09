
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Zegar_Pogoda;

public partial class Etap2View : UserControl
{
    private static readonly HttpClient Http = new();

    private readonly DispatcherTimer _timer;

    public Etap2View()
    {
        InitializeComponent();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += (_, _) => OdswiezZegar();

        OdswiezZegar();
        _timer.Start();
    }

    private void OdswiezZegar()
    {
        txtGodzina.Text = DateTime.Now.ToString(
            "HH:mm:ss",
            CultureInfo.InvariantCulture
        );
    }

    private async void BtnPogoda_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is Button przycisk &&
            przycisk.Tag is string adres)
        {
            przycisk.IsEnabled = false;

            try
            {
                await PobierzAsync(adres);
            }
            finally
            {
                przycisk.IsEnabled = true;
            }
        }
    }

    private async Task PobierzAsync(string adres)
    {
        txtWynik.Text = "Pobieram pogodę...";
        txtPobrano.Text = "";

        try
        {
            string json = await Http.GetStringAsync(adres);

            using JsonDocument dokument =
                JsonDocument.Parse(json);

            JsonElement current = dokument.RootElement
                .GetProperty("current");

            double temperatura = current
                .GetProperty("temperature_2m").GetDouble();

            double odczuwalna = current
                .GetProperty("apparent_temperature").GetDouble();

            int wilgotnosc = current
                .GetProperty("relative_humidity_2m").GetInt32();

            double wiatr = current
                .GetProperty("wind_speed_10m").GetDouble();

            int kod = current
                .GetProperty("weather_code").GetInt32();

            string opis = OpisPogody(kod);

            txtWynik.Text =
                $"Temperatura: {temperatura:F1} °C\n\n" +
                $"Odczuwalna: {odczuwalna:F1} °C\n\n" +
                $"Warunki: {opis}\n\n" +
                $"Wilgotność: {wilgotnosc}%\n\n" +
                $"Wiatr: {wiatr:F1} km/h";

            txtPobrano.Text =
                $"Ostatnia aktualizacja: {DateTime.Now:HH:mm:ss}";
        }
        catch (HttpRequestException)
        {
            txtWynik.Text =
                "Nie można połączyć się z serwerem pogodowym. Sprawdź internet.";
        }
        catch (JsonException)
        {
            txtWynik.Text =
                "Błąd odczytu danych pogodowych.";
        }
        catch (Exception ex)
        {
            txtWynik.Text =
                $"Wystąpił błąd: {ex.Message}";
        }
    }

    private static string OpisPogody(int kod)
    {
        return kod switch
        {
            0 => "Bezchmurnie",
            1 => "Przeważnie bezchmurnie",
            2 => "Częściowe zachmurzenie",
            3 => "Pochmurno",

            45 or 48 => "Mgła",

            51 or 53 or 55 => "Mżawka",
            56 or 57 => "Marznąca mżawka",

            61 => "Lekki deszcz",
            63 => "Umiarkowany deszcz",
            65 => "Silny deszcz",

            66 or 67 => "Marznący deszcz",

            71 => "Lekki śnieg",
            73 => "Umiarkowany śnieg",
            75 => "Silny śnieg",
            77 => "Ziarnisty śnieg",

            80 => "Przelotny deszcz",
            81 => "Umiarkowane przelotne opady",
            82 => "Silne przelotne opady",

            85 or 86 => "Przelotny śnieg",

            95 => "Burza",
            96 or 99 => "Burza z gradem",

            _ => "Nieznane warunki pogodowe"
        };
    }
}
