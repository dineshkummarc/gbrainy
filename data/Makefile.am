
pixmapdir = $(datadir)/pixmaps
pixmap_DATA = gbrainy.png \
		gbrainy.svg \
		gbrainy16.png 

hicolordir = $(datadir)/icons/hicolor

man_MANS = gbrainy.1

tango_icons = 				\
	gbrainy.png	\
	gbrainy.svg	\
	math-games-80.png	\
	logic-games-80.png	\		
	memory-games-80.png	\ 
	math-games-32.png	\
	logic-games-32.png	\
	memory-games-32.png	\	 
	resume-32.png		\
	endgame-32.png		\
	allgames-32.png		\
	pause-32.png


install-data-local:
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/scalable/apps
	$(INSTALL_DATA) $(srcdir)/gbrainy.svg $(DESTDIR)$(hicolordir)/scalable/apps/gbrainy.svg
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/16x16/apps
	$(INSTALL_DATA) $(srcdir)/gbrainy16.png $(DESTDIR)$(hicolordir)/16x16/apps/gbrainy.png
	@-$(mkinstalldirs) $(DESTDIR)$(hicolordir)/32x32/apps
	$(INSTALL_DATA) $(srcdir)/gbrainy.png $(DESTDIR)$(hicolordir)/32x32/apps/gbrainy.png
	@-$(mkinstalldirs) $(datadir)/gbrainy
	$(INSTALL_DATA) $(srcdir)/math-games-80.png $(datadir)/gbrainy/math-games-80.png
	$(INSTALL_DATA) $(srcdir)/logic-games-80.png $(datadir)/gbrainy/logic-games-80.png
	$(INSTALL_DATA) $(srcdir)/memory-games-80.png $(datadir)/gbrainy/memory-games-80.png

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
	rm -f $(DESTDIR)$(datadir)/gbrainy/math-games-80.png
	rm -f $(DESTDIR)$(datadir)/gbrainy/logic-games-80.png
	rm -f $(DESTDIR)$(datadir)/gbrainy/memory-games-80.png
	rm -f $(DESTDIR)$(hicolordir)/32x32/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/16x16/apps/gbrainy.png
	rm -f $(DESTDIR)$(hicolordir)/scalable/apps/gbrainy.svg
	rm -f $(DESTDIR)$(hicolordir)/32x32/apps/gbrainy.png


noinst_DATA = 	

EXTRA_DIST = $(pixmap_DATA) $(tango_icons) $(man_MANS) 
