EXTRAFLAGS =  -unsafe -nowarn:0169 $(CSC_DEFINES) 

TARGET = ../../gbrainy.Web.dll

GBRAINY_CSDISTFILES =		\
	$(srcdir)/Download.aspx.cs \
	$(srcdir)/Default.aspx.cs \
	$(srcdir)/Download.aspx.designer.cs \
	$(srcdir)/Default.aspx.designer.cs \
	$(srcdir)/Game.aspx.cs \
	$(srcdir)/Game.aspx.designer.cs \
	$(srcdir)/MasterPage.master.cs \
	$(srcdir)/MasterPage.master.designer.cs \
	$(srcdir)/LanguageSupport.cs \
	$(srcdir)/Logger.cs \
	$(srcdir)/Status.aspx.cs \
	$(srcdir)/Status.aspx.designer.cs \
	$(srcdir)/Global.asax.cs \
	$(srcdir)/WebSession.cs \
	$(srcdir)/TranslationsWeb.cs \
	$(srcdir)/Defines.cs \
	$(srcdir)/Help.aspx.cs \
	$(srcdir)/Help.aspx.designer.cs \
	$(srcdir)/AllGames.aspx.cs \
	$(srcdir)/AllGames.aspx.designer.cs \
	$(srcdir)/GameImage.cs \
	$(srcdir)/GameImageAreaShape.cs


ASSEMBLIES = \
	 $(GBRAINY_LIBS)    		\
	 $(MONO_ADDINS_LIBS)    	\
	-r:System.Web			\
	-r:Mono.Cairo.dll		\
	-r:../../gbrainy.Core.dll	\
	-r:Mono.Posix

RESOURCES =

GBRAINY_CSFILES = $(GBRAINY_CSDISTFILES)	\
	AssemblyInfo.cs

if ENABLE_WEBCLIENT

../../gbrainy.Web.dll: $(GBRAINY_CSFILES)
	$(CSC) -target:library -out:$@ $(EXTRAFLAGS) $(GBRAINY_CSFILES) $(ASSEMBLIES) $(RESOURCES)


all: ../../gbrainy.Web.dll

EXTRA_DIST = $(GBRAINY_CSDISTFILES)

endif

CLEANFILES =				\
	../../gbrainy.Web.dll.mdb	\
	../../gbrainy.Web.dll


DISTCLEANFILES = 			\
	Makefile

