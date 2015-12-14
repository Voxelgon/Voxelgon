#! /bin/bash

# Example build script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# Change this the name of your project. This will be the name of the final executables as well.
project="voxelgon"

echo "Running unit tests"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -runEditorTests \
  -editorTestsResultFile $(pwd)/unit_test_results.xml \
  -projectPath $(pwd) \
  -quit

if [ $? != 0 ]; then
  echo "Unit tests failed!"
  cat $(pwd)/unit_test_results.xml
  exit 1;
fi
echo "Unit tests passed!"

echo "Attempting to build $project for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildWindowsPlayer "$(pwd)/Build/windows/$project.exe" \
  -quit

echo "Attempting to build $project for OS X"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildOSXUniversalPlayer "$(pwd)/Build/osx/$project.app" \
  -quit

echo "Attempting to build $project for Linux"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildLinuxUniversalPlayer "$(pwd)/Build/linux/$project.exe" \
  -quit

grep -q "Scripts have compiler errors." $(pwd)/unity.log
if [ $? == 0 ]; then
  echo "Build Failed!"
  echo "Logs:"
  cat $(pwd)/unity.log
  exit 1;
fi

echo "Build completed without errors"
exit 0