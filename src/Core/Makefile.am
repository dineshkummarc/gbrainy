CSFLAGS = -unsafe -target:library $(CSC_DEFINES) 

#Active when designing games
#CSFLAGS += -D:DESIGN_MODE

TARGET = ../gbrainy.Core.dll
TARGET_CFG = ../gbrainy.Core.dll.config

CSDISTFILES =  \
		$(srcdir)/Main/ArrayListIndicesRandom.cs	\
		$(srcdir)/Main/ColorPalette.cs		\
		$(srcdir)/Main/IMouseEvent.cs		\
		$(srcdir)/Main/Game.cs			\
		$(srcdir)/Main/GameManager.cs		\
		$(srcdir)/Main/GameSession.cs		\
		$(srcdir)/Main/GameTips.cs		\
		$(srcdir)/Main/Memory.cs			\
		$(srcdir)/Main/PlayerHistory.cs		\
		$(srcdir)/Main/Preferences.cs		\
		$(srcdir)/Main/UpdateUIStateEventArgs.cs \
		$(srcdir)/Main/Verbal/Analogies.cs 	\
		$(srcdir)/Main/Verbal/AnalogiesFactory.cs	\
		$(srcdir)/Main/Verbal/AnalogiesMultipleOptions.cs	\
		$(srcdir)/Main/Verbal/AnalogiesPairOfWordsCompare.cs	\
		$(srcdir)/Main/Verbal/AnalogiesPairOfWordsOptions.cs	\
		$(srcdir)/Main/Verbal/AnalogiesQuestionAnswer.cs		\
		$(srcdir)/Main/Verbal/Analogy.cs		\
		$(srcdir)/Views/CountDownView.cs		\
		$(srcdir)/Views/FinishView.cs		\
		$(srcdir)/Main/IDrawable.cs		\
		$(srcdir)/Main/IDrawRequest.cs		\
		$(srcdir)/Toolkit/Container.cs		\
		$(srcdir)/Toolkit/DrawableArea.cs		\
		$(srcdir)/Toolkit/DrawEventArgs.cs	\
		$(srcdir)/Toolkit/HorizontalContainer.cs	\
		$(srcdir)/Toolkit/SeletectedEventArgs.cs	\
		$(srcdir)/Toolkit/Widget.cs		\
		$(srcdir)/Views/PlayerHistoryView.cs	\
		$(srcdir)/Views/ViewsControler.cs		\
		$(srcdir)/Views/WelcomeView.cs		\
		$(srcdir)/Platform/Unix.cs		\
		$(srcdir)/Libraries/CairoContextEx.cs	\
		$(srcdir)/Libraries/GetText.cs		\
		$(srcdir)/Libraries/SVGImage.cs

CSFILES = $(CSDISTFILES)	\
	Main/Defines.cs	\
	AssemblyInfo.cs	

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

RES_CSFLAGS = $(foreach res, $(RES), -resource:$(res))

$(TARGET): $(CSFILES) $(RES)
	echo $(RES_FILES)
	$(CSC) -out:$@ $(CSFLAGS) $(REFS) $(RES_CSFLAGS) $(CSFILES)

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
	$(CSDISTFILES) $(RES) gbrainy.Core.dll.config.in

CLEANFILES =				\
	$(TARGET)			\
	$(TARGET).mdb			\
	$(TARGET_CFG)
