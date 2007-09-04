EXTRAFLAGS =  -nowarn:0169

GBRAINY_CSDISTFILES =				\
	$(srcdir)/ArrayListIndicesRandom.cs	\
	$(srcdir)/GameManager.cs		\
	$(srcdir)/PuzzleRectangles.cs		\
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
	$(srcdir)/GameType.cs			\
	$(srcdir)/MemoryColouredFigures.cs	\
	$(srcdir)/GameSession.cs		\
	$(srcdir)/MemoryNumbers.cs		\
	$(srcdir)/Memory.cs			\
	$(srcdir)/MemoryColouredText.cs		\
	$(srcdir)/gbrainy.cs			

ASSEMBLIES = \
	-r:Mono.Cairo.dll		\
	-pkg:gtk-sharp-2.0		\
	-pkg:glade-sharp-2.0		\
	-pkg:gnome-sharp-2.0		\
	-r:Mono.Posix



RESOURCES =										\
-resource:$(srcdir)/gbrainy.glade  \
-resource:$(top_srcdir)/icons/gbrainy.png,  \
-resource:$(top_srcdir)/icons/gbrainy.svg

bin_SCRIPTS = gbrainy \
	      gbrainy.exe

GBRAINY_CSFILES = $(GBRAINY_CSDISTFILES)	\
	Defines.cs			

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

