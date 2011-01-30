#
# This script creates a package for deploying gbrainy on a web server
#

rm package -r -f
mkdir package
cd package

mkdir bin
mkdir tmp
mkdir po
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

mkdir locale/ca
mkdir locale/ca/LC_MESSAGES
cp ../../../../po/ca.gmo locale/ca/LC_MESSAGES/gbrainy.mo

mkdir locale/es
mkdir locale/es/LC_MESSAGES
cp ../../../../po/es.gmo locale/es/LC_MESSAGES/gbrainy.mo

mkdir locale/de
mkdir locale/de/LC_MESSAGES
cp ../../../../po/de.gmo locale/de/LC_MESSAGES/gbrainy.mo

tar -cvf gbrainy_web.tar *
cp gbrainy_web.tar ../
ls -l ../gbrainy_web.tar
