mv Assets/PiRhoUtilities/Samples Assets/PiRhoUtilities/Samples~
cp -f README.md Assets/PiRhoUtilities

git subtree push --prefix Assets/PiRhoUtilities origin upm

mv Assets/PiRhoUtilities/Samples~ Assets/PiRhoUtilities/Samples
