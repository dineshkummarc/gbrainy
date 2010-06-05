TARGET = gbrainy.memory_sample_extension.dll

all: MemorySample.cs MemorySample.addin.xml
	gmcs -t:library MemorySample.cs -resource:MemorySample.addin.xml -pkg:gbrainy -out:$(TARGET)

clean:
	rm $(TARGET) -r -f
