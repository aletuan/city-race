#!/usr/bin/env python3
"""Serve only the generated Unity Web build on loopback, with compression headers."""
import argparse
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1] / 'Builds/Web'


class UnityHandler(SimpleHTTPRequestHandler):
    def list_directory(self, path):
        self.send_error(403, 'Directory listing disabled')
        return None

    def guess_type(self, path):
        base = path.removesuffix('.br').removesuffix('.gz')
        if base.endswith('.wasm'):
            return 'application/wasm'
        if base.endswith('.js'):
            return 'application/javascript'
        return super().guess_type(base)

    def end_headers(self):
        path = self.path.split('?', 1)[0]
        if self.command in ('GET', 'HEAD') and Path(self.translate_path(self.path)).is_file():
            if path.endswith('.br'):
                self.send_header('Content-Encoding', 'br')
            elif path.endswith('.gz'):
                self.send_header('Content-Encoding', 'gzip')
        self.send_header('X-Content-Type-Options', 'nosniff')
        self.send_header('Cache-Control', 'no-cache')
        super().end_headers()

    def send_head(self):
        target = Path(self.translate_path(self.path)).resolve()
        if not target.is_relative_to(ROOT.resolve()):
            self.send_error(403, 'Outside build directory')
            return None
        return super().send_head()


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--port', type=int, default=8765)
    args = parser.parse_args()
    if not (ROOT / 'index.html').is_file():
        parser.error('Builds/Web/index.html missing; run unity_project.py build-web first')
    server = ThreadingHTTPServer(('127.0.0.1', args.port), partial(UnityHandler, directory=str(ROOT)))
    print(f'Serving {ROOT} at http://127.0.0.1:{args.port}', flush=True)
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        pass
    finally:
        server.server_close()
