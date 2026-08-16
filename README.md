# Examen Segundo Parcial — Programación Móvil II

Proyecto completo: **Backend REST API en C# (Clean Architecture)** + **App móvil .NET MAUI**.

---

## 📁 Estructura

```
SitiosExamen/
├── backend/                     ← API REST (ASP.NET Core)
│   └── src/
│       ├── SitiosApi.Domain/         (entidad Sitio, sin dependencias)
│       ├── SitiosApi.Application/    (DTOs, interfaces, lógica de negocio)
│       ├── SitiosApi.Infrastructure/ (EF Core + SQLite, implementa los repos)
│       └── SitiosApi.Api/            (Controllers, Program.cs, arranque)
│
└── mobile/                      ← App .NET MAUI (Android)
    └── SitiosApp/
        ├── Models/                   (Sitio, DTOs)
        ├── Services/ApiService.cs    (llamadas HTTP al backend)
        ├── Views/
        │   ├── NuevaUbicacionPage    (pantalla 1: foto, GPS, audio, guardar)
        │   ├── UbicacionesSalvadasPage (pantalla 2: lista, eliminar/actualizar)
        │   └── MapaPage               (pantalla 3: mapa con pin)
        └── Platforms/Android/        (permisos, MainActivity)
```

Por qué es "Clean Architecture": **Domain** no depende de nada. **Application**
solo depende de Domain y define *interfaces* (`ISitioRepository`) sin saber
cómo se implementan. **Infrastructure** implementa esas interfaces con EF Core.
**Api** conecta todo con inyección de dependencias en `Program.cs`. Si te
preguntan "¿por qué es clean architecture?", esa es la respuesta corta.

---

## 🚀 Cómo correrlo

### 1. Backend

Requisitos: **.NET 8 SDK**, Visual Studio 2022 (o `dotnet` CLI).

```bash
cd backend
dotnet restore
dotnet run --project src/SitiosApi.Api
```

Esto:
- Crea automáticamente `sitios.db` (SQLite) la primera vez que corre.
- Levanta la API en `http://localhost:5100` (revisa `Properties/launchSettings.json`).
- Abre Swagger en `http://localhost:5100/swagger` para probar los endpoints
  a mano (GET, POST, PUT, DELETE) **antes** de conectar el celular — así
  sabes si el problema es el backend o la app.

### 2. App móvil (.NET MAUI)

Requisitos: Visual Studio 2022 con el workload **".NET Multi-platform App UI
development"** instalado (Herramientas → Obtener herramientas y características).

```bash
cd mobile
dotnet restore
```

Ábrelo en Visual Studio (`SitiosApp.sln`), selecciona el emulador de Android
y dale Run (F5).

**Muy importante — URL de la API** (`Services/ApiService.cs`, línea `BaseUrl`):

| Dónde corres la app          | URL a usar                          |
|-------------------------------|--------------------------------------|
| Emulador de Android           | `http://10.0.2.2:5100/api/`         |
| Celular físico (misma WiFi)   | `http://TU_IP_LOCAL:5100/api/`      |
| Windows (WinUI)               | `http://localhost:5100/api/`        |

El emulador de Android **no** puede usar `localhost` para llegar a tu PC —
`10.0.2.2` es la dirección especial que el emulador usa para referirse a tu
máquina anfitriona. Si usas un celular físico, necesitas la IP local de tu PC
(`ipconfig` en Windows) y que el celular esté en la misma red WiFi.

### 3. Mapa (pantalla 3)

`Microsoft.Maui.Controls.Maps` en Android necesita una **API Key de Google
Maps**. Pasos:
1. Consigue una key gratis en https://console.cloud.google.com/ (habilita
   "Maps SDK for Android").
2. Agrégala en `Platforms/Android/AndroidManifest.xml` dentro de `<application>`:
   ```xml
   <meta-data android:name="com.google.android.geo.API_KEY" android:value="TU_API_KEY" />
   ```
   Si no tienes tiempo de sacar la key antes del examen, dilo — el resto de
   la app (CRUD, foto, GPS, audio) funciona igual sin el mapa.

---

## ✅ Checklist contra la rúbrica de la pantalla 1

| Requisito                          | Dónde está en el código                              |
|-------------------------------------|-------------------------------------------------------|
| Subir datos al CRUD (CREATE)        | `NuevaUbicacionPage.xaml.cs` → `OnSalvarUbicacionClicked` → `ApiService.CreateSitioAsync` → `SitiosController.Create` |
| Validar conexión a internet y GPS   | `OnObtenerUbicacionClicked` + `Connectivity.Current.NetworkAccess` |
| Toma de fotografía                  | `OnTomarFotoClicked` → `MediaPicker.CapturePhotoAsync` |
| Grabar audio                        | `OnGrabarAudioClicked` → `Plugin.Maui.Audio` (`IAudioRecorder`) |
| Validaciones sobre datos            | `OnSalvarUbicacionClicked` (descripción vacía / muy corta) |

Y el resto del examen:

| Requisito                          | Dónde está |
|--------------------------------------|------------|
| CRUD completo en C# / Clean Architecture | Carpeta `backend/` completa |
| GET                                  | `SitiosController.GetAll` / `GetById` |
| POST (actualizar, según enunciado)   | `SitiosController.Update` (mapeado a PUT **y** POST) |
| DELETE                               | `SitiosController.Delete` |
| Pantalla lista + eliminar/actualizar/ver mapa/escuchar audio | `UbicacionesSalvadasPage` |
| Pantalla de mapa con pin             | `MapaPage` |

---

## 🎤 Si te preguntan en el examen (para defenderlo)

- **"¿Por qué Base64 y no un archivo?"** — Así se puede mandar la foto/audio
  dentro del mismo JSON del POST, sin necesitar un endpoint de subida de
  archivos aparte. Es más simple para un CRUD REST estándar.
- **"¿Qué es la inversión de dependencias aquí?"** — `SitioService` (en
  Application) depende de `ISitioRepository` (una interfaz), no de
  `SitioRepository` (la clase concreta en Infrastructure). Quien decide qué
  implementación usar es `Program.cs`, con `AddScoped<ISitioRepository,
  SitioRepository>()`.
- **"¿Por qué el GPS puede fallar?"** — Si el usuario tiene el GPS apagado,
  `Geolocation.GetLocationAsync` lanza `FeatureNotEnabledException`; por eso
  el código la atrapa y muestra la alerta "Gps no esta activo" en vez de
  crashear.

---

## ⚠️ Nota honesta

Este código fue escrito directamente (no se compiló en este entorno porque no
tiene el SDK de .NET/Android). Sigue los patrones estándar de ASP.NET Core 8
y .NET MAUI 8, pero **corre `dotnet build` en tu máquina lo antes posible**
(no esperes al último momento) para atrapar cualquier typo o versión de
paquete que necesite ajuste. Si algo no compila, pégame el error exacto y lo
arreglamos juntos.
