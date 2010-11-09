CORE = gbrainy.Core.Test.dll
CLASSICAL = gbrainy.ClassicalClient.Test.dll
GAMES = gbrainy.Games.Test.dll

CSFLAGS =				\
	-debug				\
	-define:DEBUG			\
	-target:library

CSFILES_CORE =					\
	$(srcdir)/Core/AnalogiesFactoryTest.cs	\
	$(srcdir)/Core/PlayerHistoryTest.cs	\
	$(srcdir)/Core/PlayerPersonalRecordTest.cs \
	$(srcdir)/Core/GameSessionTest.cs \
	$(srcdir)/Core/GameManagerTest.cs \
	$(srcdir)/Core/GameTest.cs \
	$(srcdir)/Core/GameXmlFactoryTest.cs

CSFILES_CLASSICAL =					\
	$(srcdir)/Clients/Classical/CommandLineTest.cs

CSFILES_GAMES =						\
	$(srcdir)/Games/Logic/PuzzleNumericRelationTest.cs

if ENABLE_TESTS

ASSEMBLIES = \
	$(NUNIT_LIBS)			\
	$(MONO_ADDINS_LIBS)		\
	-r:../src/gbrainy.Core.dll	\
	-r:../src/gbrainy.exe

MONO_PATH = .:$(top_builddir)/src:

RESSOURCES =

$(CORE): $(CSFILES_CORE) $(top_builddir)/src/gbrainy.exe
	$(CSC) -out:$@ $(CSFLAGS) $(CSFILES_CORE) $(ASSEMBLIES) $(RESSOURCES)

$(CLASSICAL): $(CSFILES_CLASSICAL) $(top_builddir)/src/gbrainy.exe
	$(CSC) -out:$@ $(CSFLAGS) $(CSFILES_CLASSICAL) $(ASSEMBLIES) $(RESSOURCES)

$(GAMES): $(CSFILES_GAMES) $(top_builddir)/src/gbrainy.exe
	$(CSC) -out:$@ $(CSFLAGS) $(CSFILES_GAMES) $(ASSEMBLIES) -r:../src/gbrainy.Games.dll $(RESSOURCES)

all: $(CORE) $(CLASSICAL) $(GAMES)

run: $(CORE) $(CLASSICAL)  $(GAMES)
	MONO_PATH=$(MONO_PATH) $(NUNIT) $(CLASSICAL) -nologo
	MONO_PATH=$(MONO_PATH) $(NUNIT) $(CORE) -nologo
	MONO_PATH=$(MONO_PATH) $(NUNIT) $(GAMES) -nologo

endif

EXTRA_DIST = 				\
	$(CSFILES_CORE)			\
	$(CSFILES_CLASSICAL)

CLEANFILES = 				\
	$(CORE)				\
	$(CORE).mdb			\
	$(CLASSICAL)			\
	$(CLASSICAL).mdb		\
	TestResult.xml

.PHONY: test
