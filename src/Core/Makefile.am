CSFLAGS = -unsafe -target:library $(CSC_DEFINES)

TARGET = ../gbrainy.Core.dll
TARGET_CFG = ../gbrainy.Core.dll.config

CSFILES =  \
		AssemblyInfo.cs			\
		Main/ArrayListIndicesRandom.cs	\
		Main/ColorPalette.cs		\
		Main/Defines.cs			\
		Main/Game.cs			\
		Main/GameManager.cs		\
		Main/GameSession.cs		\
		Main/GameTips.cs		\
		Main/Memory.cs			\
		Main/PlayerHistory.cs		\
		Main/Preferences.cs		\
		Main/UpdateUIStateEventArgs.cs \
		Main/Verbal/Analogies.cs 	\
		Main/Verbal/AnalogiesFactory.cs	\
		Main/Verbal/AnalogiesMultipleOptions.cs	\
		Main/Verbal/AnalogiesPairOfWordsCompare.cs	\
		Main/Verbal/AnalogiesPairOfWordsOptions.cs	\
		Main/Verbal/AnalogiesQuestionAnswer.cs		\
		Main/Verbal/Analogy.cs		\
		Views/CountDownView.cs		\
		Views/FinishView.cs		\
		Main/IDrawable.cs		\
		Main/IDrawRequest.cs		\
		Views/PlayerHistoryView.cs	\
		Views/ViewsControler.cs		\
		Views/WelcomeView.cs		\
		Platform/Unix.cs		\
		Libraries/CairoContextEx.cs	\
		Libraries/SVGImage.cs

RES = \
$(top_srcdir)/data/app-graphics/background.svg  	\
$(top_srcdir)/data/app-graphics/logic-games.svg		\
$(top_srcdir)/data/app-graphics/math-games.svg		\
$(top_srcdir)/data/app-graphics/memory-games.svg	\
$(top_srcdir)/data/app-graphics/verbal-games.svg	\
$(srcdir)/gbrainy.addin.xml


REFS =  \
	-r:System \
	-r:Mono.Cairo.dll		\
	-r:Mono.Posix			\
	 $(GBRAINY_LIBS)    		\
	 $(MONO_ADDINS_LIBS)

SRCDIR_CSFILES = $(CSFILES:%=$(srcdir)/%)
RES_CSFLAGS = $(foreach res, $(RES), -resource:$(res))

$(TARGET): $(SRCDIR_CSFILES) $(RES)
	echo $(RES_FILES)
	$(CSC) -out:$@ $(CSFLAGS) $(REFS) $(RES_CSFLAGS) $(SRCDIR_CSFILES)

$(TARGET_CFG): $(srcdir)/gbrainy.Core.dll.config.in
	sed -e "s|\@pkglibdir\@|$(pkglibdir)|" \
	    < $< > $@

all: $(TARGET) $(TARGET_CFG)

install-data-local: $(TARGET) $(TARGET_CFG)
	$(mkinstalldirs) $(DESTDIR)$(pkglibdir)
	$(INSTALL_DATA) $(TARGET) $(DESTDIR)$(pkglibdir)
	$(INSTALL_DATA) $(TARGET_CFG) $(DESTDIR)$(pkglibdir)

uninstall-local:
	cd $(DESTDIR)$(pkglibdir) && rm -f gbrainy.Core.dll
	cd $(DESTDIR)$(pkglibdir) && rm -f gbrainy.Core.dll.config

EXTRA_DIST = 			\
	$(CSFILES) $(RES) gbrainy.Core.dll.config.in

CLEANFILES =				\
	$(TARGET)			\
	$(TARGET).mdb			\
	$(TARGET_CFG)
