EXTRAFLAGS =  -nowarn:0169 $(CSC_DEFINES)

WRAPPER = gbrainy

GBRAINY_CSDISTFILES =					\
	$(srcdir)/Defines.cs 				\
	$(srcdir)/gbrainy.cs 				\
	$(srcdir)/GtkSynchronize.cs 			\
	$(srcdir)/SimpleLabel.cs			\
	$(srcdir)/Dialogs/AboutDialog.cs		\
	$(srcdir)/Dialogs/CustomGameDialog.cs		\
	$(srcdir)/Dialogs/GtkDialog.cs			\
	$(srcdir)/Dialogs/PlayerHistoryDialog.cs	\
	$(srcdir)/Dialogs/PreferencesDialog.cs

ASSEMBLIES = \
	 $(GBRAINY_LIBS)    		\
	 $(MONO_ADDINS_LIBS)    	\
	-r:Mono.Cairo.dll		\
	-r:../../gbrainy.Core.dll	\
	-r:Mono.Posix

RESOURCES =										\
-resource:$(srcdir)/gbrainy.glade  \
-resource:$(top_srcdir)/data/app-graphics/resume-32.png  \
-resource:$(top_srcdir)/data/app-graphics/endgame-32.png  \
-resource:$(top_srcdir)/data/app-graphics/pause-32.png  \
-resource:$(top_srcdir)/data/app-graphics/allgames-32.png  \
-resource:$(top_srcdir)/data/app-graphics/gbrainy.png  \
-resource:$(top_srcdir)/data/app-graphics/logic-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/math-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/memory-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/verbal-games.svg  \
-resource:$(top_srcdir)/data/app-graphics/verbal-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/gbrainy.svg

gbrainydir = $(libdir)/gbrainy
gbrainy_SCRIPTS = ../../gbrainy.exe
bin_SCRIPTS = gbrainy

GBRAINY_CSFILES = $(GBRAINY_CSDISTFILES)	\
	AssemblyInfo.cs


../../gbrainy.exe: $(GBRAINY_CSFILES) gbrainy.glade
	$(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(GBRAINY_CSFILES) $(ASSEMBLIES) $(RESOURCES)

all: ../../gbrainy.exe

EXTRA_DIST =					\
	gbrainy.glade \
	$(srcdir)/mono-addins-strings.xml \
	$(GBRAINY_CSDISTFILES)			


CLEANFILES =					\
	gbrainy.exe.config			\
	gbrainy.exe.mdb				\
	../../gbrainy.exe			\
	gbrainy


DISTCLEANFILES = 				\
	Makefile

