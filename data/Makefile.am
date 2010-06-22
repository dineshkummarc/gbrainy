
pixmapdir = $(datadir)/pixmaps
pixmap_DATA = app-graphics/gbrainy.png \
		app-graphics/gbrainy.svg \
		app-graphics/gbrainy16.png

analogiesdir = $(datadir)/games/gbrainy

hicolordir = $(datadir)/icons/hicolor
images = $(datadir)/games/gbrainy

man_MANS = gbrainy.6

desktop_in_files=	\
	gbrainy.desktop.in	

desktop_files=$(desktop_in_files:.desktop.in=.desktop)
@INTLTOOL_DESKTOP_RULE@

Applicationsdir = $(datadir)/applications
Applications_DATA = $(desktop_files)

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = gbrainy.pc

tango_icons = 				\
	app-graphics/gbrainy.png	\
	app-graphics/gbrainy.svg	\
	app-graphics/background.svg	\
	app-graphics/math-games.svg	\
	app-graphics/logic-games.svg	\
	app-graphics/memory-games.svg	\
	app-graphics/math-games-32.png	\
	app-graphics/logic-games-32.png	\
	app-graphics/memory-games-32.png	\
	app-graphics/resume-32.png		\
	app-graphics/endgame-32.png		\
	app-graphics/allgames-32.png		\
	app-graphics/pause-32.png \
	app-graphics/verbal-games.svg \
	app-graphics/verbal-games-32.png


analogies_DATA = \
	verbal_analogies.xml \
	games.xml \
	game-graphics/clock.svg \
	game-graphics/family.svg \
	game-graphics/password.svg \
	game-graphics/tennis.svg \
	game-graphics/father_son.svg \
	game-graphics/money.svg 
	

install-data-local:
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/scalable/apps
	$(INSTALL_DATA) $(srcdir)/app-graphics/gbrainy.svg $(DESTDIR)$(hicolordir)/scalable/apps/gbrainy.svg
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/16x16/apps
	$(INSTALL_DATA) $(srcdir)/app-graphics/gbrainy16.png $(DESTDIR)$(hicolordir)/16x16/apps/gbrainy.png
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/32x32/apps
	$(INSTALL_DATA) $(srcdir)/app-graphics/gbrainy.png $(DESTDIR)$(hicolordir)/32x32/apps/gbrainy.png
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/48x48/apps
	$(INSTALL_DATA) $(srcdir)/app-graphics/gbrainy.png $(DESTDIR)$(hicolordir)/48x48/apps/gbrainy.png
	@-$(mkinstalldirs) $(DESTDIR)$(images)
	$(INSTALL_DATA) $(srcdir)/verbal_analogies.xml $(DESTDIR)$(images)/verbal_analogies.xml
	$(INSTALL_DATA) $(srcdir)/games.xml $(DESTDIR)$(images)/games.xml
	$(INSTALL_DATA) $(srcdir)/game-graphics/clock.svg $(DESTDIR)$(images)/clock.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/family.svg $(DESTDIR)$(images)/family.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/password.svg $(DESTDIR)$(images)/password.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/tennis.svg $(DESTDIR)$(images)/tennis.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/father_son.svg $(DESTDIR)$(images)/father_son.svg
	$(INSTALL_DATA) $(srcdir)/game-graphics/money.svg $(DESTDIR)$(images)/money.svg

gtk_update_icon_cache = gtk-update-icon-cache -f -t $(datadir)/icons/hicolor

install-data-hook:
	@-if test -z "$(DESTDIR)"; then \
		echo "Updating Gtk icon cache."; \
		$(gtk_update_icon_cache); \
	else \
		echo "*** Icon cache not updated.  After install, run this:"; \
		echo "***   $(gtk_update_icon_cache)"; \
	fi

uninstall-hook:
	rm -f $(DESTDIR)$(images)/verbal_analogies.xml
	rm -f $(DESTDIR)$(hicolordir)/48x48/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/32x32/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/16x16/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/scalable/apps/gbrainy.svg

noinst_DATA = 	

DISTCLEANFILES = $(desktop_files)

EXTRA_DIST = $(pixmap_DATA) $(tango_icons) $(man_MANS) $(analogies_DATA) $(desktop_in_files) gbrainy.pc.in
