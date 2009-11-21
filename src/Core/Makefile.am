CSFLAGS = -target:library 

TARGET = ../gbrainy.Core.dll

CSFILES =  \
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
		Main/Verbal/Analogies.cs 	\
		Main/Verbal/AnalogiesFactory.cs	\
		Main/Verbal/AnalogiesMultipleOptions.cs	\
		Main/Verbal/AnalogiesPairOfWordsCompare.cs	\
		Main/Verbal/AnalogiesPairOfWordsOptions.cs	\
		Main/Verbal/AnalogiesQuestionAnswer.cs		\
		Main/Verbal/Analogy.cs		\
		Views/CountDownView.cs		\
		Views/FinishView.cs		\
		Views/IDrawable.cs		\
		Views/IDrawRequest.cs		\
		Views/ViewsControler.cs		\
		Views/WelcomeView.cs		\
		Platform/Unix.cs		\
		Libraries/CairoContextEx.cs	\
		Libraries/SVGImage.cs

RES = 
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

all: $(TARGET)

install-data-local: $(TARGET)
	$(mkinstalldirs) $(DESTDIR)$(pkglibdir)
	$(INSTALL_DATA) $(TARGET) $(DESTDIR)$(pkglibdir)

uninstall-local:
	cd $(DESTDIR)$(pkglibdir) && rm -f gbrainy.Core.dll

EXTRA_DIST = 			\
	$(CSFILES) $(RES)

CLEANFILES =			\
	$(TARGET)		\
	$(TARGET).mdb

