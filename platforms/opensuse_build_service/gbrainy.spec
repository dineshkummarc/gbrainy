#
# spec file for package gbrainy
#
# Copyright (c) 2007 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
#
# Please submit bugfixes or comments via http://bugs.opensuse.org/
#

# norootforbuild

Name:           gbrainy
Version:        0.53
Release:        1.0
License:        GPL
Source:         %{name}-%{version}.tar.gz
Autoreqprov:    on
PreReq:         filesystem
URL:            http://live.gnome.org/gbrainy
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
BuildRequires:  mono-devel gtk-sharp2 perl-XML-Parser intltool
Group:          Games
Summary:        gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained.

%if 0%{?suse_version}
BuildRequires: glade-sharp2 gnome-sharp2 
%endif

%if 0%{?fedora_version}
BuildRequires: gtk-sharp2-devel gnome-sharp-devel
%endif

%description
gbrainy is a brain teaser game and trainer to have fun and to keep your brain trained.

It provides the following types of games:

* Logic puzzles. Games designed to challenge your reasoning and thinking skills.
* Mental calculation. Games based on arithmetical operations designed to prove your mental calculation skills.
* Memory trainers. Games designed to challenge your short term memory.


%prep
%setup -q

%build
export MONO_SHARED_DIR=/var/tmp
export CFLAGS="$RPM_OPT_FLAGS"
autoreconf
%configure
make

%install
make install DESTDIR=$RPM_BUILD_ROOT

%clean
rm -rf "$RPM_BUILD_ROOT"

%files
%defattr(-,root,root)
%doc AUTHORS NEWS README COPYING
%{_bindir}/gbrainy
%{_libdir}/gbrainy/gbrainy.exe
%{_datadir}/applications/gbrainy.desktop
%{_datadir}/games/gbrainy/*
%{_datadir}/icons/hicolor/*/apps/*
%{_datadir}/locale/*/LC_MESSAGES/gbrainy.mo
%{_datadir}/man/man6/gbrainy.6.gz
%{_datadir}/pixmaps/*

%changelog

