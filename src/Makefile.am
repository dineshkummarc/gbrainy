EXTRAFLAGS =  -nowarn:0169 $(CSC_DEFINES)

WRAPPER = gbrainy

GBRAINY_CSDISTFILES =				\
	$(srcdir)/ArrayListIndicesRandom.cs	\
	$(srcdir)/GameManager.cs		\
	$(srcdir)/PuzzleGames/PuzzleCirclesRectangle.cs	\
	$(srcdir)/Game.cs			\
	$(srcdir)/PuzzleGames/PuzzleFigures.cs		\
	$(srcdir)/PuzzleGames/PuzzleMatrixNumbers.cs	\
	$(srcdir)/PuzzleGames/PuzzleMoveFigure.cs		\
	$(srcdir)/PuzzleGames/PuzzlePencil.cs		\
	$(srcdir)/PuzzleGames/PuzzleSquares.cs		\
	$(srcdir)/PuzzleGames/PuzzleTriangles.cs		\
	$(srcdir)/PuzzleGames/PuzzleCoverPercentage.cs	\
	$(srcdir)/PuzzleGames/PuzzleNumericSequence.cs	\
	$(srcdir)/PuzzleGames/PuzzleSquaresAndLetters.cs	\
	$(srcdir)/PuzzleGames/PuzzleSquareDots.cs		\
	$(srcdir)/PuzzleGames/PuzzleNumericRelation.cs	\
	$(srcdir)/PuzzleGames/PuzzleNextFigure.cs		\
	$(srcdir)/PuzzleGames/PuzzleSquareSheets.cs		\
	$(srcdir)/CalculationGames/CalculationArithmetical.cs	\
	$(srcdir)/MemoryGames/MemoryColouredFigures.cs	\
	$(srcdir)/GameSession.cs		\
	$(srcdir)/MemoryGames/MemoryFiguresNumbers.cs		\
	$(srcdir)/Memory.cs			\
	$(srcdir)/MemoryGames/MemoryColouredText.cs		\
	$(srcdir)/PuzzleGames/PuzzleCube.cs			\
	$(srcdir)/MemoryGames/MemoryWords.cs		\
	$(srcdir)/PuzzleGames/PuzzleFigureLetter.cs		\
	$(srcdir)/Dialogs/CustomGameDialog.cs		\
	$(srcdir)/PuzzleGames/PuzzleDivideCircle.cs		\
	$(srcdir)/CalculationGames/CalculationGreatestDivisor.cs	\
	$(srcdir)/CalculationGames/CalculationTwoNumbers.cs	\
	$(srcdir)/CalculationGames/CalculationCloserFraction.cs	\
	$(srcdir)/PuzzleGames/PuzzleMatrixGroups.cs		\
	$(srcdir)/PuzzleGames/PuzzleBalance.cs		\
	$(srcdir)/PuzzleGames/PuzzleTrianglesWithNumbers.cs	\
	$(srcdir)/PuzzleGames/PuzzleOstracism.cs		\
	$(srcdir)/MemoryGames/MemoryCountDots.cs		\
	$(srcdir)/CalculationGames/CalculationOperator.cs	\
	$(srcdir)/PuzzleGames/PuzzleFigurePattern.cs	\
	$(srcdir)/ColorPalette.cs		\
	$(srcdir)/PuzzleGames/PuzzlePeopleTable.cs		\
	$(srcdir)/GameDrawingArea.cs		\
	$(srcdir)/MemoryGames/MemoryFigures.cs		\
	$(srcdir)/PuzzleGames/PuzzleMissingSlice.cs		\
	$(srcdir)/PuzzleGames/PuzzleLines.cs		\
	$(srcdir)/PuzzleGames/PuzzleTetris.cs		\
	$(srcdir)/Dialogs/PreferencesDialog.cs		\
	$(srcdir)/PuzzleGames/PuzzleMissingPiece.cs		\
	$(srcdir)/MemoryGames/MemoryIndications.cs		\
	$(srcdir)/PuzzleGames/PuzzleMostInCommon.cs		\
	$(srcdir)/PuzzleGames/PuzzleBuildTriangle.cs	\
	$(srcdir)/CairoContextEx.cs		\
	$(srcdir)/PuzzleGames/PuzzleClocks.cs		\
	$(srcdir)/PuzzleGames/PuzzleCountCircles.cs		\
	$(srcdir)/PuzzleGames/PuzzleEquation.cs		\
	$(srcdir)/PuzzleGames/PuzzleQuadrilaterals.cs	\
	$(srcdir)/CalculationGames/CalculationFractions.cs	\
	$(srcdir)/PuzzleGames/PuzzleExtraCircle.cs		\
	$(srcdir)/PuzzleGames/PuzzleCountSeries.cs		\
	$(srcdir)/PlayerHistory.cs		\
	$(srcdir)/Dialogs/PlayerHistoryDialog.cs	\
	$(srcdir)/Dialogs/GtkDialog.cs			\
	$(srcdir)/Preferences.cs		\
	$(srcdir)/PuzzleGames/PuzzleFourSided.cs		\
	$(srcdir)/PuzzleGames/PuzzleLargerShape.cs		\
	$(srcdir)/SVGImage.cs			\
	$(srcdir)/MemoryGames/MemoryNumbers.cs		\
	$(srcdir)/Dialogs/AboutDialog.cs	\
	$(srcdir)/PuzzleGames/PuzzleHandshakes.cs	\
	$(srcdir)/CalculationGames/CalculationPrimes.cs	\
	$(srcdir)/MemoryGames/MemoryFacts.cs		\
	$(srcdir)/PuzzleGames/PuzzleCounting.cs	\
	$(srcdir)/VerbalAnalogies/Analogy.cs \
	$(srcdir)/VerbalAnalogies/AnalogiesFactory.cs \
	$(srcdir)/VerbalAnalogies/Analogies.cs \
	$(srcdir)/VerbalAnalogies/AnalogiesQuestionAnswer.cs \
	$(srcdir)/VerbalAnalogies/AnalogiesMultipleOptions.cs \
	$(srcdir)/VerbalAnalogies/AnalogiesPairOfWordsOptions.cs \
	$(srcdir)/VerbalAnalogies/AnalogiesPairOfWordsCompare.cs \
	$(srcdir)/CalculationGames/CalculationAverage.cs \
	$(srcdir)/CalculationGames/CalculationProportions.cs \
	$(srcdir)/CalculationGames/CalculationRatio.cs \
	$(srcdir)/PuzzleGames/PuzzlePercentage.cs	\
	$(srcdir)/SimpleLabel.cs	\
	$(srcdir)/gbrainy.cs			


