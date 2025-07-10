<template>
  <div>
    <h2>Video Streamer</h2>
    <video ref="videoElement" autoplay muted></video>
    <div v.if="!isStreaming">
      <button @click="startStreaming">Start Streaming</button>
    </div>
    <div v.else>
      <button @click="stopStreaming">Stop Streaming</button>
    </div>
    <p>{{ statusMessage }}</p>
  </div>
</template>

<script>
export default {
  data() {
    return {
      isStreaming: false,
      statusMessage: 'Ready to stream.',
      peerConnection: null,
      localStream: null,
    };
  },
  methods: {
    preferH264(sdp) {
      const lines = sdp.split('\n');
      let mLineIndex = -1;

      for (let i = 0; i < lines.length; i++) {
        if (lines[i].startsWith('m=video')) {
          mLineIndex = i;
          break;
        }
      }

      if (mLineIndex === -1) {
        return sdp;
      }

      let h264PayloadType = '';
      for (let i = 0; i < lines.length; i++) {
        if (lines[i].includes('H264/90000')) {
          const parts = lines[i].match(/a=rtpmap:(\d+)/);
          if (parts && parts.length > 1) {
            h264PayloadType = parts[1];
            break;
          }
        }
      }

      if (h264PayloadType) {
        const mLineParts = lines[mLineIndex].split(' ');
        const otherPayloads = mLineParts.slice(3).filter(p => p !== h264PayloadType);
        lines[mLineIndex] = [mLineParts[0], mLineParts[1], mLineParts[2], h264PayloadType, ...otherPayloads].join(' ');
      }

      return lines.join('\n');
    },
    async startStreaming() {
      try {
        // Step 1: Authenticate and get streampath and jwt
        this.statusMessage = 'Authenticating...';
        const loginResponse = await fetch('http://localhost:5260/auth', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ username: 'streamer', password: 'securepass' }) // hard-coded credentials for demo purposes
        });
        if (!loginResponse.ok) {
          this.statusMessage = 'Login failed. Check credentials.';
          return;
        }
        const loginData = await loginResponse.json();
        const streampath = loginData.streampath;
        const jwt = loginData.jwt;
        if (!streampath || !jwt) {
          this.statusMessage = 'No stream path or token received from server.';
          return;
        }

        // Step 2: Get camera/mic and start streaming
        this.localStream = await navigator.mediaDevices.getUserMedia({
          video: true,
          audio: true,
        });
        this.$refs.videoElement.srcObject = this.localStream;
        this.statusMessage = 'Camera accessed. Ready to start streaming.';
        await this.initiateWhip(streampath, jwt);
      } catch (error) {
        console.error('Error starting streaming.', error);
        this.statusMessage = 'Error starting streaming. Please check permissions and network.';
      }
    },
    stopStreaming() {
      if (this.peerConnection) {
        this.peerConnection.close();
        this.peerConnection = null;
      }
      if (this.localStream) {
        this.localStream.getTracks().forEach(track => track.stop());
        this.localStream = null;
      }
      this.$refs.videoElement.srcObject = null;
      this.isStreaming = false;
      this.statusMessage = 'Stream stopped.';
    },
    async initiateWhip(streampath, jwt) {
      this.statusMessage = 'Connecting to Mediamtx...';

      this.peerConnection = new RTCPeerConnection();

      this.localStream.getTracks().forEach(track => {
        this.peerConnection.addTrack(track, this.localStream);
      });

      const offer = await this.peerConnection.createOffer();
      offer.sdp = this.preferH264(offer.sdp);
      await this.peerConnection.setLocalDescription(offer);

      try {
        // Append JWT as query param
        const whipUrl = `http://localhost:8889/${encodeURIComponent(streampath)}/whip?jwt=${encodeURIComponent(jwt)}`;

        const response = await fetch(whipUrl, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/sdp',
          },
          body: this.peerConnection.localDescription.sdp,
        });

        if (response.ok) {
          const answerSdp = await response.text();
          await this.peerConnection.setRemoteDescription(
            new RTCSessionDescription({ type: 'answer', sdp: answerSdp })
          );
          this.isStreaming = true;
          this.statusMessage = 'Streaming live!';
        } else {
          throw new Error(`Failed to connect to WHIP endpoint: ${response.status} ${response.statusText}`);
        }
      } catch (error) {
        console.error('Error initiating WHIP connection:', error);
        this.statusMessage = 'Failed to start stream. Check Mediamtx server and network.';
      }
    },
  },
};
</script>

<style scoped>
video {
  width: 100%;
  max-width: 600px;
  border: 1px solid #ccc;
}
</style>