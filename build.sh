#!/bin/bash

echo "Building BPE5 build..."
dotnet build /p:BPE5=1
cp bin/Debug/netstandard2.1/com.rhythmdr.oldcharacterreplacer.dll bin/Debug/netstandard2.1/com.rhythmdr.bpe5oldcharacterreplacer.dll
echo "Building BPE6 build..."
dotnet build
echo "Zipping files..."
rm -rf com.rhythmdr.oldcharacterreplacer.zip
rm -rf com.rhythmdr.bpe5oldcharacterreplacer.zip
zip -r com.rhythmdr.oldcharacterreplacer.zip ocrData bin/Debug/netstandard2.1/com.rhythmdr.oldcharacterreplacer.dll
zip -r com.rhythmdr.bpe5oldcharacterreplacer.zip ocrData bin/Debug/netstandard2.1/com.rhythmdr.bpe5oldcharacterreplacer.dll