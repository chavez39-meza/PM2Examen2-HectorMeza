using Plugin.Maui.Audio;
using SitiosApp.Models;
using SitiosApp.Services;

namespace SitiosApp.Views
{
    public partial class UbicacionesSalvadasPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<Sitio> _sitios = new();
        private Sitio? _seleccionado;

        public UbicacionesSalvadasPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarSitiosAsync();
        }

        // ---------------- OBTENER INFO (GET) ----------------
        private async Task CargarSitiosAsync()
        {
            try
            {
                _sitios = await _apiService.GetSitiosAsync();
                ListaSitios.ItemsSource = _sitios;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", $"No se pudo cargar la lista: {ex.Message}", "OK");
            }
            finally
            {
                RefreshViewLista.IsRefreshing = false;
            }
        }

        private async void OnRefrescarClicked(object sender, EventArgs e) => await CargarSitiosAsync();

        private void OnSitioSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            _seleccionado = e.CurrentSelection.FirstOrDefault() as Sitio;
        }

        // ---------------- ELIMINAR / ACTUALIZAR ----------------
        private async void OnEliminarActualizarClicked(object sender, EventArgs e)
        {
            if (_seleccionado is null)
            {
                await DisplayAlert("Alerta", "Selecciona una ubicación de la lista.", "OK");
                return;
            }

            string accion = await DisplayActionSheet(
                $"'{_seleccionado.Descripcion}'", "Cancelar", null, "Actualizar", "Eliminar");

            if (accion == "Eliminar")
            {
                bool confirmar = await DisplayAlert("Confirmar", "¿Eliminar esta ubicación?", "Sí", "No");
                if (!confirmar) return;

                var ok = await _apiService.DeleteSitioAsync(_seleccionado.Id);
                if (ok)
                {
                    await DisplayAlert("Éxito", "Ubicación eliminada.", "OK");
                    await CargarSitiosAsync();
                }
                else
                {
                    await DisplayAlert("Alerta", "No se pudo eliminar.", "OK");
                }
            }
            else if (accion == "Actualizar")
            {
                string? nuevaDescripcion = await DisplayPromptAsync(
                    "Actualizar descripción", "Nueva descripción:", initialValue: _seleccionado.Descripcion);

                if (string.IsNullOrWhiteSpace(nuevaDescripcion))
                {
                    await DisplayAlert("Alerta", "debe describir la ubicación", "OK");
                    return;
                }

                var dto = new SitioCreateDto
                {
                    Descripcion = nuevaDescripcion.Trim(),
                    Latitud = _seleccionado.Latitud,
                    Longitud = _seleccionado.Longitud
                };

                var ok = await _apiService.UpdateSitioAsync(_seleccionado.Id, dto);
                if (ok)
                {
                    await DisplayAlert("Éxito", "Ubicación actualizada.", "OK");
                    await CargarSitiosAsync();
                }
                else
                {
                    await DisplayAlert("Alerta", "No se pudo actualizar.", "OK");
                }
            }
        }

        // ---------------- VER MAPA ----------------
        private async void OnVerMapaClicked(object sender, EventArgs e)
        {
            if (_seleccionado is null)
            {
                await DisplayAlert("Alerta", "Selecciona una ubicación de la lista.", "OK");
                return;
            }

            await Navigation.PushAsync(new MapaPage(_seleccionado));
        }

        // ---------------- ESCUCHAR AUDIO ----------------
        private async void OnEscucharAudioClicked(object sender, EventArgs e)
        {
            if (_seleccionado is null)
            {
                await DisplayAlert("Alerta", "Selecciona una ubicación de la lista.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_seleccionado.AudioBase64))
            {
                await DisplayAlert("Alerta", "Esta ubicación no tiene nota de voz.", "OK");
                return;
            }

            try
            {
                var bytes = Convert.FromBase64String(_seleccionado.AudioBase64);
                var stream = new MemoryStream(bytes);

                var player = AudioManager.Current.CreatePlayer(stream);
                player.Play();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", $"No se pudo reproducir el audio: {ex.Message}", "OK");
            }
        }

        private async void OnAtrasClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
