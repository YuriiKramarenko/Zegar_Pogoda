using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Zegar_Pogoda;

public partial class Etap1View : UserControl
{
    private static readonly CultureInfo Pl = new("pl-PL");

    private readonly DispatcherTimer _timer;

    public Etap1View()
    {
        InitializeComponent();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => Odswiez();


        Odswiez();
        _timer.Start();
    }

    private void Odswiez()
    {
        DateTime teraz = DateTime.Now;
        txtGodzina.Text = teraz.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

        txtData.Text = teraz.ToString("dddd, d MMMM yyyy", Pl);
    }
}
