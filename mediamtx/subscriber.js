const http = require('http');

const PORT = 3000;
const WEBHOOK_PATH = '/webhook';

const server = http.createServer((req, res) => {
    if (req.method === 'POST' && req.url === WEBHOOK_PATH) {
        let body = '';

        req.on('data', chunk => {
            body += chunk.toString();
        });

        req.on('end', () => {
            try {
                const event = JSON.parse(body);
                console.log('Received MediaMTX Event:');
                console.log(JSON.stringify(event, null, 2));

                res.writeHead(200, { 'Content-Type': 'application/json' });
                res.end(JSON.stringify({ status: 'ok' }));
            } catch (error) {
                console.error('Error parsing JSON:', error);
                res.writeHead(400, { 'Content-Type': 'application/json' });
                res.end(JSON.stringify({ error: 'Invalid JSON' }));
            }
        });
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain' });
        res.end('Not Found');
    }
});

server.listen(PORT, () => {
    console.log(`Server listening on port ${PORT}`);
    console.log(`Webhook endpoint: http://localhost:${PORT}${WEBHOOK_PATH}`);
});
