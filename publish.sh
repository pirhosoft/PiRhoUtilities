mv Assets/PiRhoUtilities/Samples Assets/PiRhoUtilities/Samples~
mv Assets/PiRhoUtilities/Samples.meta Assets/Samples.mata
cp -f README.md Assets/PiRhoUtilities

git add .
git commit -m "publishing branch"
git subtree push --prefix Assets/PiRhoUtilities origin upm

mv Assets/PiRhoUtilities/Samples~ Assets/PiRhoUtilities/Samples
mv Assets/Samples.meta Assets/PiRhoUtilities/Samples.mata
