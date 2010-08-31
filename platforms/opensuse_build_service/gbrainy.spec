#
# spec file for package gbrainy
#
# Copyright (c) 2007 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
#
# Please submit bugfixes or comments via http://bugs.opensuse.org/
#
# 
# 
# norootforbuild

Name:           gbrainy
Version:        1.51
Release:        1.0
License:        GPL v2 or later
Source:         %{name}-%{version}.tar.gz
Autoreqprov:    on
PreReq:         filesystem
URL:            http://live.gnome.org/gbrainy
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
BuildRequires:  mono-devel gtk-sharp2 perl-XML-Parser intltool mono-core
Group:          Games
Summary:        gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained.

%if 0%{?suse_version}
BuildRequires: glade-sharp2 gnome-sharp2 librsvg-devel update-desktop-files mono-addins gnome-doc-utils-devel
%endif

%if 0%{?fedora_version}
BuildRequires: gtk-sharp2-devel gnome-sharp-devel librsvg2-devel mono-addins-devel PolicyKit-gnome gnome-doc-utils
%endif

%description
gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained.

It provides the following types of games:

* Logic puzzles. Games designed to challenge your reasoning and thinking skills.
* Mental calculation. Games based on arithmetical operations designed to prove your mental calculation skills.
* Memory trainers. Games designed to challenge your short term memory.
* Verbal analogies. Challenge your verbal aptitude.


%prep
%setup -q

%build
export MONO_SHARED_DIR=/var/tmp
export CFLAGS="$RPM_OPT_FLAGS"
intltoolize --force
autoreconf
%configure
make

%install
make install DESTDIR=$RPM_BUILD_ROOT

%if 0%{?suse_version}
%suse_update_desktop_file %{name} -N "gbrainy"
%endif

%clean
rm -rf "$RPM_BUILD_ROOT"

%files
%defattr(-,root,root)
%doc AUTHORS NEWS README COPYING
%{_bindir}/gbrainy
%{_libdir}/gbrainy/gbrainy.exe
%{_libdir}/gbrainy/gbrainy.Core.dll
%{_libdir}/gbrainy/gbrainy.Core.dll.config
%{_libdir}/gbrainy/gbrainy.Games.dll
%{_datadir}/games/gbrainy/games.xml
%{_datadir}/games/gbrainy/verbal_analogies.xml
%{_datadir}/icons/hicolor/*/apps/*
%{_datadir}/locale/*/LC_MESSAGES/gbrainy.mo
%{_datadir}/man/man6/gbrainy.6.gz
%{_datadir}/pixmaps/*
%{_datadir}/gnome/help/gbrainy/*
%{_datadir}/gnome/help/gbrainy/
%{_libdir}/gbrainy
%{_libdir}/pkgconfig/gbrainy.pc
%{_datadir}/games/gbrainy/*
%{_datadir}/games/gbrainy/
%{_datadir}/applications/gbrainy.desktop
%changelog

