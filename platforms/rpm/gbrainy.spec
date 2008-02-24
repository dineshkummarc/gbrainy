%define debug_package %{nil}
#gbrainy does not contain any ELF binary

Name:           gbrainy
Version:        0.3
Release:        3%{?dist}
Summary:        A brain teaser game

Group:          Amusements/Games
License:        GPLv2+
URL:            http://live.gnome.org/gbrainy
Source0:        http://www.softcatala.org/~jmas/gbrainy/%{name}-%{version}.tar.gz
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)

BuildRequires:  gettext 
BuildRequires:  desktop-file-utils
BuildRequires:  perl(XML::Parser)
BuildRequires:  mono-devel
BuildRequires:  gnome-sharp-devel
BuildRequires:  gtk-sharp2-devel


ExcludeArch: ppc64
#Mono not available on this arch


%description
gbrainy is a brain teaser game and trainer to have fun and to keep your brain 
trained
%prep
%setup -q

%build
%configure
sed -i -e 's|/usr//usr|%{_prefix}|' src/%{name}
make %{?_smp_mflags}

%install
rm -rf $RPM_BUILD_ROOT
make install DESTDIR=$RPM_BUILD_ROOT INSTALL="install -p"
desktop-file-install --vendor="fedora"                     \
  --delete-original                                        \
  --dir=%{buildroot}%{_datadir}/applications               \
  %{buildroot}%{_datadir}/applications/%{name}.desktop
%find_lang %{name}

%clean
rm -rf $RPM_BUILD_ROOT

%post
touch --no-create %{_datadir}/icons/hicolor
if [ -x %{_bindir}/gtk-update-icon-cache ]; then
  %{_bindir}/gtk-update-icon-cache --quiet %{_datadir}/icons/hicolor || :
fi

%postun
touch --no-create %{_datadir}/icons/hicolor
if [ -x %{_bindir}/gtk-update-icon-cache ]; then
  %{_bindir}/gtk-update-icon-cache --quiet %{_datadir}/icons/hicolor || :
fi

%files -f %{name}.lang
%defattr(-,root,root,-)
%doc README NEWS COPYING AUTHORS ChangeLog MAINTAINERS
%{_bindir}/gbrainy
%{_bindir}/gbrainy.exe
%{_datadir}/icons/hicolor/16x16/apps/gbrainy.png
%{_datadir}/icons/hicolor/32x32/apps/gbrainy.png
%{_datadir}/icons/hicolor/scalable/apps/gbrainy.svg
%{_mandir}/man1/gbrainy.1.gz
%{_datadir}/pixmaps/gbrainy.png
%{_datadir}/pixmaps/gbrainy.svg
%{_datadir}/pixmaps/gbrainy16.png
%{_datadir}/applications/fedora-gbrainy.desktop


%changelog
* Wed Oct 17 2007 Jean-François Martin <lokthare@gmail.com> 0.3-3
- BuildRequires and Requires corrections

* Tue Oct 16 2007 Jean-François Martin <lokthare@gmail.com> 0.3-2
- BuildRequires and Requires corrections
- Exclude ppc64 arch
- Don't create the debuginfo which is useless
- Refresh the icon-cache after install and uninstall
- Various cleaning in the spec 

* Sat Oct 13 2007 Jean-François Martin <lokthare@gmail.com> 0.3-1
- First RPM release.
