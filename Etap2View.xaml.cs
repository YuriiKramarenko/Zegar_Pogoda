using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Zegar_Pogoda;

public partial class Etap2View : UserControl
{
    private static readonly HttpClient http = new();

    public Etap2View()
    {
        InitializeComponent();

        DispatcherTimer timer = new()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        timer.Tick += (s, e) =>
            txtGodzina.Text = DateTime.Now.ToString("HH:mm:ss");

        txtGodzina.Text = DateTime.Now.ToString("HH:mm:ss");

        timer.Start();
    }

    private async void BtnPogoda_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string adres)
            return;

        button.IsEnabled = false;
        txtWynik.Text = "Pobieranie danych...";

        try
        {
            string json = await http.GetStringAsync(adres);

            txtWynik.Text = json;
        }
        catch (Exception ex)
        {
            txtWynik.Text = "Błąd pobierania danych: " + ex.Message;
        }
        finally
        {
            button.IsEnabled = true;
        }
    }
}
