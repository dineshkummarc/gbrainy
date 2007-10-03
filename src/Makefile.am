EXTRAFLAGS =  -nowarn:0169

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
	$(srcdir)/PuzzleMatrixGroups.cs		\
	$(srcdir)/gbrainy.cs			

ASSEMBLIES = \
	 $(GBRAINY_LIBS)    		\
	-r:Mono.Cairo.dll		\
	-r:Mono.Posix



RESOURCES =										\
-resource:$(srcdir)/gbrainy.glade  \
-resource:$(top_srcdir)/data/gbrainy.png,  \
-resource:$(top_srcdir)/data/gbrainy.svg

bin_SCRIPTS = gbrainy \
	      gbrainy.exe

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

