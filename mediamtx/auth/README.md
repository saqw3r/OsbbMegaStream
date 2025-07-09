# OsbbMegaStream Auth Service

This service provides authentication endpoints for the OsbbMegaStream platform.

## How to Build and Run the Auth Service

1. **Navigate to the auth service directory:**

   ```bash
   cd /home/saqw3r/Sources/OsbbMegaStream/mediamtx/auth
   ```

2. **Restore dependencies:**

   ```bash
   dotnet restore
   ```

3. **Build the project:**

   ```bash
   dotnet build
   ```

4. **Run the service:**

   ```bash
   dotnet run
   ```

   By default, the service will listen on `http://localhost:5260` (see `Properties/launchSettings.json` for details).

---

## Endpoints

- `POST /login` — Authenticates a user and returns a unique `streampath` if credentials are valid.
- `POST /auth` — Used for stream authentication (see backend code for details).

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- (Optional) Mediamtx server for streaming
