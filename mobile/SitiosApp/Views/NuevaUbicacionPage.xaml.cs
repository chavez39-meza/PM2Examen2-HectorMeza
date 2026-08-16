using System.Text;
using Plugin.Maui.Audio;
using SitiosApp.Models;
using SitiosApp.Services;

namespace SitiosApp.Views
{
    public partial class NuevaUbicacionPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private readonly IAudioManager _audioManager = AudioManager.Current;

        private string? _fotoBase64;
        private string? _audioBase64;
        private double? _latitud;
        private double? _longitud;

        private IAudioRecorder? _recorder;
        private bool _grabando;

        public NuevaUbicacionPage()
        {
            InitializeComponent();
        }

        // ---------------- FOTOGRAFÍA (2%) ----------------
        private async void OnTomarFotoClicked(object sender, EventArgs e)
        {
            try
            {
                var estadoCamara = await Permissions.RequestAsync<Permissions.Camera>();
                if (estadoCamara != PermissionStatus.Granted)
                {
                    await DisplayAlert("Alerta", "Se necesita permiso de cámara.", "OK");
                    return;
                }

                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlert("Alerta", "Este dispositivo no soporta cámara.", "OK");
                    return;
                }

                FileResult? foto = await MediaPicker.Default.CapturePhotoAsync();
                if (foto is null) return;

                using var stream = await foto.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                _fotoBase64 = Convert.ToBase64String(bytes);
                ImgFoto.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", $"No se pudo tomar la foto: {ex.Message}", "OK");
            }
        }

        // ---------------- GPS + CONEXIÓN A INTERNET (1%) ----------------
        private async void OnObtenerUbicacionClicked(object sender, EventArgs e)
        {
            // Validar conexión a internet
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alerta", "No hay conexión a internet.", "OK");
                return;
            }

            // Validar que el GPS esté activo
            var location = await ObtenerUbicacionGpsAsync();
            if (location is null)
            {
                await DisplayAlert("Alerta", "Gps no esta activo", "OK");
                return;
            }

            _latitud = location.Latitude;
            _longitud = location.Longitude;
            EntryLatitud.Text = location.Latitude.ToString("F5");
            EntryLongitud.Text = location.Longitude.ToString("F5");
        }

        private async Task<Location?> ObtenerUbicacionGpsAsync()
        {
            try
            {
                var estado = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (estado != PermissionStatus.Granted) return null;

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                return await Geolocation.Default.GetLocationAsync(request);
            }
            catch (FeatureNotEnabledException)
            {
                // El GPS del dispositivo está apagado
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // ---------------- GRABAR AUDIO (1%) ----------------
        private async void OnGrabarAudioClicked(object sender, EventArgs e)
        {
            var estadoMic = await Permissions.RequestAsync<Permissions.Microphone>();
            if (estadoMic != PermissionStatus.Granted)
            {
                await DisplayAlert("Alerta", "Se necesita permiso de micrófono.", "OK");
                return;
            }

            if (!_grabando)
            {
                // Iniciar grabación
                _recorder = _audioManager.CreateRecorder();
                await _recorder.StartAsync();
                _grabando = true;
                BtnGrabar.Text = "⏹ Detener";
                LblEstadoAudio.Text = "Grabando...";
            }
            else
            {
                // Detener grabación
                var resultado = await _recorder!.StopAsync();
                _grabando = false;
                BtnGrabar.Text = "🎤 Grabar";

                using var stream = resultado.GetAudioStream();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                _audioBase64 = Convert.ToBase64String(bytes);
                LblEstadoAudio.Text = $"Nota de voz grabada ({bytes.Length / 1024} KB)";
            }
        }

        // ---------------- VALIDACIONES + GUARDAR (CREATE, 1%) ----------------
        private async void OnSalvarUbicacionClicked(object sender, EventArgs e)
        {
            // Validación: debe describir la ubicación
            if (string.IsNullOrWhiteSpace(EditorDescripcion.Text))
            {
                await DisplayAlert("Alerta", "debe describir la ubicación", "OK");
                return;
            }

            // Validación: descripción no puede ser demasiado corta
            if (EditorDescripcion.Text.Trim().Length < 5)
            {
                await DisplayAlert("Alerta", "debe escribir una ubicación corta", "OK");
                return;
            }

            // Validación: debe tener coordenadas GPS
            if (_latitud is null || _longitud is null)
            {
                await DisplayAlert("Alerta", "Gps no esta activo", "OK");
                return;
            }

            // Validación de conexión a internet antes de llamar al API
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alerta", "No hay conexión a internet.", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                BtnGuardar.IsEnabled = false;

                var dto = new SitioCreateDto
                {
                    Descripcion = EditorDescripcion.Text.Trim(),
                    Latitud = _latitud.Value,
                    Longitud = _longitud.Value,
                    FotografiaBase64 = _fotoBase64,
                    AudioBase64 = _audioBase64
                };

                var creado = await _apiService.CreateSitioAsync(dto);

                if (creado is not null)
                {
                    await DisplayAlert("Éxito", "Ubicación guardada correctamente.", "OK");
                    LimpiarFormulario();
                }
                else
                {
                    await DisplayAlert("Alerta", "No se pudo guardar la ubicación.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", $"Error al conectar con el servidor: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                BtnGuardar.IsEnabled = true;
            }
        }

        private void LimpiarFormulario()
        {
            EditorDescripcion.Text = string.Empty;
            EntryLatitud.Text = string.Empty;
            EntryLongitud.Text = string.Empty;
            ImgFoto.Source = null;
            LblEstadoAudio.Text = "Sin grabar";
            _fotoBase64 = null;
            _audioBase64 = null;
            _latitud = null;
            _longitud = null;
        }

        private async void OnVerUbicacionesSalvadasClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UbicacionesSalvadasPage());
        }
    }
}