ASSEMBLIES = \
	 $(GBRAINY_LIBS)    		\
	 $(MONO_ADDINS_LIBS)    	\
	-r:Mono.Cairo.dll		\
	-r:Mono.Posix

RESOURCES =										\
-resource:$(srcdir)/gbrainy.glade  \
-resource:$(top_srcdir)/data/app-graphics/resume-32.png  \
-resource:$(top_srcdir)/data/app-graphics/endgame-32.png  \
-resource:$(top_srcdir)/data/app-graphics/pause-32.png  \
-resource:$(top_srcdir)/data/app-graphics/allgames-32.png  \
-resource:$(top_srcdir)/data/app-graphics/gbrainy.png  \
-resource:$(top_srcdir)/data/app-graphics/logic-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/math-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/memory-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/verbal-games.svg  \
-resource:$(top_srcdir)/data/app-graphics/verbal-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/gbrainy.svg  \
-resource:$(top_srcdir)/data/game-graphics/present.svg  \
-resource:$(top_srcdir)/data/game-graphics/handshake.svg  \
-resource:$(srcdir)/gbrainy.addin.xml

gbrainydir = $(libdir)/gbrainy
gbrainy_SCRIPTS = gbrainy.exe ../data/gbrainy.exe.config
bin_SCRIPTS = gbrainy

GBRAINY_CSFILES = $(GBRAINY_CSDISTFILES)	\
	Defines.cs \
	AssemblyInfo.cs

gbrainy.exe: $(GBRAINY_CSFILES) 
	$(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(GBRAINY_CSFILES) $(ASSEMBLIES) $(RESOURCES)

all: gbrainy.exe

EXTRA_DIST =					\
	gbrainy.glade \
	$(srcdir)/gbrainy.addin.xml \
	$(srcdir)/mono-addins-strings.xml \
	$(GBRAINY_CSDISTFILES)			


CLEANFILES =					\
	gbrainy.exe.config			\
	gbrainy.exe.mdb				\
	gbrainy.exe				\
	gbrainy


DISTCLEANFILES = 				\
	Makefile

