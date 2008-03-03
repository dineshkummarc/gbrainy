@echo off
echo Running gbrainy
set PATH=\mono\MonoX-1.2.1\bin;%PATH%
set GTK_BASEPATH=\mono\MonoX-1.2.1
set PKG_CONFIG_PATH=\mono\MonoX-1.2.1\lib\pkgconfig
set GNOME_PATH=\mono\MonoX-1.2.1
mono gbrainy.exe