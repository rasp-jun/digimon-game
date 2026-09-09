"""Compatibility launcher for the rebuilt, offline File Island League.

The former independent Tkinter implementation is preserved under legacy/.
No third-party Python packages or local HTTP server are required.
"""
from pathlib import Path
import webbrowser


def main():
    game = Path(__file__).resolve().with_name("index.html")
    if not game.is_file():
        raise SystemExit("index.html is missing. Keep this launcher in the game folder.")
    webbrowser.open(game.as_uri())


if __name__ == "__main__":
    main()
