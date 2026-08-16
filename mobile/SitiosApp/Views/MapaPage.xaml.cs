using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using SitiosApp.Models;

namespace SitiosApp.Views
{
    public partial class MapaPage : ContentPage
    {
        public MapaPage(Sitio sitio)
        {
            InitializeComponent();

            var posicion = new Location(sitio.Latitud, sitio.Longitud);

            MapaSitio.MoveToRegion(MapSpan.FromCenterAndRadius(posicion, Distance.FromMeters(500)));

            MapaSitio.Pins.Add(new Pin
            {
                Label = $"Pin: {sitio.Descripcion}",
                Location = posicion,
                Type = PinType.Place
            });
        }

        private async void OnAtrasClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
