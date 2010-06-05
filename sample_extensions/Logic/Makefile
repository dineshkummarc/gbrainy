TARGET = gbrainy.logic_sample_extension.dll

all: PuzzleSample.cs PuzzleSample.addin.xml
	gmcs -t:library PuzzleSample.cs -resource:PuzzleSample.addin.xml -pkg:gbrainy -out:$(TARGET)

clean:
	rm $(TARGET) -r -f
