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
	$(srcdir)/PuzzleSquaresAndLetters.cs	\
	$(srcdir)/PuzzleSquareDots.cs		\
	$(srcdir)/PuzzleNumericRelation.cs	\
	$(srcdir)/PuzzleNextFigure.cs		\
	$(srcdir)/PuzzleSquareSheets.cs		\
	$(srcdir)/CalculationArithmetical.cs	\
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
	$(srcdir)/CalculationGreatestDivisor.cs	\
	$(srcdir)/CalculationTwoNumbers.cs	\
	$(srcdir)/CalculationWhichNumber.cs	\
	$(srcdir)/PuzzleMatrixGroups.cs		\
	$(srcdir)/PuzzleBalance.cs		\
	$(srcdir)/PuzzleTrianglesWithNumbers.cs	\
	$(srcdir)/PuzzleOstracism.cs		\
	$(srcdir)/MemoryCountDots.cs		\
	$(srcdir)/CalculationOperator.cs	\
	$(srcdir)/PuzzleFigurePattern.cs	\
	$(srcdir)/ColorPalette.cs		\
	$(srcdir)/PuzzlePeopleTable.cs		\
	$(srcdir)/GameDrawingArea.cs		\
	$(srcdir)/MemoryFigures.cs		\
	$(srcdir)/PuzzleMissingSlice.cs		\
	$(srcdir)/PuzzleLines.cs		\
	$(srcdir)/PuzzleTetris.cs		\
	$(srcdir)/PreferencesDialog.cs		\
	$(srcdir)/PuzzleMissingPiece.cs		\
	$(srcdir)/MemoryIndications.cs		\
	$(srcdir)/PuzzleMostInCommon.cs		\
	$(srcdir)/PuzzleBuildTriangle.cs	\
	$(srcdir)/CairoContextEx.cs		\
	$(srcdir)/PuzzleClocks.cs		\
	$(srcdir)/PuzzleCountCircles.cs		\
	$(srcdir)/PuzzleEquation.cs		\
	$(srcdir)/PuzzleQuadrilaterals.cs	\
	$(srcdir)/CalculationFractions.cs	\
	$(srcdir)/PuzzleExtraCircle.cs		\
	$(srcdir)/PuzzleCountSeries.cs		\
	$(srcdir)/PlayerHistory.cs		\
	$(srcdir)/PlayerHistoryDialog.cs	\
	$(srcdir)/GtkDialog.cs			\
	$(srcdir)/Preferences.cs		\
	$(srcdir)/PuzzleFourSided.cs		\
	$(srcdir)/gbrainy.cs			

ASSEMBLIES = \
	 $(GBRAINY_LIBS)    		\
	-r:Mono.Cairo.dll		\
	-r:Mono.Posix

RESOURCES =										\
-resource:$(srcdir)/gbrainy.glade  \
-resource:$(top_srcdir)/data/resume-32.png  \
-resource:$(top_srcdir)/data/endgame-32.png  \
-resource:$(top_srcdir)/data/pause-32.png  \
-resource:$(top_srcdir)/data/allgames-32.png  \
-resource:$(top_srcdir)/data/gbrainy.png  \
-resource:$(top_srcdir)/data/logic-games-32.png  \
-resource:$(top_srcdir)/data/math-games-32.png  \
-resource:$(top_srcdir)/data/memory-games-32.png  \
-resource:$(top_srcdir)/data/gbrainy.svg

gbrainydir = $(libdir)/gbrainy
gbrainy_SCRIPTS = gbrainy.exe
bin_SCRIPTS = gbrainy

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
	Makefile

