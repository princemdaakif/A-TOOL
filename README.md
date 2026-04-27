# A-TOOL — Android ADB Utility Tool

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows-blue?style=flat-square&logo=windows" />
  <img src="https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet" />
  <img src="https://img.shields.io/badge/ADB-Bundled-green?style=flat-square&logo=android" />
  <img src="https://img.shields.io/badge/License-MIT-yellow?style=flat-square" />
  <img src="https://img.shields.io/badge/Version-2.0.0-orange?style=flat-square" />
</p>

<p align="center">
  A powerful all-in-one Windows tool to debloat, back up, tweak, and manage any Android device — no root required.
</p>

---

## ✨ Features

| Module | Description |
|---|---|
| 📊 **Device Info** | Full hardware & software info dashboard |
| 📦 **Debloat Manager** | Remove bloat by preset OEM list **or** manually pick any package |
| 💾 **Backup & Restore** | ADB backup, APK pull, full /sdcard copy, restore |
| ⚡ **Performance & Tweaks** | Animations, DPI, font scale, MSAA, battery, ZRAM |
| 📱 **App Manager** | Install, uninstall, disable, enable, clear data, force-stop |
| 🔴 **ReVanced Installer** | One-click install for ReVanced YouTube & ReVanced Manager |
| 📸 **Screenshot & Record** | Capture screenshots & screen recordings, auto-saved to PC |
| 📡 **Wireless ADB** | Enable TCP/IP mode, pair & connect without USB |
| 🔄 **Reboot** | Normal, recovery, bootloader, safe mode |

---

## 🆕 What's New in v2.0

- ✅ **Manual debloat selector** — browse every package on your device, filter by name, checkbox-select exactly what you want removed
- ✅ **App Manager module** — full install/uninstall/disable/enable/clear/force-stop support
- ✅ **Screenshot & screen record** — auto-pulls to PC
- ✅ **Wireless ADB pairing** — Android 11+ pair-by-QR-code support
- ✅ **ReVanced architecture detector** — tells you which APK variant to download
- ✅ **CLI flags** — use `--debloat manual` etc. for scripting
- ✅ Debloat log saved automatically after every run

---

## 📋 Requirements

- Windows 10/11 (64-bit)
- [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) — or use the self-contained release
- Android phone with **USB Debugging enabled**
  - Settings → About Phone → tap Build Number 7× → Developer Options → USB Debugging ✔

---

## 🚀 Quick Start

1. **Download** the latest release from [Releases](https://github.com/princemdaakif/A-TOOL/releases)
2. **Extract** all files to a folder (keep `adb.exe`, `fastboot.exe`, and DLLs alongside the `.exe`)
3. **Connect** your phone via USB and allow the ADB authorisation prompt on the device
4. **Run** `AAKIFTools.exe`

---

## 🔌 Enabling USB Debugging

```
Settings → About Phone → tap "Build Number" 7 times
Settings → Developer Options → USB Debugging → ON
```

When you run A-TOOL for the first time, a pop-up will appear on your phone — tap **Allow**.

---

## 📦 Debloat — Manual App Selection (New)

The manual selector lets you browse every installed package, filter by name, and checkbox-select exactly what you want to remove — no guessing.

**How to use:**
1. Open **Debloat Manager → Manual selection**
2. A-TOOL fetches the full package list from your device
3. Navigate with **↑ ↓** arrow keys
4. Press **Space** to toggle selection ✔
5. Type **/** to open the filter — start typing a package name to search
6. Press **A** to select all visible, **N** to clear all
7. Press **Enter** to confirm, **Esc** to cancel
8. Choose **Disable** (reversible) or **Uninstall** (permanent)
9. A log is saved to `debloat_log.txt`

> **Tip:** Use the filter to quickly find e.g. `com.facebook`, `bixby`, or `miui` across hundreds of packages.

---

## 🏭 Preset Debloat Lists

A-TOOL ships with ready-made bloat lists for common OEMs:

| Preset | Removes |
|---|---|
| Samsung | Bixby, Galaxy Store, Samsung Cloud, AR features, S Browser |
| Xiaomi/MIUI | MIUI analytics, ads, Mi Picks, Joyose, game centre SDK |
| OnePlus/OPPO | Heytap Market, ColorOS bloat, NearMe Game Center |
| Google | YouTube, Maps, Duo/Meet, Google TV, subscriptions |
| Facebook/Meta | Pre-installed Facebook, Messenger, Instagram system agents |
| Microsoft | Office suite, Teams, OneDrive pre-installs |

---

## 💾 Backup & Restore

| Option | Notes |
|---|---|
| Full ADB backup | Backs up apps + data — requires tapping "Back up my data" on device |
| APK-only backup | Pulls `.apk` files silently, no user interaction needed |
| Selected app backup | Choose specific apps to back up |
| Pull /sdcard | Copies entire phone storage to PC |
| Restore | Restores from any `.ab` backup file |

Backups are saved under `backups/<timestamp>/` next to the exe.

---

## ⚡ Performance Tweaks

| Tweak | Effect |
|---|---|
| Disable animations | Phone feels instant |
| Speed up (0.5×) | Snappier without breaking transitions |
| Background process limit | Reduces RAM usage |
| 4× MSAA | Smoother GPU rendering |
| Battery optimize | Aggressive doze, limit background |
| DPI change | Scale UI up or down |
| Font scale | Make text larger or smaller |
| Apply all | Recommended defaults in one step |

---

## 🔴 ReVanced

A-TOOL bundles the ReVanced APK and ReVanced Manager Plus for quick installation.

1. Make sure **Unknown Sources** is enabled: Settings → Apps → Special app access → Install unknown apps → allow your file manager or A-TOOL
2. Select **ReVanced Installer** from the main menu
3. Choose which APK to install
4. Use **Check device architecture** if unsure which variant to download

---

## 📡 Wireless ADB

Connect without a USB cable:

1. First connect via USB once
2. Go to **Wireless ADB → Enable wireless mode** — your phone switches to TCP/IP mode
3. Unplug USB
4. Go to **Connect to device by IP** — enter your phone's Wi-Fi IP (found in Settings → Wi-Fi → tap connected network)

For Android 11+, use **Pair new device** with the QR pairing code shown in Developer Options.

---

## 🗂 File Structure

```
AAKIFTools.exe            — Main executable
adb.exe                   — Bundled ADB (v35+)
fastboot.exe              — Bundled Fastboot
AdbWinApi.dll             — ADB Windows API
AdbWinUsbApi.dll          — ADB USB driver API
revanced.apk              — ReVanced YouTube
revanced_manager_plus_v3.0.16.apk  — ReVanced Manager Plus
backups/                  — Created automatically for backups
debloat_log.txt           — Created after each debloat run
packages.txt              — Created when you save package list
```

---

## 🛠 Building from Source

```bash
git clone https://github.com/princemdaakif/A-TOOL.git
cd A-TOOL
dotnet build -c Release
# Output: bin/Release/net8.0/AAKIFTools.exe
```

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

To build a self-contained single-file executable:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

---

## ⚠️ Disclaimer

- **No root required** — A-TOOL uses standard ADB commands only
- Always back up your device before debloating
- Removing system packages incorrectly can cause bootloops — use the **Disable** option if unsure; it's fully reversible
- ReVanced APKs are provided as-is; always verify APK signatures from trusted sources

---

## 📄 License

MIT — see [LICENSE](LICENSE)

---

## 👤 Author

**Aakif** — [github.com/princemdaakif](https://github.com/princemdaakif)
