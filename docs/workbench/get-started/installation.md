# Installation

## Windows (Recommended)

### Option 1: Download Release (Easiest)

1. Go to [GitHub Releases](https://github.com/Supervertaler/Supervertaler-Workbench/releases)
2. Download the latest `.zip` file
3. Extract to a folder of your choice
4. Run `Supervertaler.exe`
5. _Optional:_ double-click **`Add Supervertaler to Start Menu.cmd`** once to add a Start Menu shortcut, so you can launch the app from the Start Menu (or pin it to the taskbar) like any installed program. This is just a friendly wrapper around `create_start_menu_shortcut.ps1` that bypasses Windows' default PowerShell ExecutionPolicy without changing any system-wide settings.

### Option 2: Run from Source

If you want the latest development version or want to contribute:

```bash
# Clone the repository
git clone https://github.com/Supervertaler/Supervertaler-Workbench.git
cd Supervertaler

# Create virtual environment
python -m venv venv
venv\Scripts\activate

# Install dependencies
pip install -r requirements.txt

# Run the application
python Supervertaler.py
```

## macOS

The macOS install method depends on your Mac's processor.

### Apple Silicon (M1, M2, M3, M4) — Download Release

1. Go to [GitHub Releases](https://github.com/Supervertaler/Supervertaler-Workbench/releases)
2. Download the latest `.dmg` file
3. Open the `.dmg` and drag **Supervertaler** to your Applications folder
4. Launch from Spotlight or Launchpad

### Intel Macs — Install via pip

The published macOS `.dmg` is built for Apple Silicon only and will not run on Intel hardware. Intel Mac users need to install via pip and provide a system Java for the Okapi sidecar (which handles Word, Excel, HTML and other office-document imports).

```bash
# 1. Install Homebrew (skip if already installed)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# 2. Install Python 3 (skip if already installed)
brew install python@3.12

# 3. Install Java 17 (Eclipse Temurin, free, no Oracle account required)
brew install --cask temurin@17

# 4. Install Supervertaler
pip3 install supervertaler

# 5. Run it
supervertaler
```

If you skip the Java step, Supervertaler shows a friendly dialog at startup with the install command. Plain-text translation, TMX, glossaries, etc. all work without Java – only office-document import/export needs it.

On first DOCX import, Supervertaler downloads the Okapi sidecar JAR (~28 MB) into `~/Library/Application Support/Supervertaler/okapi-sidecar/`. After that, everything runs locally and offline.

### Run from Source (any Mac)

Follow the [Linux source instructions](#linux) below – the commands are identical except for the spellcheck step, where you'd use `brew install hunspell` instead of `apt install hunspell-*`.

## Linux

Supervertaler is compatible with Linux, though Windows is the primary development platform.

```bash
# Clone the repository
git clone https://github.com/Supervertaler/Supervertaler-Workbench.git
cd Supervertaler

# Create virtual environment
python3 -m venv venv
source venv/bin/activate

# Install dependencies
pip install -r requirements.txt

# Install Hunspell dictionaries (for spellcheck)
sudo apt install hunspell-en-us hunspell-nl  # Add your languages

# Run the application
python Supervertaler.py
```

{% hint style="info" %}
**Linux Users:** If you experience crashes related to spellcheck or ChromaDB, see [Linux-Specific Issues](../troubleshooting/linux.md).
{% endhint %}

## Dependencies

The main dependencies are automatically installed via `requirements.txt`:

| Package | Purpose |
|---------|---------|
| PyQt6 | User interface |
| openai | OpenAI GPT integration |
| anthropic | Anthropic Claude integration |
| google-generativeai | Google Gemini integration |
| python-docx | DOCX file handling |
| chromadb | Supermemory vector search |
| sentence-transformers | Semantic embeddings |
| pyspellchecker | Spellcheck |

## Next Steps

After installation:

1. [Set up your API keys](api-keys.md) for AI translation
2. Follow the [Quick Start Guide](quick-start.md)
3. Create your [first translation project](first-project.md)
