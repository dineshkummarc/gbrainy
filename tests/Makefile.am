TARGET = gbrainy.Core.Test.dll

CSFLAGS =				\
	-debug				\
	-define:DEBUG			\
	-target:library

CSFILES =					\
	$(srcdir)/Core/AnalogiesFactoryTest.cs	\
	$(srcdir)/Core/PlayerHistoryTest.cs	\
	$(srcdir)/Core/PlayerPersonalRecordTest.cs

ASSEMBLIES = \
	$(NUNIT_LIBS)			\
	$(MONO_ADDINS_LIBS)		\
	-pkg:gbrainy

MONO_PATH = .:$(top_builddir)/src:

RESSOURCES =

$(TARGET): $(CSFILES) $(top_builddir)/src/gbrainy.exe
	$(CSC) -out:$@ $(CSFLAGS) $(CSFILES) $(ASSEMBLIES) $(RESSOURCES)

test: $(TARGET)
	MONO_PATH=$(MONO_PATH) $(NUNIT) $(TARGET) -nologo

EXTRA_DIST = 				\
	$(CSFILES)

CLEANFILES = 				\
	$(TARGET)			\
	$(TARGET).mdb			\
	TestResult.xml

.PHONY: test
