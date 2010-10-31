EXTRAFLAGS = $(CSC_DEFINES)

GAMESXMLSTRINGS_CSFILES = \
	$(srcdir)/GameXmlToGetString.cs

TRANSLATIONSCHECKER_CSFILES = \
	$(srcdir)/TranslationsChecker.cs \
	$(srcdir)/GetTextParser/CatalogParser.cs \
	$(srcdir)/GetTextParser/StringEscaping.cs

ASSEMBLIES = \
	-r:../src/gbrainy.Core.dll	\
	-r:Mono.Posix

GameXmlToGetString.exe: $(GAMESXMLSTRINGS_CSFILES) ../data/games.xml $(srcdir)/GameXmlGetStringTemplate.cs
		   $(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(GAMESXMLSTRINGS_CSFILES) $(ASSEMBLIES)
		   export MONO_PATH=../src && $(MONO) $@ $(srcdir)

TranslationsChecker.exe: $(TRANSLATIONSCHECKER_CSFILES)
		   $(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(TRANSLATIONSCHECKER_CSFILES) $(ASSEMBLIES)

all: GameXmlToGetString.exe

EXTRA_DIST = $(GAMESXMLSTRINGS_CSFILES) $(srcdir)/GameXmlGetStringTemplate.cs  $(srcdir)/GameXmlGetString.cs

CLEANFILES = GameXmlToGetString.exe \
	     GameXmlGetString.cs

DISTCLEANFILES = 				\
	Makefile

