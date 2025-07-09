# OsbbMegaStream UI

## Login and Streaming Flow

1. When you click **Start Streaming**, the app first sends a POST request to `http://localhost:5260/login` with the following JSON payload:

   ```json
   {
     "username": "streamer",
     "password": "securepass"
   }
   ```

2. If the credentials are correct, the backend responds with a JSON object containing a unique `streampath` and the current UTC datetime:

   ```json
   {
     "streampath": "<guid>",
     "datetime": "2025-07-09T12:34:56Z"
   }
   ```

3. The UI then accesses your camera and microphone, and starts streaming using the `streampath` as part of the WHIP connection to the Mediamtx server.

4. If authentication fails, streaming does not start and an error message is shown.

---

# b2c

This template should help get you started developing with Vue 3 in Vite.

## Recommended IDE Setup

[VSCode](https://code.visualstudio.com/) + [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur).

## Customize configuration

See [Vite Configuration Reference](https://vite.dev/config/).

## Project Setup

```sh
npm install
```

### Compile and Hot-Reload for Development

```sh
npm run dev
```

### Compile and Minify for Production

```sh
npm run build
```
