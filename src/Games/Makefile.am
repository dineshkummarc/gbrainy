CSFLAGS = -target:library 

TARGET = ../gbrainy.Games.dll

CSFILES =  \
		AssemblyInfo.cs					\
		GameList.cs					\
		Logic/Puzzle3DCube.cs				\
		Logic/PuzzleBalance.cs				\
		Logic/PuzzleBuildTriangle.cs			\
		Logic/PuzzleCirclesRectangle.cs			\
		Logic/PuzzleClocks.cs				\
		Logic/PuzzleCountCircles.cs			\
		Logic/PuzzleCounting.cs				\
		Logic/PuzzleCountSeries.cs			\
		Logic/PuzzleCoverPercentage.cs			\
		Logic/PuzzleCube.cs				\
		Logic/PuzzleDivideCircle.cs			\
		Logic/PuzzleEquation.cs				\
		Logic/PuzzleExtraCircle.cs			\
		Logic/PuzzleFigureLetter.cs			\
		Logic/PuzzleFigurePattern.cs			\
		Logic/PuzzleFigures.cs				\
		Logic/PuzzleFourSided.cs			\
		Logic/PuzzleHandshakes.cs			\
		Logic/PuzzleLargerShape.cs			\
		Logic/PuzzleLines.cs				\
		Logic/PuzzleMatrixGroups.cs			\
		Logic/PuzzleMatrixNumbers.cs			\
		Logic/PuzzleMissingPiece.cs			\
		Logic/PuzzleMissingSlice.cs			\
		Logic/PuzzleMostInCommon.cs			\
		Logic/PuzzleMoveFigure.cs			\
		Logic/PuzzleNextFigure.cs			\
		Logic/PuzzleNumericRelation.cs			\
		Logic/PuzzleNumericSequence.cs			\
		Logic/PuzzleOstracism.cs			\
		Logic/PuzzlePencil.cs				\
		Logic/PuzzlePeopleTable.cs			\
		Logic/PuzzlePercentage.cs			\
		Logic/PuzzleQuadrilaterals.cs			\
		Logic/PuzzleSquareDots.cs			\
		Logic/PuzzleSquaresAndLetters.cs		\
		Logic/PuzzleSquares.cs				\
		Logic/PuzzleSquareSheets.cs			\
		Logic/PuzzleTetris.cs				\
		Logic/PuzzleTimeNow.cs				\
		Logic/PuzzleTriangles.cs			\
		Logic/PuzzleTrianglesWithNumbers.cs		\
		Memory/MemoryColouredFigures.cs			\
		Memory/MemoryColouredText.cs			\
		Memory/MemoryCountDots.cs			\
		Memory/MemoryFacts.cs				\
		Memory/MemoryFigures.cs				\
		Memory/MemoryFiguresNumbers.cs			\
		Memory/MemoryIndications.cs			\
		Memory/MemoryNumbers.cs				\
		Memory/MemoryWords.cs				\
		Calculation/CalculationArithmetical.cs		\
		Calculation/CalculationAverage.cs		\
		Calculation/CalculationCloserFraction.cs 	\
		Calculation/CalculationFractions.cs		\
		Calculation/CalculationGreatestDivisor.cs 	\
		Calculation/CalculationOperator.cs		\
		Calculation/CalculationPrimes.cs		\
		Calculation/CalculationProportions.cs		\
		Calculation/CalculationRatio.cs			\
		Calculation/CalculationTwoNumbers.cs

RES = \
$(top_srcdir)/data/game-graphics/present.svg  \
$(top_srcdir)/data/game-graphics/handshake.svg

REFS =  \
	-r:System \
	-r:Mono.Cairo.dll		\
	-r:Mono.Posix			\
	-r:.././gbrainy.Core.dll

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
	cd $(DESTDIR)$(pkglibdir) && rm -f gbrainy.Games.dll

EXTRA_DIST = 			\
	$(CSFILES) $(RES)

CLEANFILES =			\
	$(TARGET)		\
	$(TARGET).mdb

