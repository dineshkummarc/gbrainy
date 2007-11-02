EXTRAFLAGS =  -nowarn:0169

WRAPPER = gbrainy

GBRAINY_CSDISTFILES =				\
	$(srcdir)/ArrayListIndicesRandom.cs	\
	$(srcdir)/GameManager.cs		\
	$(srcdir)/PuzzleCirclesRectangle.cs	\
	$(srcdir)/Game.cs			\
	$(srcdir)/PuzzleFigures.cs		\
	$(srcdir)/PuzzleMatrixNumbers.cs	\
	$(srcdir)/PuzzleMoveFigure.cs		\
	$(srcdir)/PuzzlePencil.cs		\
	$(srcdir)/PuzzleSquares.cs		\
	$(srcdir)/PuzzleTriangles.cs		\
	$(srcdir)/PuzzleCoverPercentage.cs	\
	$(srcdir)/PuzzleNumericSequence.cs	\
	$(srcdir)/PuzzleAlphabeticSequence.cs	\
	$(srcdir)/PuzzleSquareDots.cs		\
	$(srcdir)/PuzzleNumericRelation.cs	\
	$(srcdir)/PuzzleNextFigure.cs		\
	$(srcdir)/GameOverDialog.cs		\
	$(srcdir)/PuzzleSquareSheets.cs		\
	$(srcdir)/MathArithmetical.cs		\
	$(srcdir)/MemoryColouredFigures.cs	\
	$(srcdir)/GameSession.cs		\
	$(srcdir)/MemoryNumbers.cs		\
	$(srcdir)/Memory.cs			\
	$(srcdir)/MemoryColouredText.cs		\
	$(srcdir)/PuzzleCube.cs			\
	$(srcdir)/MemoryWords.cs		\
	$(srcdir)/PuzzleFigureLetter.cs		\
	$(srcdir)/CustomGameDialog.cs		\
	$(srcdir)/PuzzleDivideCircle.cs		\
	$(srcdir)/MathGreaterDivisor.cs		\
	$(srcdir)/MathTwoNumbers.cs		\
	$(srcdir)/MathWhichNumber.cs		\
	$(srcdir)/PuzzleMatrixGroups.cs		\
	$(srcdir)/PuzzleBalance.cs		\
	$(srcdir)/PuzzlePairs.cs		\
	$(srcdir)/PuzzleOstracism.cs		\
	$(srcdir)/MemoryCountDots.cs		\
	$(srcdir)/DrawingHelpers.cs		\
	$(srcdir)/MathOperator.cs		\
	$(srcdir)/PuzzleFigurePattern.cs	\
	$(srcdir)/ColorPalette.cs		\
	$(srcdir)/gbrainy.cs			

ASSEMBLIES = \
	 $(GBRAINY_LIBS)    		\
	-r:Mono.Cairo.dll		\
	-r:Mono.Posix



RESOURCES =										\
-resource:$(srcdir)/gbrainy.glade  \
-resource:$(top_srcdir)/data/gbrainy.png,  \
-resource:$(top_srcdir)/data/gbrainy.svg

bin_SCRIPTS = $(WRAPPER) \
	      gbrainy.exe

$(WRAPPER): $(srcdir)/$(WRAPPER).in Makefile
	sed -e "s|\@prefix\@|$(prefix)|g" 		\
	    -e "s|\@pkglibdir\@|$(pkglibdir)|g" 	\
	    -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@target\@|$(TARGET)|g" 		\
	    -e "s|\@wrapper\@|$(WRAPPER)|g"		\
	    -e "s|\@extragac\@|$(EXTRA_GAC_PREFIX)|g"	\
	    -e "s|\@exec_prefix\@|$(bindir)|g"			\
	    -e "s|\@srcdir\@|$(PWD)|g"	\
	    < $< > $@
	chmod +x $(WRAPPER)

GBRAINY_CSFILES = $(GBRAINY_CSDISTFILES)	\
	Defines.cs \
	AssemblyInfo.cs

gbrainy.exe: $(GBRAINY_CSFILES) 
	$(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(GBRAINY_CSFILES) $(ASSEMBLIES) $(RESOURCES)

all: gbrainy.exe

EXTRA_DIST =					\
	gbrainy.glade \
	$(GBRAINY_CSDISTFILES)			


CLEANFILES =					\
	gbrainy.exe.config			\
	gbrainy.exe.mdb				\
	gbrainy.exe				\
	gbrainy


DISTCLEANFILES = 				\
	makefile

