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
cp ../*.asax .
cp ../web.config .

cp ../bin/* bin
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
