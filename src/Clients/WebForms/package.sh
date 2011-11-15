#
# This script creates a package for deploying gbrainy on a web server
#

rm package -r -f
mkdir package
cd package

mkdir bin
mkdir tmp
mkdir data
mkdir images
mkdir locale
mkdir themes

cp ../*.aspx .
cp ../*.master .
cp ../*.css .
cp ../*.js .
cp ../*.asax .
cp ../web.config .

cp ../bin/*.dll bin
cp ../bin/*.config bin
cp ../../../../data/*.xml data
cp ../../../../data/game-graphics/* images
cp ../../../../data/app-graphics/* images
cp ../../../../data/themes/* themes

LOCALES=(af ca cs da de eu es fr gl hu nl pt ro pt_BR sl sr sv)

for i in "${LOCALES[@]}"
do
	:
	mkdir locale/$i
	mkdir locale/$i/LC_MESSAGES
	cp ../../../../po/$i.gmo locale/$i/LC_MESSAGES/gbrainy.mo
   echo $i
done


tar -cvf gbrainy_web.tar *
cp gbrainy_web.tar ../
ls -l ../gbrainy_web.tar
if (grep 'Production' web.config > null && grep 'value="0"' web.config > null); then
	echo WARNING: Production setting is set to 0 - not production ready
fi
