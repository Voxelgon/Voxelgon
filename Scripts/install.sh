#! /bin/sh

# Example install script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# hosted on personal server
echo 'Downloading from http://pileof.rocks/Unity.pkg: '
curl -o Unity.pkg http://pileof.rocks/Unity.pkg

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /