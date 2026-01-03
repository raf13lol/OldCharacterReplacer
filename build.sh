#!/bin/bash

echo "Building BPE5 build..."
dotnet build /p:BPE5=1
cp bin/Debug/netstandard2.1/com.rhythmdr.oldcharacterreplacer.dll bin/Debug/netstandard2.1/com.rhythmdr.bpe5oldcharacterreplacer.dll
echo "Building BPE6 build..."
dotnet build