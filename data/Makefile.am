
pixmapdir = $(datadir)/pixmaps
pixmap_DATA = app-graphics/gbrainy.png \
		app-graphics/gbrainy.svg \
		app-graphics/gbrainy16.png 

hicolordir = $(datadir)/icons/hicolor
images = $(datadir)/games/gbrainy

man_MANS = gbrainy.6

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
	app-graphics/pause-32.png

game_graphics = \
	game-graphics/present.svg	\
	game-graphics/handshake.svg

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
	$(INSTALL_DATA) $(srcdir)/app-graphics/math-games.svg $(DESTDIR)$(images)/math-games.svg
	$(INSTALL_DATA) $(srcdir)/app-graphics/logic-games.svg $(DESTDIR)$(images)/logic-games.svg
	$(INSTALL_DATA) $(srcdir)/app-graphics/memory-games.svg $(DESTDIR)$(images)/memory-games.svg
	$(INSTALL_DATA) $(srcdir)/app-graphics/background.svg $(DESTDIR)$(images)/background.svg

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
	rm -f $(DESTDIR)$(images)/math-games.svg
	rm -f $(DESTDIR)$(images)/logic-games.svg
	rm -f $(DESTDIR)$(images)/memory-games.svg
	rm -f $(DESTDIR)$(images)/background.svg
	rm -f $(DESTDIR)$(hicolordir)/48x48/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/32x32/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/16x16/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/scalable/apps/gbrainy.svg

noinst_DATA = 	

EXTRA_DIST = $(pixmap_DATA) $(tango_icons) $(game_graphics) $(man_MANS) gbrainy.exe.config
