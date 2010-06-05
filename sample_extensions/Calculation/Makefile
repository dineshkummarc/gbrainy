TARGET = gbrainy.calculation_sample_extension.dll

all: CalculationSample.cs CalculationSample.addin.xml
	gmcs -t:library CalculationSample.cs -resource:CalculationSample.addin.xml -pkg:gbrainy -out:$(TARGET)

clean:
	rm $(TARGET) -r -f
