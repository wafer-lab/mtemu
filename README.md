# MT1804 Emulator

## Getting started on Windows

Для запуска программы на Windows необходимо:
- [Скачать mtemu.zip](https://docs.github.com/en/actions/managing-workflow-runs/downloading-workflow-artifacts),
- Разархивировать скачанный архив и запустить `mtemu.exe`

## Getting started on Linux

Для запуска программы на Linux необходимо:
- [Скачать mtemu.zip](https://docs.github.com/en/actions/managing-workflow-runs/downloading-workflow-artifacts),
- Разархивировать скачанный архив и установить права запуска для `mtemu.exe`:
```shell
unzip mtemu.zip
chmod 755 mtemu.exe
```
- Выполнить `./mtemu.exe`

При возникновении ошибки Exec format error
```
bash: ./mtemu.exe: cannot execute binary file: Exec format error
```
Необходимо установить MSBuild или Mono MSBuild

### Mono MSBuild для Arch Linux / Manjaro

```
sudo pacman -S mono-msbuild
```

### Mono MSBuild для Ubuntu

```
sudo apt-get install -y gnupg ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt-get update -y
sudo apt-get install -y mono-roslyn
```
