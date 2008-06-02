Here are some notes on how to build gbrainy's installer for Win32

-- Software used 

*  The Experimental Combined Mono and Gtk# Installer for Win32
Available at: http://forge.novell.com/modules/xfmod/project/?monowin32

It contains the mono run-time and gtk# bindings

* NSIS 2.31 install system

To build the installer

-- Files need it

Since we do not use ./configure there are a few files that we should
generate manually and that there are at subdirectory src/

* A built Defines.cs for the win32 build
* build.bat that builds the application
* Apply the win32.patch to remove GNOME dependencies

At /installer you have

* gbrainy.nsi that is the NSI gbrainy installer project
* gbrainy.bat the batch used to launch gbrainy

-- Installer directory structure 

-- Todo (things to do someday)

* Assembly.cs
* Application icon
